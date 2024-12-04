using Azure.Messaging.ServiceBus;
using System.Text.Json;
using SBSharedLib.Models;
using Microsoft.Extensions.Configuration;

namespace SBTopicSubscriber
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Check if subscription name was provided as command line argument
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a subscription name as a command line argument.");
                return;
            }
            string subscriptionName = args[0];

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

            Console.WriteLine($"Starting to receive messages for subscription {subscriptionName}... Press Ctrl+C to exit");

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync(cancellationToken: cts.Token);
                    if (message != null)
                    {
                        try
                        {
                            string jsonBody = message.Body.ToString();
                            Person? person = JsonSerializer.Deserialize<Person>(jsonBody);

                            if (person != null)
                            {
                                // Get the message type from application properties
                                string messageType = "Unknown";
                                if (message.ApplicationProperties.ContainsKey("MessageType"))
                                {
                                    messageType = message.ApplicationProperties["MessageType"].ToString() ?? "Unknown";
                                }

                                Console.WriteLine($"[{subscriptionName}] Received {messageType} person - Name: {person.FirstName} {person.LastName}");
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
                            await receiver.DeadLetterMessageAsync(
                                message,
                                "DeserializationError",
                                $"Failed to deserialize to Person: {jsonEx.Message}"
                            );
                        }
                        catch (Exception messageException)
                        {
                            Console.WriteLine($"Error processing message {message.MessageId}: {messageException.Message}");
                            try
                            {
                                await receiver.DeadLetterMessageAsync(
                                    message,
                                    "ProcessingError",
                                    $"Error: {messageException.Message}"
                                );
                                Console.WriteLine($"Message {message.MessageId} moved to dead letter queue");
                            }
                            catch (Exception deadLetterException)
                            {
                                Console.WriteLine($"Error moving message to dead letter queue: {deadLetterException.Message}");
                                await receiver.AbandonMessageAsync(message);
                                Console.WriteLine($"Message {message.MessageId} abandoned and returned to queue");
                            }
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

        private static async Task ProcessPersonAsync(Person person)
        {
            await Task.Run(() =>
            {
                if (string.IsNullOrEmpty(person.FirstName) || string.IsNullOrEmpty(person.LastName))
                {
                    throw new Exception("Person has missing required fields");
                }
                // Add any additional processing logic here
            });
        }
    }
}