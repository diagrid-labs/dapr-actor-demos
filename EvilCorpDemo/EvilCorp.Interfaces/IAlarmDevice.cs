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

    public record AlarmDeviceData(string RegionalOfficeId, string EmployeeId, TimeOnly AlarmTime);
}