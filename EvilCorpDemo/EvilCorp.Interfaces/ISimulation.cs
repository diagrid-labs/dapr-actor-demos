using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface ISimulation : IActor
    {
        Task InitActorsAsync();
        Task StartTimeAsync();
    }
}