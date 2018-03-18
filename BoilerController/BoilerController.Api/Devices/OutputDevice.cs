using System;
using Unosquare.RaspberryIO.Gpio;

namespace BoilerController.Api.Devices
{
    public class OutputDevice : IDevice
    {
        private bool _state;
        public Guid Id { get; set; }
        public DeviceType Type => DeviceType.Output;

        public string DeviceName { get; set; }
        public GpioPin DevicePin { get; set; }

        public bool State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;
                DevicePin.Write(value);
            }
        }
    }
}
