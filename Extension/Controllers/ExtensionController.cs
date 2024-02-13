using Extension.Hubs;
using Extension.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Extension.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ExtensionController : ControllerBase
    {
        private readonly IHubContext<ExtensionHub> _hubContext;

        public ExtensionController(IHubContext<ExtensionHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] Contact oContact)
        {
            await _hubContext
                .Clients
                .Group(oContact.IdGroup)
                .SendAsync("ReceiveMessage",
                    oContact.Name,
                    JsonConvert.SerializeObject(oContact));
            return Ok("Ok");
        }

    }
}
