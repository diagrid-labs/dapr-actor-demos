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

        public async Task TriggerAlarmClocksAsync()
        {
            Logger.LogInformation("Triggering alarm clocks for {Region}!", Id);

            var mapping = await GetAlarmClockEmployeeMappingAsync();
            foreach (var alarmClockId in mapping.Keys)
            {
                var alarmClockProxy = ProxyFactory.CreateActorProxy<IAlarmClock>(
                    new ActorId(alarmClockId),
                    nameof(AlarmClockActor));
                await alarmClockProxy.TriggerAlarmAsync();
            }
        }

        public async Task FireEmployeeAsync(string alarmClockId)
        {
            var regionalOfficeData = await GetRegionalOfficeDataAsync();
            var headQuartersProxy = ProxyFactory.CreateActorProxy<IHeadQuarters>(
                new ActorId(regionalOfficeData.HeadQuartersId),
                nameof(HeadQuartersActor));
            var mapping = await GetAlarmClockEmployeeMappingAsync();
            var employeeId = mapping[alarmClockId];
            await headQuartersProxy.FireEmployeeAsync(Id.GetId(), employeeId);
        }

        public async Task<string[]> GetEmployeeIdsAsync()
        {
            var regionalOfficeData = await GetRegionalOfficeDataAsync();
            var headQuartersId = new ActorId(regionalOfficeData.HeadQuartersId);
            var headQuartersProxy = ProxyFactory.CreateActorProxy<IHeadQuarters>(
                headQuartersId,
                nameof(HeadQuartersActor));

            return await headQuartersProxy.GetEmployeeIdsForRegionalOfficeAsync(Id.GetId());
        }
    }
}