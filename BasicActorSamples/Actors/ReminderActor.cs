using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace BasicActorSamples.Actors
{
    public interface IReminderActor : IActor
    {
        Task<string> GetAlarmStatus();
    }
    
    public class ReminderActor : Actor, IReminderActor, IRemindable
    {
        private const string IS_SNOOZING_KEY = "is_snoozing";
        private const string REMINDER_NAME = "snooze";
        
        public ReminderActor(ActorHost host) : base(host)
        {
        }

        public async Task<string> GetAlarmStatus()
        {
            return await StateManager.GetStateAsync<bool>(IS_SNOOZING_KEY) ? "Snoozing!" : "Not snoozing.";
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == REMINDER_NAME)
            {
                Console.WriteLine($"{REMINDER_NAME} fired!");
                StateManager.SetStateAsync(IS_SNOOZING_KEY, true);
            }

            return Task.CompletedTask;
        }
    }
}