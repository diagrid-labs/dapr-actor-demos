using Dapr.Actors;
using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class RegionalOfficeActor : Actor, IRegionalOffice
    {
        private const string REGIONAL_OFFICE_DATA_KEY = "alarm-device-ids";
        private const string ALARM_DEVICE_IDS_KEY = "alarm-device-ids";

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

        public async Task SetAlarmDeviceIdsAsync(IEnumerable<string> alarmDeviceIds)
        {
            await StateManager.SetStateAsync(ALARM_DEVICE_IDS_KEY, alarmDeviceIds);
        }

        public async Task<string[]> GetAlarmDeviceIdsAsync()
        {
            return await StateManager.GetStateAsync<string[]>(ALARM_DEVICE_IDS_KEY);
        }

        public async Task TriggerAlarmDevicesAsync()
        {
            Console.WriteLine("Triggering alarm devices for {Region}!", Id);

            var alarmDeviceIds = await GetAlarmDeviceIdsAsync();
            foreach (var alarmDeviceId in alarmDeviceIds)
            {
                var alarmDeviceProxy = ProxyFactory.CreateActorProxy<IAlarmDevice>(
                    new ActorId(alarmDeviceId),
                    nameof(AlarmDeviceActor));
                await alarmDeviceProxy.TriggerAlarmAsync();
            }
        }

        public async Task FireEmployeeAsync(string employeeId)
        {
            var regionalOfficeData = await GetRegionalOfficeDataAsync();
            var headQuartersProxy = ProxyFactory.CreateActorProxy<IHeadQuarters>(
                new ActorId(regionalOfficeData.HeadQuartersId),
                nameof(HeadQuartersActor));
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