using Dapr.Actors;

namespace ActorInterfaces
{
    public interface IGame : IActor
    {
        Task StartNewGame(GameData data);
        Task<GameData> GetGameData();
        Task NextWave();

    }
}