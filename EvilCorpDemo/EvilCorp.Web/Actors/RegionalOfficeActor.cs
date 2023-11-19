using Dapr.Actors;
using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class RegionalOfficeActor : Actor, IRegionalOffice
    {
        private const string REGIONAL_OFFICE_DATA_KEY = "regional-office-data";
        private const string ALARM_DEVICE_EMPLOYEE_KEY = "alarm-device-employee";

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

        public async Task SetAlarmDeviceEmployeeMappingAsync(Dictionary<string, string> alarmDeviceEmployeeMapping)
        {
            await StateManager.SetStateAsync(ALARM_DEVICE_EMPLOYEE_KEY, alarmDeviceEmployeeMapping);
        }

        public async Task<Dictionary<string, string>> GetAlarmDeviceEmployeeMappingAsync()
        {
            return await StateManager.GetStateAsync<Dictionary<string, string>>(ALARM_DEVICE_EMPLOYEE_KEY);
        }

        public async Task TriggerAlarmDevicesAsync()
        {
            Logger.LogInformation("Triggering alarm devices for {Region}!", Id);

            var mapping = await GetAlarmDeviceEmployeeMappingAsync();
            foreach (var alarmDeviceId in mapping.Keys)
            {
                var alarmDeviceProxy = ProxyFactory.CreateActorProxy<IAlarmDevice>(
                    new ActorId(alarmDeviceId),
                    nameof(AlarmDeviceActor));
                await alarmDeviceProxy.TriggerAlarmAsync();
            }
        }

        public async Task FireEmployeeAsync(string alarmDeviceId)
        {
            var regionalOfficeData = await GetRegionalOfficeDataAsync();
            var headQuartersProxy = ProxyFactory.CreateActorProxy<IHeadQuarters>(
                new ActorId(regionalOfficeData.HeadQuartersId),
                nameof(HeadQuartersActor));
            var mapping = await GetAlarmDeviceEmployeeMappingAsync();
            var employeeId = mapping[alarmDeviceId];
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