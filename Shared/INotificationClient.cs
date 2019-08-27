namespace Shared
{
    using System.Threading.Tasks;

    public interface INotificationClient
    {
        void Register();
        void SendMessage(string destination, string caption, string message);
        Task SendMessageAsync(string destination, string caption, string message);
        void SendMessageToManyDestinations(string[] destinations, string caption, string message);
        Task SendMessageToManyDestinationsAsync(string[] destinations, string caption, string message);
        bool IsRegistered { get; }
    }
}
