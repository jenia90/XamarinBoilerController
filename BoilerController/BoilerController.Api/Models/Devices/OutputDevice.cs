using System;
using BoilerController.Api.Contracts;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace BoilerController.Api.Models.Devices
{
    public class OutputDevice : Device
    {
        private bool _state;
#if DEBUG
        private int _devicePin;
#else
        private GpioPin _devicePin;
#endif

        public Guid Id { get; set; }
        public DeviceType Type => DeviceType.Output;

        public string DeviceName { get; set; }
#if DEBUG
        public int DevicePin { get; set; }
#else
        public int DevicePin
        {
            get => _devicePin.PinNumber;
            set => _devicePin = Pi.Gpio[value];
        }
#endif

        public bool State
        {
            get => _state;
#if DEBUG
            set => _state = value;
#else
            set
            {
                if (_state == value || _devicePin == null) return;

                _devicePin.Write(value);
                _state = value;
            }
#endif
        }
    }
}
