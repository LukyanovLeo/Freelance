using System.Threading.Tasks;

namespace Freelance.Hubs.Interfaces
{
    public interface IMessageHub
    {
        Task ReceiveMessage(string message, string userEmail);
    }
}
