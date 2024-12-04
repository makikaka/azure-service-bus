
namespace SBQuePublisher.Services
{
    public interface IQueService
    {
        Task SendMessageAsync<T>(T serviceBusMessage, string queName);
    }
}