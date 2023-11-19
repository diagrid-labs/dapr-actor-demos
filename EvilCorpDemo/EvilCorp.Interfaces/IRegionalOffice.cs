using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IRegionalOffice : IActor
    {
        Task SetRegionalOfficeDataAsync(RegionalOfficeData regionalOfficeData);
        Task<RegionalOfficeData> GetRegionalOfficeDataAsync();
        Task SetAlarmDeviceEmployeeMappingAsync(Dictionary<string, string> employeeIds);
        Task<Dictionary<string, string>> GetAlarmDeviceEmployeeMappingAsync();
        Task<string[]> GetEmployeeIdsAsync();
        Task TriggerAlarmDevicesAsync();
        Task FireEmployeeAsync(string alarmDeviceId);
    }

    public class RegionalOfficeData
    {
        public RegionalOfficeData()
        {
        }

        public RegionalOfficeData(
            string id,
            string timeZoneId,
            string headQuartersId,
            DateTime utcSyncTime,
            DateTime wakeUpTime)
        {
            Id = id;
            TimeZoneId = timeZoneId;
            HeadQuartersId = headQuartersId;
            UtcSyncTime = utcSyncTime;
            WakeUpTime = wakeUpTime;
        }

        public string Id { get; init;}
        public string TimeZoneId { get; init; }
        public string HeadQuartersId { get; init; }
        public DateTime UtcSyncTime { get; init; }
        public DateTime WakeUpTime { get; init; }
    }
}