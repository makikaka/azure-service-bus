namespace SBTopicPublisher.Services
{
    public interface ITopicService
    {
        Task SendMessageAsync<T>(T serviceBusMessage, string topicName, string? filter);
    }
}