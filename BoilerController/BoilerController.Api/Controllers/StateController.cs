using System;
using BoilerController.Api.Contracts;
using BoilerController.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BoilerController.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/state")]
    public class StateController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repoWrapper;

        public StateController(ILoggerManager logger, IRepositoryWrapper repoWrapper)
        {
            _logger = logger;
            _repoWrapper = repoWrapper;
        }

        [HttpGet("pin/{id}")]
        public IActionResult GetStateByPin(int pin)
        {
            try
            {
                var device = _repoWrapper.Devices.GetDeviceByPin(pin);
                if (device.IsEmptyObject())
                {
                    _logger.LogError($"Device at pin: {pin} couldn't be found.");
                    return NotFound("Device connected to that pin couldn't be found.");
                }

                _logger.LogInfo($"Current state for device at: {pin}, returned.");
                return Ok(device);
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong inside GetCurrentState action: {e.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("id/{id}")]
        public IActionResult GetStateById(Guid id)
        {
            try
            {
                var device = _repoWrapper.Devices.GetDeviceById(id);
                if (device.IsEmptyObject())
                {
                    _logger.LogError($"Device with ID: {id} couldn't be found.");
                    return NotFound("Device with such ID couldn't be found.");
                }

                _logger.LogInfo($"Current state for device: {id}, returned.");
                return Ok(device.State);
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong inside GetCurrentState action: {e.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}