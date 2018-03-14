namespace BoilerController.Api.Contracts
{
    public interface IRepositoryWrapper
    {
        IJobRepository Job { get; set; }
    }
}
