using Azure.Messaging.ServiceBus;
using System.Text.Json;
using SBSharedLib.Models;
using Microsoft.Extensions.Configuration;

namespace SBTopicSubscriber
{
    class Program
    {
        private static readonly Dictionary<string, string> AvailableSubscriptions = new()
        {
            { "1", "s1" },
            { "2", "s2" }
        };

        static async Task Main(string[] args)
        {
            // Get subscription and filter choices
            string subscriptionName = GetSubscriptionChoice();
            string? messageTypeFilter = GetMessageTypeFilter();

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets<Program>()
                .Build();

            string connectionString = configuration.GetSection("ConnectionStrings:AzureServiceBus").Value
                ?? throw new InvalidOperationException("ServiceBus ConnectionString not found in configuration");
            string topicName = configuration.GetSection("AzureServiceBus:Topics:Person").Value
                ?? throw new InvalidOperationException("ServiceBus Topic name not found in configuration");

            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            await using ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusReceiver receiver = client.CreateReceiver(topicName, subscriptionName);

            string filterDisplay = messageTypeFilter != null ? $" (Filtering for {messageTypeFilter} messages)" : " (No message type filter)";
            Console.WriteLine($"Starting to receive messages for subscription {subscriptionName}{filterDisplay}...");
            Console.WriteLine("Press Ctrl+C to exit");

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync(
                        TimeSpan.FromSeconds(30),
                        cancellationToken: cts.Token);

                    if (message != null)
                    {
                        // Check if message matches our filter
                        string receivedMessageType = message.ApplicationProperties.TryGetValue("MessageType", out var type)
                            ? type?.ToString() ?? "Unspecified"
                            : "Unspecified";

                        // Skip message if it doesn't match our filter
                        if (messageTypeFilter != null && receivedMessageType != messageTypeFilter)
                        {
                            await receiver.CompleteMessageAsync(message);
                            continue;
                        }

                        try
                        {
                            string jsonBody = message.Body.ToString();
                            Person? person = JsonSerializer.Deserialize<Person>(jsonBody);

                            if (person != null)
                            {
                                Console.WriteLine($"[{subscriptionName}] Received message:");
                                Console.WriteLine($"Message Type: {receivedMessageType}");
                                Console.WriteLine($"Person: {person.FirstName} {person.LastName}");
                                Console.WriteLine($"Message ID: {message.MessageId}");
                                Console.WriteLine($"Enqueued Time: {message.EnqueuedTime}");
                                Console.WriteLine("------------------------");

                                await ProcessPersonAsync(person);
                                await receiver.CompleteMessageAsync(message);
                            }
                            else
                            {
                                throw new Exception("Failed to deserialize Person object - result was null");
                            }
                        }
                        catch (JsonException jsonEx)
                        {
                            Console.WriteLine($"Error deserializing message {message.MessageId}: {jsonEx.Message}");
                            await receiver.DeadLetterMessageAsync(message, "DeserializationError", jsonEx.Message);
                        }
                        catch (Exception messageException)
                        {
                            Console.WriteLine($"Error processing message {message.MessageId}: {messageException.Message}");
                            await receiver.DeadLetterMessageAsync(message, "ProcessingError", messageException.Message);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Receiver for subscription {subscriptionName} stopped.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error in message processing loop: {ex.Message}");
            }
            finally
            {
                await receiver.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        private static string GetSubscriptionChoice()
        {
            while (true)
            {
                Console.WriteLine("\nChoose a subscription:");
                Console.WriteLine("1: Maki Subscription");
                Console.WriteLine("2: Paun Subscription");

                string? choice = Console.ReadLine();
                if (AvailableSubscriptions.TryGetValue(choice ?? "", out string? subscriptionName))
                {
                    return subscriptionName;
                }

                Console.WriteLine("Invalid choice. Please try again.");
            }
        }

        private static string? GetMessageTypeFilter()
        {
            while (true)
            {
                Console.WriteLine("\nChoose message type to filter (or press Enter for no filter):");
                Console.WriteLine("1: VIP messages only");
                Console.WriteLine("2: Regular messages only");
                Console.WriteLine("Press Enter: Receive all messages");

                string? choice = Console.ReadLine();
                return choice switch
                {
                    "1" => "VIP",
                    "2" => "Regular",
                    "" => null,
                    _ => GetMessageTypeFilter() // Recurse if invalid input
                };
            }
        }

        private static async Task ProcessPersonAsync(Person person)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(person.FirstName) || string.IsNullOrEmpty(person.LastName))
                {
                    throw new Exception("Person has missing required fields");
                }
            });
        }
    }
}