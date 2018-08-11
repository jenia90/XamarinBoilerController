using System;
using System.Collections.Generic;

namespace BoilerController.Common.Models
{
    public enum DeviceType
    {
        Input = 0,
        Output = 1,
        PWM = 2
    }
    public class Device
    {
        public Guid Id { get; set; }
        public DeviceType Type { get; set; }
        public string DeviceName { get; set; }
        public int DevicePin { get; set; }
        public bool State { get; set; }
        public IEnumerable<Job> Jobs { get; set; }
    }
}
