
namespace SBSender.Services
{
    public interface IQueService
    {
        Task SendMessageAsync<T>(T serviceBusMessage, string queName);
    }
}