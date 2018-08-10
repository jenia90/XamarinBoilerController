using System;

namespace BoilerController.Api.Models
{
    public class State
    {
        public Guid DeviceId { get; set; }
        public bool DeviceState { get; set; }
    }
}
