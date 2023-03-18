using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
	[HubName("GeneralHub")]
	public class GeneralHub : Hub
	{
		//just for tests
		public async Task SendMessage(string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", message);
		}
	}
}
