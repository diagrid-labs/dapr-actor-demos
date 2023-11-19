using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IAlarmDevice : IActor
    {
        Task SetAlarmDeviceDataAsync(AlarmDeviceData alarmDeviceData);
        Task<AlarmDeviceData> GetAlarmDeviceDataAsync();
        Task TriggerAlarmAsync();
        Task StopAlarmAsync();
        Task SetTimeAsync(TimeOnly time);
    }

    public class AlarmDeviceData
    {
        public AlarmDeviceData()
        {
        }

        public AlarmDeviceData(string regionalOfficeId, TimeOnly alarmTime)
        {
            RegionalOfficeId = regionalOfficeId;
            AlarmTime = alarmTime;
        }

        public string RegionalOfficeId { get; init; }
        public TimeOnly AlarmTime { get; init; }
    }
}