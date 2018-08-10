using BoilerController.Api.Contracts;
using BoilerController.Api.Models;
using System.Collections.Generic;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;

namespace BoilerController.Api.Services
{
    public class DeviceService:IDeviceService
    {
        private readonly List<Device> _devices;
        public DeviceService(IRepositoryWrapper repositoryWrapper)
        {
            _devices = new List<Device>(repositoryWrapper.Devices.GetAllDevices());
        }

        /// <summary>
        /// Gets the state for the given device
        /// </summary>
        /// <param name="device">Device object</param>
        public bool GetState(Device device)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sets the desired state for the given device
        /// </summary>
        /// <param name="device">Device object</param>
        /// <param name="state">Boolean state value</param>
        public void SetState(Device device, bool state)
        {
            if (device.Type != DeviceType.Output) return;

            var pin = Pi.Gpio[device.DevicePin];
            pin.PinMode = GpioPinDriveMode.Output;
            pin.Write(state);
        }
    }
}
