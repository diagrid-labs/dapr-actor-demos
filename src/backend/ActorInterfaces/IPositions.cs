using Dapr.Actors;

namespace ActorInterfaces
{
    public interface IPositions : IActor
    {
        Task<IZombie> GetClosestZombie();
        Task UpdateZombiePosition(IZombie zombie);
        Task AddZombie(IZombie zombie);
        Task RemoveZombie(IZombie zombie);
    }
}