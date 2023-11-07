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
        private const int Speed = 1;
        private const int Damage = 2;

        public async Task SetNewPosition(Coordinate position)
        {
            var random = new Random();
            Position = new Coordinate(random.Next(0, position.X), random.Next(0, position.Y));
        }

        public async Task Walk()
        {

        }

        public async Task Attack()
        {

        }

        public async Task TakeDamage(int damage)
        {

        }
    }
}