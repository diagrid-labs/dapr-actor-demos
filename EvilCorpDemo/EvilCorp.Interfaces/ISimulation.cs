using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface ISimulation : IActor
    {
        Task InitActorsAsync(SimulationData data);
        Task StartTimeAsync();
    }

    public class SimulationData
    {
        public int EmployeeCount { get; set; }
    }
}