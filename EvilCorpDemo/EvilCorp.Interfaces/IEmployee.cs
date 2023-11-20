using Dapr.Actors;

namespace EvilCorp.Interfaces
{
    public interface IEmployee : IActor
    {
        Task SetEmployeeDataAsync(EmployeeData employeeData);
        Task<EmployeeData> GetEmployeeDataAsync();
        Task HandleAlarmAsync();
    }

    public class EmployeeData
    {
        public EmployeeData()
        {
            
        }

        public EmployeeData(string alarmClockId)
        {
            AlarmClockId = alarmClockId;
        }

        public string AlarmClockId { get; init; }
    }
}