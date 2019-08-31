namespace Shared
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INotificationClient
    {
        void SendMessage(string destination, string caption, string message);
        Task SendMessageAsync(string destination, string caption, string message);
        void SendMessageToManyDestinations(IEnumerable<string> destinations, string caption, string message);
        Task SendMessageToManyDestinationsAsync(IEnumerable<string> destinations, string caption, string message);
    }
}
