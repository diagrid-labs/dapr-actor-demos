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

        public async Task HandleAlarmAsync()
        {
            var random = new Random();
            var outcome = random.Next(0, 3);
            switch (outcome)
            {
                case 0:
                    Logger.LogInformation("{ActorId} Acknowledging alarm", Id);
                    await AcknowledgeAlarmAsync();
                    break;
                case 1:
                    //Logger.LogInformation("{ActorId} Snoozing alarm", Id);
                    //await SnoozeAlarmAsync();
                    //break;
                default:
                    Logger.LogInformation("{ActorId} Ignoring alarm", Id);
                    break;
            }
        }

        private async Task AcknowledgeAlarmAsync()
        {
            var employeeData = await GetEmployeeDataAsync();
            var alarmClockProxy = ProxyFactory.CreateActorProxy<IAlarmClock>(
                new ActorId(employeeData.AlarmClockId),
                nameof(AlarmClockActor));
            await alarmClockProxy.StopTimersAsync();
        }

        private async Task SnoozeAlarmAsync()
        {
            var employeeData = await GetEmployeeDataAsync();
            var alarmClockProxy = ProxyFactory.CreateActorProxy<IAlarmClock>(
                new ActorId(employeeData.AlarmClockId),
                nameof(AlarmClockActor));
            await alarmClockProxy.SnoozeAlarmAsync();
        }
    }
}