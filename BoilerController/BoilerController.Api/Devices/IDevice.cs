using System;
using Unosquare.RaspberryIO.Gpio;

namespace BoilerController.Api.Devices
{
    public enum DeviceType
    {
        Input,
        Output,
        PWM
    }
    public interface IDevice
    {
        /// <summary>
        /// Gets or sets device ID.
        /// </summary>
        Guid Id { get; set; }
        /// <summary>
        /// Gets device type.
        /// </summary>
        DeviceType Type { get; }
        /// <summary>
        /// Gets or sets device name.
        /// </summary>
        string DeviceName { get; set; }
        /// <summary>
        /// Gets or sets device pin.
        /// </summary>
        GpioPin DevicePin { get; set; }
        /// <summary>
        /// Gets or sets device state.
        /// </summary>
        bool State { get; set; }
    }
}
