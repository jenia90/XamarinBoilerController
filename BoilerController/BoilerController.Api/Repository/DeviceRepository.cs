using System;
using System.Collections.Generic;
using System.Linq;
using BoilerController.Api.Contracts;
using BoilerController.Api.Devices;
using BoilerController.Api.Extensions;
using BoilerController.Api.Models;

namespace BoilerController.Api.Repository
{
    public class DeviceRepository : RepositoryBase<IDevice>, IDeviceRepository
    {
        public DeviceRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<IDevice> GetAllDevices()
        {
            return FindAll().OrderBy(d => d.DeviceName);
        }

        public IDevice GetDeviceById(Guid deviceId)
        {
            return FindByCondition(d => d.Id.Equals(deviceId))
                .DefaultIfEmpty(null)
                .FirstOrDefault();
        }

        public IDevice GetDeviceByPin(int pin)
        {
            return FindByCondition(d => d.DevicePin.PinNumber.Equals(pin))
                .DefaultIfEmpty(null)
                .FirstOrDefault();
        }

        public void CreateDevice(IDevice device)
        {
            Create(device);
            Save();
        }

        public void UpdateDevice(IDevice dbDevice, IDevice device)
        {
            dbDevice.Map(device);
            Update(dbDevice);
            Save();
        }

        public void DeleteDevice(IDevice device)
        {
            Delete(device);
            Save();
        }
    }
}
