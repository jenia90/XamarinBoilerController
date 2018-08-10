using System;
using System.Collections.Generic;

namespace BoilerController.Common.Models
{
    public enum DeviceType
    {
        Input,
        Output,
        PWM
    }
    public class Device
    {
        public Guid Id { get; set; }
        public DeviceType Type { get; }
        public string DeviceName { get; set; }
        public int DevicePin { get; set; }
        public bool State { get; set; }
        public IEnumerable<Job> Jobs { get; set; }
    }
}
