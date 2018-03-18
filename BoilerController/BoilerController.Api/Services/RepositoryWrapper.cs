using BoilerController.Api.Contracts;
using BoilerController.Api.Repository;

namespace BoilerController.Api.Services
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        public IJobRepository Job { get; set; }
        public IDeviceRepository Devices { get; set; }

        public RepositoryWrapper(RepositoryContext repositoryContext)
        {
            Job = new JobRepository(repositoryContext);
            Devices = new DeviceRepository(repositoryContext);
        }
    }
}
