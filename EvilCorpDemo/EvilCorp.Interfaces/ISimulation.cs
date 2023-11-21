using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface ISimulation : IActor
    {
        Task InitActorsAsync(int employeeCount);
        Task StartTimeAsync();
    }
}