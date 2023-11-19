using Dapr.Actors;
using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class SimulationActor : Actor, ISimulation
    {
        public SimulationActor(ActorHost host) : base(host)
        {
        }

        public Task InitActors()
        {
            throw new NotImplementedException();
        }
    }
}