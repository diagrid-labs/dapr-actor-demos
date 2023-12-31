using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IRegionalOffice : IActor
    {
        Task SetRegionalOfficeDataAsync(RegionalOfficeData regionalOfficeData);
        Task<RegionalOfficeData> GetRegionalOfficeDataAsync();
        Task SetAlarmClockEmployeeMappingAsync(Dictionary<string, string> employeeIds);
        Task<Dictionary<string, string>> GetAlarmClockEmployeeMappingAsync();
        Task<string> GetEmployeeIdAsync(string alarmClockId);
        Task SetAlarmClockTimeAsync(DateTime time);
        Task FireEmployeeAsync(string alarmClockId);
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
            DateTime utcSyncTime)
        {
            Id = id;
            TimeZoneId = timeZoneId;
            HeadQuartersId = headQuartersId;
            UtcSyncTime = utcSyncTime;
        }

        public string Id { get; init;}
        public string TimeZoneId { get; init; }
        public string HeadQuartersId { get; init; }
        public DateTime UtcSyncTime { get; init; }
    }
}