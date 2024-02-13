using Microsoft.AspNetCore.SignalR;

namespace Extension.Hubs
{
    public class ExtensionHub : Hub
    {
        public async Task SendMessage(string id, string message)
        {
            await Clients.Group(id).SendAsync("ReceiveMessage", id, message);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Caller.SendAsync("JoinedGroup", groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
