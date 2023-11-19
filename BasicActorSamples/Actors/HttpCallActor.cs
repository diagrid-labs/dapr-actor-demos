using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace BasicActorSamples.Actors
{
    public interface IHttpCallActor : IActor
    {
        Task<string> CallEndpoint(string id);
    }
    
    public class HttpCallActor : Actor, IHttpCallActor
    {
        public HttpCallActor(ActorHost host) : base(host)
        {
        }

        public async Task<string> CallEndpoint(string helloWorldActorId)
        {
            var helloWorldProxy = ProxyFactory.CreateActorProxy<IHelloWorld>(
                new ActorId(helloWorldActorId),
                nameof(HelloWorldActor));

            var result = await helloWorldProxy.SayHelloWorld();
            return result;
        }
    }
}