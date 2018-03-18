using System;
using Unosquare.RaspberryIO.Gpio;

namespace BoilerController.Api.Devices
{
    public class InputDevice : IDevice
    {
        private bool _state;
        public Guid Id { get; set; }
        public DeviceType Type => DeviceType.Input;
        public string DeviceName { get; set; }
        public GpioPin DevicePin { get; set; }

        public bool State
        {
            get => DevicePin.ReadValue() == GpioPinValue.High;
            set { }
        }
    }
}
