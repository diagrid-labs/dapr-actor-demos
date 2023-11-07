using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActorInterfaces;
using Dapr.Actors.Runtime;

namespace ActorDemo
{
    public class PositionsActor : Actor, IPositions
    {
        public PositionsActor(ActorHost host) : base(host)
        {
        }

        public List<IZombie> Zombies { get; set; }
        public List<IHero> Heroes {get ; set; }
        public async Task<IList<IZombie>> GetClosestZombies()
        {
            throw new NotImplementedException();
        }

        public static double GetEuclidianDistance(Coordinate position1, Coordinate position2)
        {
            return Math.Sqrt((position1.X - position2.X) * (position1.X - position2.X) + (position1.Y - position2.Y) * (position1.Y - position2.Y));
        }
    }
}