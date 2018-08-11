using BoilerController.Api.Contracts;
using BoilerController.Api.Extensions;
using BoilerController.Api.Models;
using System;
using System.Collections.Generic;
#if NDEBUG
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Gpio;
#endif

namespace BoilerController.Api.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly List<Device> _devices;
        private readonly IRepositoryWrapper _repoWrapper;

        public DeviceService(IRepositoryWrapper repositoryWrapper)
        {
            _devices = new List<Device>(repositoryWrapper.Devices.GetAllDevices());
            _repoWrapper = repositoryWrapper;
        }

        /// <summary>
        /// Gets the state for the given device
        /// </summary>
        /// <param name="device">Device object</param>
        public bool GetState(Device device)
        {
#if DEBUG
            return device.State;
#else
            return (device.Type == DeviceType.Input) ? 
                Pi.Gpio[device.DevicePin].Read() : device.State;
#endif
        }

        /// <summary>
        /// Returns the state of a device by its GUID.
        /// </summary>
        /// <param name="id">Guid of the device.</param>
        /// <returns>Boolean value representing the current state.</returns>
        public bool GetStateById(Guid id)
        {
            var device = _devices.Find(d => d.Id == id);
            return GetState(device);
        }

        /// <summary>
        /// Sets the desired state for the given device
        /// </summary>
        /// <param name="device">Device object</param>
        /// <param name="state">Boolean state value</param>
        public void SetState(Device device, bool state)
        {
            if (device.Type != DeviceType.Output) return;
#if DEBUG
            device.State = state;
            UpdateDevice(device.Id, _repoWrapper.Devices.GetDeviceById(device.Id), device);
#else
            var pin = Pi.Gpio[device.DevicePin];
            pin.PinMode = GpioPinDriveMode.Output;
            pin.Write(state);
#endif
        }

        /// <summary>
        /// Set state for a device by its GUID.
        /// </summary>
        /// <param name="id">Guid of the device</param>
        /// <param name="state">Boolean state to set.</param>
        public void SetStateById(Guid id, bool state)
        {
            var device = _devices.Find(d => d.Id == id);
            SetState(device, state);
        }

        /// <summary>
        /// Adds device configuration to the database and to the list of managed devices.
        /// </summary>
        /// <param name="device">Device object to register.</param>
        /// <returns>Guid of the new device.</returns>
        public Guid RegisterDevice(Device device)
        {
            var id = device.Id = Guid.NewGuid();
            _repoWrapper.Devices.CreateDevice(device);
            _devices.Add(device);
            return id;
        }

        public void UpdateDevice(Guid id, Device dbDevice, Device device)
        {
            var dev = _devices.Find(d => d.Id == id);
            dev.Map(device);
            _repoWrapper.Devices.UpdateDevice(dbDevice, device);
        }

        /// <summary>
        /// Removes device configuration.
        /// </summary>
        /// <param name="device">Device object to remove.</param>
        public void DeregisterDevice(Device device)
        {
            _devices.Remove(device);
            _repoWrapper.Devices.DeleteDevice(device);
        }
    }
}
