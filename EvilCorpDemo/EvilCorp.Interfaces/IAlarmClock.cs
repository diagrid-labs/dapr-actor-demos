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

        public AlarmClockData(string regionalOfficeId, DateTime alarmTime, int timeIncrementMinutes, int maxAllowedSnoozeCount)
        {
            RegionalOfficeId = regionalOfficeId;
            AlarmTime = alarmTime;
            TimeIncrementMinutes = timeIncrementMinutes;
            MaxAllowedSnoozeCount = maxAllowedSnoozeCount;
        }

        public string RegionalOfficeId { get; init; }
        public DateTime AlarmTime { get; init; }
        public int TimeIncrementMinutes { get; set; }
        public int MaxAllowedSnoozeCount { get; init; }
    }
}