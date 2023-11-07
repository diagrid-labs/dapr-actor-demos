using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActorInterfaces;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

namespace ActorDemo
{
    public class GameActor : Actor, IGame
    {
        public GameActor(ActorHost host) : base(host)
        {
        }

        public string Name { get; set; }
        public int Wave { get; set; }
        public string[] ZombieIds { get; set; }
        public string[] HeroIds { get; set; }

        public async Task StartNewGame(GameData data)
        {
            Name = data.Name;
            Wave = 0;

            for (int i = 0; i < 10; i++)
            {
                var actorId =new ActorId($"{Name}-{i}");
                var zombieProxy = ActorProxy.Create<IZombie>(actorId, nameof(ZombieActor));
                await zombieProxy.SetNewPosition(new Coordinate(data.AreaSize, data.AreaSize));
            }
        }

        public async Task NextWave()
        {
            throw new NotImplementedException();
        }
    }
}