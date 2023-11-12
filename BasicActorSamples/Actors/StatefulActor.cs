using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace BasicActorSamples.Actors
{
    public interface IStatefulActor : IActor
    {
        Task SetGreeting(string greeting);
        Task<string> GetGreeting();
    }
    
    public class StatefulActor : Actor, IStatefulActor
    {
        private const string GREETING_KEY = "greeting";
        
        public StatefulActor(ActorHost host) : base(host)
        {
        }

        public async Task SetGreeting(string greeting)
        {
           await StateManager.SetStateAsync(GREETING_KEY, greeting);
        }

        public async Task<string> GetGreeting()
        {
            return await StateManager.GetStateAsync<string>(GREETING_KEY);
        }
    }
}