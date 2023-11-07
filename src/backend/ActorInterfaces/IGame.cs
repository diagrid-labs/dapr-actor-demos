using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace ActorInterfaces
{
    public interface IGame : IActor
    {
        Task StartWave();
    }
}