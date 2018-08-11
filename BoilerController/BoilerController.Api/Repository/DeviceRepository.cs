using System;
using System.Collections.Generic;
using System.Linq;
using BoilerController.Api.Contracts;
using BoilerController.Api.Extensions;
using BoilerController.Api.Models;

namespace BoilerController.Api.Repository
{
    public class DeviceRepository : RepositoryBase<Device>, IDeviceRepository
    {
        public DeviceRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<Device> GetAllDevices()
        {
            return FindAll().OrderBy(d => d.DeviceName);
        }

        public Device GetDeviceById(Guid deviceId)
        {
            return FindByCondition(d => d.Id.Equals(deviceId))
                .DefaultIfEmpty(null)
                .FirstOrDefault();
        }

        public void CreateDevice(Device device)
        {
            Create(device);
            Save();
        }

        public void UpdateDevice(Device dbDevice, Device device)
        {
            dbDevice.Map(device);
            Update(dbDevice);
            Save();
        }

        public void DeleteDevice(Device device)
        {
            Delete(device);
            Save();
        }
    }
}
