using System;
using BoilerController.Api.Contracts;
using BoilerController.Api.Extensions;
using BoilerController.Api.Models.Devices;

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
        /// <param name="modelDevice"></param>
        /// <returns>A newly created device object if there's no device connected to specified pin.
        /// Otherwise an existing device will be returned.</returns>
        public static Device GetNewDevice(Device modelDevice)
        {
            Device dev;
            switch (modelDevice.Type)
            {
                case DeviceType.Input:
                    dev = new InputDevice();
                    break;
                case DeviceType.Output:
                    dev = new OutputDevice();
                    break;
                case DeviceType.PWM:
                    return null; // TODO: Add Pwm device
                default:
                    throw new ArgumentOutOfRangeException(nameof(modelDevice.Type), modelDevice.Type, null);
            }

            dev.Map(modelDevice);
            return dev;
        }
    }
}
