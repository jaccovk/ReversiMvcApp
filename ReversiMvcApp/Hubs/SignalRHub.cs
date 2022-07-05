using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ReversiMvcApp.Hubs
{
    public class SignalRHub: Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
