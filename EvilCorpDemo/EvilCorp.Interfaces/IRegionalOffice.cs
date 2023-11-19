using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IRegionalOffice : IActor
    {
        Task SetRegionalOfficeDataAsync(RegionalOfficeData regionalOfficeData);
        Task<RegionalOfficeData> GetRegionalOfficeDataAsync();
        Task SetAlarmDeviceIdsAsync(IEnumerable<string> employeeIds);
        Task<string[]> GetAlarmDeviceIdsAsync();
        Task<string[]> GetEmployeeIdsAsync();
        Task TriggerAlarmDevicesAsync();
        Task FireEmployeeAsync(string employeeId);
    }

    public record RegionalOfficeData(TimeZoneInfo TimeZone, string HeadQuartersId, TimeOnly WakeUpTime);
}