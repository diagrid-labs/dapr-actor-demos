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