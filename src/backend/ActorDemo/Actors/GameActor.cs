using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActorInterfaces;
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

        public async Task StartWave()
        {

        }
    }
}