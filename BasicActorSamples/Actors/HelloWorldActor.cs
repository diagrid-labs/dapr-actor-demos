using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace BasicActorSamples.Actors
{
    public interface IHelloWorld : IActor
    {
        Task<string> SayHelloWorld();
        Task<string> SayHello(string name);
    }
    
    public class HelloWorldActor : Actor, IHelloWorld
    {
        public HelloWorldActor(ActorHost host) : base(host)
        {
        }

        public Task<string> SayHelloWorld()
        {
            return Task.FromResult("Hello World!");
        }

        public Task<string> SayHello(string name)
        {
            return Task.FromResult($"Hello {name}!");
        }
    }
}