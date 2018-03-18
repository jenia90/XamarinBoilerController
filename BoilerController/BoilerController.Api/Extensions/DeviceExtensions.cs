using System;
using BoilerController.Api.Contracts;
using BoilerController.Api.Devices;

namespace BoilerController.Api.Extensions
{
    public static class DeviceExtensions
    {
        /// <summary>
        /// Returns true if object is null
        /// </summary>
        /// <param name="device">IDevice object</param>
        /// <returns>True if null; false otherwise</returns>
        public static bool IsObjectNull(this IDevice device)
        {
            return device == null;
        }

        /// <summary>
        /// Returns true if object is uninitialized
        /// </summary>
        /// <param name="device">IDevice object</param>
        /// <returns>true if uninitialized; false otherwise</returns>
        public static bool IsEmptyObject(this IDevice device)
        {
            return device.Id == Guid.Empty;
        }
    }
}
