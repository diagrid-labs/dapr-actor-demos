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

        public async Task<IZombie> GetClosestZombie()
        {
            return await Task.FromResult(
                Zombies.OrderBy(z => z.DistanceToHero).First());
        }

        public static double GetEuclidianDistance(Coordinate position1, Coordinate position2)
        {
            return Math.Sqrt((position1.X - position2.X) * (position1.X - position2.X) + (position1.Y - position2.Y) * (position1.Y - position2.Y));
        }

        public Task UpdateZombiePosition(IZombie zombie)
        {
            RemoveZombie(zombie);
            AddZombie(zombie);

            return Task.CompletedTask;
        }

        public Task AddZombie(IZombie zombie)
        {
            Zombies.Add(zombie);

            return Task.CompletedTask;
        }

        public Task RemoveZombie(IZombie zombie)
        {
            Zombies.Remove(Zombies.First(z => z.Name == zombie.Name));

            return Task.CompletedTask;
        }

    }
}