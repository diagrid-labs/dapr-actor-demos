using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace ActorInterfaces
{
    public interface IHero : IActor
    {
        Coordinate Position { get; set; }

        Task Run(int x, int y);

        Task Attack();

        Task TakeDamage(int damage);
    }
}