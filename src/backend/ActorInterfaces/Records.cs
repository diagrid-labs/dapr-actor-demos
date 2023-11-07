using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ActorInterfaces
{
    public record Coordinate(int X, int Y);
    public record GameData(string Name, int AreaSize);
}