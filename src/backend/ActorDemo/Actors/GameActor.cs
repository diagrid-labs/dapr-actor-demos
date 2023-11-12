using ActorInterfaces;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Actors.Runtime;

namespace ActorDemo
{
    public class GameActor : Actor, IGame
    {
        public GameActor(ActorHost host) : base(host)
        {
        }
        public GameData GameData { get; set; }
        public string[] ZombieIds { get; set; }
        public string[] HeroIds { get; set; }

        public async Task StartNewGame(GameData gameData)
        {
            GameData = gameData;
            var heroActorId = new ActorId(gameData.HeroName);
            var heroProxy = ActorProxy.Create<IHero>(heroActorId, nameof(HeroActor));

            var positionsActorId = new ActorId(Id.GetId());
            var positionsProxy = ActorProxy.Create<IPositions>(positionsActorId, nameof(PositionsActor));

            for (int i = 0; i < 10; i++)
            {
                var zombieActorId =new ActorId($"{Id.GetId()}-{i}");
                var zombieProxy = ActorProxy.Create<IZombie>(zombieActorId, nameof(ZombieActor));
                await zombieProxy.SetRandomPosition(new Coordinate(GameData.AreaSize, GameData.AreaSize));
                await positionsProxy.AddZombie(zombieProxy);
            }
        }

        public async Task NextWave()
        {
            GameData = new GameData(GameData.HeroName, GameData.AreaSize, GameData.Wave + 1);
        }

        public async Task<GameData> GetGameData()
        {
            return await StateManager.GetStateAsync<GameData>("GameData");
        }
    }
}