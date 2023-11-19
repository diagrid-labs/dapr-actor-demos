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

    public class EmployeeData
    {
        public EmployeeData()
        {
            
        }

        public EmployeeData(string alarmDeviceId)
        {
            AlarmDeviceId = alarmDeviceId;
        }

        public string AlarmDeviceId { get; init; }
    }
}