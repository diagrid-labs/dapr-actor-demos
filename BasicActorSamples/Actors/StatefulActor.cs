using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
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
        public StatefulActor(ActorHost host) : base(host)
        {
        }

        public async Task SetGreeting(string greeting)
        {
           await StateManager.SetStateAsync("greeting", greeting);
        }

        public async Task<string> GetGreeting()
        {
            Console.WriteLine($"Getting greeting for {this.Id.ToString()}");
            return await StateManager.GetStateAsync<string>("greeting");
        }
    }
}