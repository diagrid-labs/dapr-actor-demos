using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IAlarmClock : IActor
    {
        Task SetAlarmClockDataAsync(AlarmClockData alarmClockData);
        Task<AlarmClockData> GetAlarmClockDataAsync();
        Task SnoozeAlarmAsync();
        Task StopAlarmAsync();
        Task SetSyncTimeAsync(DateTime syncTime);
    }

    public class AlarmClockData
    {
        public AlarmClockData()
        {
        }

        public AlarmClockData(string regionalOfficeId, DateTime alarmTime)
        {
            RegionalOfficeId = regionalOfficeId;
            AlarmTime = alarmTime;
        }

        public string RegionalOfficeId { get; init; }
        public DateTime AlarmTime { get; init; }
    }
}