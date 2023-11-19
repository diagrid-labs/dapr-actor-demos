using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IAlarmDevice : IActor
    {
        Task SetAlarmDeviceDataAsync(AlarmDeviceData alarmDeviceData);
        Task<AlarmDeviceData> GetAlarmDeviceDataAsync();
        Task TriggerAlarmAsync();
        Task StopAlarmAsync();
    }

    public record AlarmDeviceData(string RegionalOfficeId, string EmployeeId, DateTime AlarmTime);
}