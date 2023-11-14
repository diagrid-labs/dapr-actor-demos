using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace BasicActorSamples.Actors
{
    public interface IActorToActor : IActor
    {
        Task<string> CallHelloWorld(string helloWorldActorId);
    }
    
    public class ActorToActor : Actor, IActorToActor
    {
        public ActorToActor(ActorHost host) : base(host)
        {
        }

        public async Task<string> CallHelloWorld(string helloWorldActorId)
        {
            var helloWorldProxy = ProxyFactory.CreateActorProxy<IHelloWorld>(
            new ActorId(helloWorldActorId),
            nameof(HelloWorldActor));

            var result = await helloWorldProxy.SayHelloWorld();
            return result;
        }
    }
}