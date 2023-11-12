using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using ActorInterfaces;

namespace ActorDemo
{
    public class ZombieActor : Actor, IZombie
    {
        public ZombieActor(ActorHost host) : base(host)
        {
        }
        
        public string Name { get; set; }
        public int Health { get; set; }
        public Coordinate Position { get; set; }
        public double DistanceToHero { get; set; }
        private const int Speed = 1;
        private const int Damage = 2;

        public async Task SetRandomPosition(Coordinate position)
        {
            var random = new Random();
            Position = new Coordinate(
                random.Next(0, Convert.ToInt32(position.X)),
                random.Next(0, Convert.ToInt32(position.Y)));

             await Task.CompletedTask;
        }

        public async Task Walk()
        {
            throw new NotImplementedException();
        }

        public async Task Attack()
        {
            throw new NotImplementedException();
        }

        public async Task TakeDamage(int damage)
        {
            throw new NotImplementedException();
        }
    }
}