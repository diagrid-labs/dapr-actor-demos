using Dapr.Actors;
using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class RegionalOfficeActor : Actor, IRegionalOffice
    {
        private const string REGIONAL_OFFICE_DATA_KEY = "regional-office-data";
        private const string ALARM_CLOCK_EMPLOYEE_KEY = "alarm-clock-employee";

        public RegionalOfficeActor(ActorHost host) : base(host)
        {
        }

        public async Task SetRegionalOfficeDataAsync(RegionalOfficeData regionalOfficeData)
        {
            await StateManager.SetStateAsync(REGIONAL_OFFICE_DATA_KEY, regionalOfficeData);
        }

        public async Task<RegionalOfficeData> GetRegionalOfficeDataAsync()
        {
            return await StateManager.GetStateAsync<RegionalOfficeData>(REGIONAL_OFFICE_DATA_KEY);
        }

        public async Task SetAlarmClockEmployeeMappingAsync(Dictionary<string, string> alarmClockEmployeeMapping)
        {
            await StateManager.SetStateAsync(ALARM_CLOCK_EMPLOYEE_KEY, alarmClockEmployeeMapping);
        }

        public async Task<Dictionary<string, string>> GetAlarmClockEmployeeMappingAsync()
        {
            return await StateManager.GetStateAsync<Dictionary<string, string>>(ALARM_CLOCK_EMPLOYEE_KEY);
        }

        public async Task SetAlarmClockTimeAsync(DateTime utcSyncTime)
        {
            Logger.LogInformation("Setting alarm clocks for {Region}!", Id);

            // Use the utcSyncTime and the TimeZoneInfo to set the regional time on the AlarmClock.
            var regionalOfficeData = await GetRegionalOfficeDataAsync();
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(regionalOfficeData.TimeZoneId);
            var regionalSyncDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcSyncTime, timeZone);

            var mapping = await GetAlarmClockEmployeeMappingAsync();
            var alarmClockTasks = new List<Task>();
            foreach (var alarmClockId in mapping.Keys)
            {
                var alarmClockProxy = ProxyFactory.CreateActorProxy<IAlarmClock>(
                    new ActorId(alarmClockId),
                    nameof(AlarmClockActor));
                alarmClockTasks.Add(alarmClockProxy.SetSyncTimeAsync(regionalSyncDateTime));
            }

            await Task.WhenAll(alarmClockTasks);
        }

        public async Task<string> GetEmployeeIdAsync(string alarmClockId)
        {
            var mapping = await GetAlarmClockEmployeeMappingAsync();
            mapping.TryGetValue(alarmClockId, out string? employeeId);

            return employeeId ?? string.Empty;
        }

        public async Task FireEmployeeAsync(string alarmClockId)
        {
            var employeeId = await GetEmployeeIdAsync(alarmClockId);
            if (employeeId != string.Empty)
            {
                var employeeProxy = ProxyFactory.CreateActorProxy<IEmployee>(
                    new ActorId(employeeId),
                    nameof(EmployeeActor));
                await employeeProxy.SetIsFiredAsync();

                await RemoveEmployeeAsync(employeeId);

                var regionalOfficeData = await GetRegionalOfficeDataAsync();
                var headQuartersProxy = ProxyFactory.CreateActorProxy<IHeadQuarters>(
                    new ActorId(regionalOfficeData.HeadQuartersId),
                    nameof(HeadQuartersActor));
                await headQuartersProxy.FireEmployeeAsync(Id.GetId(), employeeId);
            }
        }

        private async Task RemoveEmployeeAsync(string employeeId)
        {
            var mapping = await GetAlarmClockEmployeeMappingAsync();
            var alarmClockEmployeeKey = mapping.FirstOrDefault(x => x.Value == employeeId).Key;
            mapping.Remove(alarmClockEmployeeKey);
            await SetAlarmClockEmployeeMappingAsync(mapping);
        }
    }
}