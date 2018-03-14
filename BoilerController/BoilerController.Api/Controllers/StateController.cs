using Microsoft.AspNetCore.Mvc;

namespace BoilerController.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/state")]
    public class StateController : Controller
    {
        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok("WORKING!");
        }
    }
}