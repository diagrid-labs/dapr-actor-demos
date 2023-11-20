using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class EmployeeActor : Actor, IEmployee
    {
        private const string EMPLOYEE_DATA_KEY = "employee-data";
        private const string IS_FIRED_KEY = "is-fired";

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

        public Task<string> GetAlarmResponseAsync()
        {
            var random = new Random();
            var outcome = random.Next(0, 2);
            string alarmClockMethod = string.Empty;
            switch (outcome)
            {
                case 0:
                    Logger.LogInformation("{ActorId} Acknowledging alarm", Id);
                    alarmClockMethod = "StopTimersAsync";
                    break;
                default:
                    Logger.LogInformation("{ActorId} Snoozing/ignoring alarm", Id);
                    break;
            }

            return Task.FromResult(alarmClockMethod);
        }

        public async Task SetIsFiredAsync()
        {
            await StateManager.SetStateAsync(IS_FIRED_KEY, true);
        }
    }
}