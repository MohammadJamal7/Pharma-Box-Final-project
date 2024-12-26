using Microsoft.AspNetCore.SignalR;

namespace Graduation_Project.Hubs
{
    public class ChatHubs:Hub
    {
        public async Task SendMessage(string userId, string message)
        {
            // Ensure this matches the client-side method name
            await Clients.User(userId).SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
        }

        // Handle user connection
        public override async Task OnConnectedAsync()
        {
            string userId = Context.User.Identity.Name; // Replace with your user identification logic
            await Groups.AddToGroupAsync(Context.ConnectionId, userId); // Associate the connection with the user
            await base.OnConnectedAsync();
        }


        // Handle user disconnection
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userId = Context.User.Identity.Name; // Replace with your user identification logic
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId); // Remove the user from the group
            await base.OnDisconnectedAsync(exception);
        }
    }
}
