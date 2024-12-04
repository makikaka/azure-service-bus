using Azure.Messaging.ServiceBus;
using System.Text.Json;


namespace SBQuePublisher.Services
{
    public class QueService : IQueService
    {
        private readonly IConfiguration _configuration;

        public QueService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMessageAsync<T>(T serviceBusMessage, string queName)
        {
            try
            {
                ServiceBusClient client = new ServiceBusClient(_configuration.GetConnectionString("AzureServiceBus"));
                ServiceBusSender sender = client.CreateSender(queName);

                ServiceBusMessage message = new ServiceBusMessage(BinaryData.FromObjectAsJson(serviceBusMessage));

                await sender.SendMessageAsync(message);
            }
            catch
            {

            }
        }
    }
}
