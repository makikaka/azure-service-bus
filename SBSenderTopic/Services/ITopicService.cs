namespace SBSenderTopic.Services
{
    public interface ITopicService
    {
        Task SendMessageAsync<T>(T serviceBusMessage, string topicName, string? filter);
    }
}