using Azure.Messaging.ServiceBus;
using System;
using System.Text.Json;
using SBSharedLib.Models;
using Microsoft.Extensions.Configuration;

namespace SBQueSubscriber
{
    class Program
    {

        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>()  // Add this line
            .Build();

            // Get connection string and queue name from configuration
            string connectionString = configuration.GetSection("ConnectionStrings:AzureServiceBus").Value
                ?? throw new InvalidOperationException("ServiceBus ConnectionString not found in configuration");
            string queueName = configuration.GetSection("AzureServiceBus:Ques:Person").Value
                ?? throw new InvalidOperationException("ServiceBus QueName not found in configuration");

            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) => {
                e.Cancel = true;
                cts.Cancel();
            };

            await using ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusReceiver receiver = client.CreateReceiver(queueName);
            Console.WriteLine("Starting to receive messages... Press Ctrl+C to exit");

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync(cancellationToken: cts.Token);
                    if (message != null)
                    {
                        try
                        {
                            // Deserialize the message body to Person object
                            string jsonBody = message.Body.ToString();
                            Person? person = JsonSerializer.Deserialize<Person>(jsonBody);

                            if (person != null)
                            {
                                Console.WriteLine($"Received person - First Name: {person.FirstName}, Last Name: {person.LastName}");
                                Console.WriteLine($"Message ID: {message.MessageId}");
                                Console.WriteLine($"Enqueued Time: {message.EnqueuedTime}");
                                Console.WriteLine("------------------------");
                            }
                            else
                            {
                                throw new Exception("Failed to deserialize Person object - result was null");
                            }

                            await ProcessPersonAsync(person);
                            await receiver.CompleteMessageAsync(message);
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
                Console.WriteLine("Receiver stopped.");
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
                // Add any additional processing logic for the Person object here
                if (string.IsNullOrEmpty(person.FirstName) || string.IsNullOrEmpty(person.LastName))
                {
                    throw new Exception("Person has missing required fields");
                }
            });
        }
    }
}