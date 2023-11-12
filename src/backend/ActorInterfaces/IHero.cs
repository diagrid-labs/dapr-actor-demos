using Dapr.Actors;

namespace ActorInterfaces
{
    public interface IHero : IActor
    {
        string Name { get; set; }
        
        Coordinate Position { get; set; }

        Task Run(double x, double y);

        Task Attack();

        Task TakeDamage(int damage);
    }
}