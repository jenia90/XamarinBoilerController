using System;
using BoilerController.Api.Contracts;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace BoilerController.Api.Models.Devices
{
    public class InputDevice : Device
    {
#if DEBUG
        private bool _state = true;
#else
        private GpioPin _devicePin;
#endif

        public Guid Id { get; set; }
        public DeviceType Type => DeviceType.Input;
#if DEBUG

        public int DevicePin { get; set; }
#else
        public int DevicePin
        {
            get => _devicePin.PinNumber;
            set =>_devicePin = Pi.Gpio[value];
        }
#endif

        public bool State
        {
#if DEBUG
            get => _state;
#else
            get => _devicePin?.Read() ?? throw new NotImplementedException("You have to assing pin first!");
#endif
            set { }
        }
    }
}
