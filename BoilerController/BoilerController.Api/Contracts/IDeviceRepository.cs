using System;
using System.Collections.Generic;
using BoilerController.Api.Devices;

namespace BoilerController.Api.Contracts
{
    public interface IDeviceRepository : IRepositoryBase<IDevice>
    {
        IEnumerable<IDevice> GetAllDevices();
        IDevice GetDeviceById(Guid deviceId);
        IDevice GetDeviceByPin(int pin);
        void CreateDevice(IDevice device);
        void UpdateDevice(IDevice dbDevice, IDevice device);
        void DeleteDevice(IDevice device);
    }
}
