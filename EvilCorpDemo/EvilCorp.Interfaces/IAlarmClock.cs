using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IAlarmClock : IActor
    {
        Task SetAlarmClockDataAsync(AlarmClockData alarmClockData);
        Task<AlarmClockData> GetAlarmClockDataAsync();
        Task SnoozeAlarmAsync();
        Task StopTimersAsync();
        Task SetSyncTimeAsync(DateTime syncTime);
    }

    public class AlarmClockData
    {
        public AlarmClockData()
        {
        }

        public AlarmClockData(string regionalOfficeId, DateTime alarmTime, TimeSpan snoozeInterval, TimeSpan maxSnoozeTime)
        {
            RegionalOfficeId = regionalOfficeId;
            AlarmTime = alarmTime;
            SnoozeInterval = snoozeInterval;
            MaxSnoozeTime = maxSnoozeTime;
        }

        public string RegionalOfficeId { get; init; }
        public DateTime AlarmTime { get; init; }
        public TimeSpan SnoozeInterval { get; init; }
        public TimeSpan MaxSnoozeTime { get; init; }
    }
}