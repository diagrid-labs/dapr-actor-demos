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
        private const string ALARMSTATUS_KEY = "alarmstatus";
        private const string REMINDER_NAME = "alarm";
        
        public ReminderActor(ActorHost host) : base(host)
        {
        }

        public async Task<string> GetAlarmStatus()
        {
            return await StateManager.GetStateAsync<bool>(ALARMSTATUS_KEY) ? "Alarm went off!" : "Alarm did not went off.";
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            if (reminderName == REMINDER_NAME)
            {
                Console.WriteLine($"{REMINDER_NAME} fired!");
                StateManager.SetStateAsync(ALARMSTATUS_KEY, true);
            }

            return Task.CompletedTask;
        }
    }
}