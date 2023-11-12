using Dapr.Actors;

namespace ActorInterfaces
{
    public interface IZombie : IActor
    {
        string Name { get; set; }
        Coordinate Position { get; set; }
        public double DistanceToHero { get; set; }

        Task SetRandomPosition(Coordinate position);

        // Find Hero and move towards them.
        Task Walk();

        // Attack the Hero if they are in range.
        Task Attack();

        // Reduce the health and if health <=0, remove it from the list of zombies.
        Task TakeDamage(int damage);
    }
}