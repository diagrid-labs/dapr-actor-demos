using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IRegionalOffice : IActor
    {
        Task SetRegionalOfficeDataAsync(RegionalOfficeData regionalOfficeData);
        Task<RegionalOfficeData> GetRegionalOfficeDataAsync();
        Task SetAlarmDeviceIdsAsync(IEnumerable<string> employeeIds);
        Task<string[]> GetAlarmDeviceIdsAsync();
        Task TriggerAlarmDevicesAsync();
        Task FireEmployeeAsync(string employeeId);
    }

    public record Coordinate(double X, double Y);
    public record RegionalOfficeData(string Name, TimeZoneInfo TimeZone, Coordinate Coordinate, string HeadQuartersId);
}