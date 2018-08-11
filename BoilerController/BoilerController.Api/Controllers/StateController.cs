using System;
using BoilerController.Api.Contracts;
using BoilerController.Api.Extensions;
using BoilerController.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace BoilerController.Api.Controllers
{
    [Route("api/state")]
    public class StateController : Controller
    {
        private readonly ILoggerManager _logger;
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly IDeviceService _devService;

        public StateController(ILoggerManager logger, 
                               IRepositoryWrapper repoWrapper,
                               IDeviceService devSerice)
        {
            _logger = logger;
            _repoWrapper = repoWrapper;
            _devService = devSerice;
        }
        //TODO: Clean this place up and maybe move the error checking and logging errors to DeviceService.
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
                return Ok(_devService.GetState(device));
            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong inside GetCurrentState action: {e.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public IActionResult SetState([FromBody] State state)
        {
            try
            {
                if (state == null)
                {
                    _logger.LogError("State object sent from client is null.");
                    return BadRequest("State object is null.");
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError($"State object sent from client is invalid.");
                    return BadRequest($"State object is invalid.");
                }

                var device = _repoWrapper.Devices.GetDeviceById(state.DeviceId);

                if (device.IsEmptyObject())
                {
                    _logger.LogError($"Device with ID: {state.DeviceId} couldn't be found.");
                    return NotFound("Device with such ID couldn't be found.");
                }

                _devService.SetState(device, state.DeviceState);
                return NoContent();

            }
            catch (Exception e)
            {
                _logger.LogError($"Something went wrong inside GetCurrentState action: {e.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}