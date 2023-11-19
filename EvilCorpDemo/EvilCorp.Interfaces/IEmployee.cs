using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IEmployee : IActor
    {
        Task SetEmployeeDataAsync(EmployeeData employeeData);
        Task<EmployeeData> GetEmployeeDataAsync();
        Task SnoozeAlarmAsync();
        Task AcknowledgeAlarmAsync();
    }

    public record EmployeeData(string AlarmDeviceId);
}