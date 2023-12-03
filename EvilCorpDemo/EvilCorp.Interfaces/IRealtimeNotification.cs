namespace EvilCorp.Interfaces
{
    public interface IRealtimeNotification
    {
        Task SendAlarmClockIdsMessageAsync(string[] alarmClockIds);
        Task SendUpdateTimeMessageAsync(AlarmClockMessage alarmClockMessage);
        Task SendAlarmTriggeredMessageAsync(AlarmClockMessage alarmClockMessage);
        Task SendAlarmAcknowledgedMessageAsync(AlarmClockMessage alarmClockMessage);
        Task SendSnoozeAlarmMessageAsync(AlarmClockMessage alarmClockMessage);
        Task SendSnoozeLimitMessageAsync(AlarmClockMessage alarmClockMessage);
    }

    public class AlarmClockMessage
    {
        public AlarmClockMessage()
        {
        }

        public AlarmClockMessage(string id, DateTime alarmTime, DateTime? currentTime = null, int snoozeCount = 0, bool isAlarmAcknowledged = false)
        {
            Id = id;
            AlarmTime = alarmTime;
            CurrentTime = currentTime;
            SnoozeCount = snoozeCount;
            IsAlarmAcknowledged = isAlarmAcknowledged;
        }

        public string Id { get; init; }
        public DateTime AlarmTime { get; init; }
        public DateTime? CurrentTime { get; init; }
        public int SnoozeCount { get; init; }
        public bool IsAlarmAcknowledged { get; init; }
    }
}