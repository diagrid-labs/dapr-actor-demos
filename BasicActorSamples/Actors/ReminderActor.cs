using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace BasicActorSamples.Actors
{
    public interface IReminderActor : IActor
    {
        Task ResetSnoozeCount();
    }
    
    public class ReminderActor : Actor, IReminderActor, IRemindable
    {
        private const string SNOOZE_COUNT_KEY = "snoozecount";
        private const string REMINDER_NAME = "snooze";
        
        public ReminderActor(ActorHost host) : base(host)
        {
        }

        public async Task ResetSnoozeCount()
        {
            await StateManager.SetStateAsync(SNOOZE_COUNT_KEY, 0);
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == REMINDER_NAME)
            {
                Console.WriteLine($"{REMINDER_NAME} received from reminder!");
                int count = 1;
                var conditionalValue = await StateManager.TryGetStateAsync<int>(SNOOZE_COUNT_KEY);
                if (conditionalValue.HasValue)
                {
                    count = conditionalValue.Value + 1;
                }
                await StateManager.SetStateAsync(SNOOZE_COUNT_KEY, count);
            }
        }
    }
}