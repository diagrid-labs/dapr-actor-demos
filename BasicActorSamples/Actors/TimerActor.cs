using System.Text;
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace BasicActorSamples.Actors
{
    public interface ITimerActor : IActor
    {
        Task CreateTimer();
        Task ResetSnoozeCount();
    }
    
    public class TimerActor : Actor, ITimerActor
    {
        private const string SNOOZE_COUNT_KEY = "snoozecount";
        private const string TIMER_NAME = "snooze";
        
        public TimerActor(ActorHost host) : base(host)
        {
        }

        public async Task CreateTimer()
        {
            await RegisterTimerAsync(
                TIMER_NAME,
                nameof(SnoozeHandler),
                Encoding.UTF8.GetBytes("Wake up!"),
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(5));
        }

        public async Task SnoozeHandler(byte[] data)
        {
            Console.WriteLine($"{TIMER_NAME} received from timer!");

            int count = 1;
            var conditionalValue = await StateManager.TryGetStateAsync<int>(SNOOZE_COUNT_KEY);
            if (conditionalValue.HasValue)
            {
                count = conditionalValue.Value + 1;
            }
            await StateManager.SetStateAsync(SNOOZE_COUNT_KEY, count);
            Console.WriteLine($"{nameof(TimerActor)}-{Id} {Encoding.UTF8.GetString(data)} {count}");
        }
        public async Task ResetSnoozeCount()
        {
            await StateManager.SetStateAsync(SNOOZE_COUNT_KEY, 0);
        }
    }
}