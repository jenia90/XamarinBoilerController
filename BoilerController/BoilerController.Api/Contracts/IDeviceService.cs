using BoilerController.Api.Models;

namespace BoilerController.Api.Contracts
{
    public interface IDeviceService
    {
        void SetState(Device device, bool state);
        bool GetState(Device device);
    }
}
