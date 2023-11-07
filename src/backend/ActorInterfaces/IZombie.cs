using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace ActorInterfaces
{
    public interface IZombie : IActor
    {
        Coordinate Position { get; set; }

        // Find Hero and move towards them.
        Task Walk();

        // Attack the Hero if they are in range.
        Task Attack();

        // Reduce the health and if health <=0, remove it from the list of zombies.
        Task TakeDamage(int damage);
    }
}