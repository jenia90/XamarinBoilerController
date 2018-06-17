using System;
using System.Collections.Generic;
using BoilerController.Api.Models.Devices;

namespace BoilerController.Api.Contracts
{
    public interface IDeviceRepository : IRepositoryBase<Device>
    {
        IEnumerable<Device> GetAllDevices();
        Device GetDeviceById(Guid deviceId);
        void CreateDevice(Device device);
        void UpdateDevice(Device dbDevice, Device device);
        void DeleteDevice(Device device);
    }
}
