using Azure.Messaging.ServiceBus;

namespace SBTopicPublisher.Services
{
    public class TopicService : ITopicService
    {
        private readonly IConfiguration _configuration;

        public TopicService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMessageAsync<T>(T serviceBusMessage, string topicName, string? filter = null)
        {
            try
            {
                ServiceBusClient client = new ServiceBusClient(_configuration.GetConnectionString("AzureServiceBus"));
                ServiceBusSender sender = client.CreateSender(topicName);

                // Create the message
                var message = new ServiceBusMessage(BinaryData.FromObjectAsJson(serviceBusMessage));

                // Add custom properties if filter is provided
                if (!string.IsNullOrEmpty(filter))
                {
                    message.ApplicationProperties.Add("MessageType", filter);
                }

                await sender.SendMessageAsync(message);
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
            catch (Exception ex)
            {
                // Add proper logging here
                throw new Exception($"Failed to send message to topic: {ex.Message}", ex);
            }
        }
    }
}
