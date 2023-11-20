using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IAlarmClock : IActor
    {
        Task SetAlarmClockDataAsync(AlarmClockData alarmClockData);
        Task<AlarmClockData> GetAlarmClockDataAsync();
        Task TriggerAlarmAsync();
        Task StopAlarmAsync();
        Task SetTimeAsync(DateTime time);
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