using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IAlarmDevice : IActor
    {
        Task SetAlarmDeviceDataAsync(AlarmDeviceData alarmDeviceData);
        Task<AlarmDeviceData> GetAlarmDeviceDataAsync();
        Task TriggerAlarmAsync();
        Task StopAlarmAsync();
        Task SetTimeAsync(DateTime time);
    }

    public class AlarmDeviceData
    {
        public AlarmDeviceData()
        {
        }

        public AlarmDeviceData(string regionalOfficeId, DateTime alarmTime)
        {
            RegionalOfficeId = regionalOfficeId;
            AlarmTime = alarmTime;
        }

        public string RegionalOfficeId { get; init; }
        public DateTime AlarmTime { get; init; }
    }
}