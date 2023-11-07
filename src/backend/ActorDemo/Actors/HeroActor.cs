using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using ActorInterfaces;

namespace ActorDemo
{
    public class HeroActor : Actor, IHero
    {
        public HeroActor(ActorHost host) : base(host)
        {
        }
        
        public string Name { get; set; }
        public int Health { get; set; }
        public Coordinate Position { get; set; }
        private const int Speed = 2;
        private const int Damage = 10;
        private const int AttackDistance = 1;

        public async Task Run(int x, int y)
        {
            throw new NotImplementedException();
        }

        public async Task Attack()
        {
            // Check if closest zombie is in range.
            // If yes attack zombie with fixed damage.
        }

        public async Task TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                // Call end game, Hero loses
            }
        }
    }
}