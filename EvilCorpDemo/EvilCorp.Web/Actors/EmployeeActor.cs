using Dapr.Actors;
using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class EmployeeActor : Actor, IEmployee
    {
        private const string EMPLOYEE_DATA_KEY = "employee-data";

        public EmployeeActor(ActorHost host) : base(host)
        {
        }

        public async Task SetEmployeeDataAsync(EmployeeData employeeData)
        {
            await StateManager.SetStateAsync(EMPLOYEE_DATA_KEY, employeeData);
        }

        public async Task<EmployeeData> GetEmployeeDataAsync()
        {
            return await StateManager.GetStateAsync<EmployeeData>(EMPLOYEE_DATA_KEY);
        }

        public async Task AcknowledgeAlarmAsync()
        {
            var employeeData = await GetEmployeeDataAsync();
            var alarmDeviceProxy = ProxyFactory.CreateActorProxy<IAlarmDevice>(
                new ActorId(employeeData.AlarmDeviceId),
                nameof(AlarmDeviceActor));
            await alarmDeviceProxy.StopAlarmAsync();
        }

        public Task SnoozeAlarmAsync()
        {
            throw new NotImplementedException();
        }
    }
}