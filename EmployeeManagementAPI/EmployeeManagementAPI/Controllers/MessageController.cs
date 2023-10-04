using EmployeeMangementApi.Hub.Hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EmployeeManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IHubContext<NotificationHub, INotificationHubClient> _hubContext;
        public MessageController(IHubContext<NotificationHub, INotificationHubClient> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        [Route("SendMessage")]
        public async Task<IActionResult> SendMessage(string message)
        {
            await _hubContext.Clients.All.SendNotificationToUser(message);
            return Ok("Notification send Succesfully");
        }
    }
}
