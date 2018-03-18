using System;
using BoilerController.Api.Contracts;
using BoilerController.Api.Devices;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace BoilerController.Api.Services
{
    /// <summary>
    /// Manages the Raspberry Pi 3 GPIO pin using the Unosquare library.
    /// Docs here: https://github.com/unosquare/raspberryio
    /// </summary>
    public static class DeviceFactory
    {
        /// <summary>
        ///  Adds a new device to the collection
        /// </summary>
        /// <param name="name">Human friendly name of the device</param>
        /// <param name="pin">Pin the device is connected to</param>
        /// <param name="type">Input/Output/PWM type</param>
        /// <returns>A newly created device object if there's no device connected to specified pin.
        /// Otherwise an existing device will be returned.</returns>
        public static IDevice GetNewDevice(string name, int pin, DeviceType type)
        {
            IDevice device;
            switch (type)
            {
                case DeviceType.Input:
                    device = new OutputDevice { DeviceName = name, DevicePin = Pi.Gpio.Pins[pin] };
                    device.DevicePin.PinMode = GpioPinDriveMode.Input;
                    return device;
                case DeviceType.Output:
                    device = new OutputDevice {DeviceName = name, DevicePin = Pi.Gpio.Pins[pin]};
                    device.DevicePin.PinMode = GpioPinDriveMode.Output;
                    return device;
                case DeviceType.PWM:
                    return null; // TODO: Add Pwm device
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
