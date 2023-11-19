using Dapr.Actors;
using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class SimulationActor : Actor, ISimulation
    {
        private const string UTC_TIME_KEY = "utc-time";

        public SimulationActor(ActorHost host) : base(host)
        {
        }

        public async Task InitActorsAsync()
        {
            // Create the HeadQuarters actor
            Logger.LogInformation("Creating HeadQuarters actor");
            var headQuartersId = new ActorId("headquarters");
            var hqProxy = ProxyFactory.CreateActorProxy<IHeadQuarters>(headQuartersId, nameof(HeadQuartersActor));

            // At 3:00 UTC regional offices will sync the time with all the AlarmDevices
            var utcSyncTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 3, 0, 0);
            Logger.LogInformation("Setting UTC time to {UtcSyncTime}", utcSyncTime);
            await StateManager.SetStateAsync(UTC_TIME_KEY, utcSyncTime);

            var globalEmployeeIdList = new Dictionary<string, string[]>();
            
            var regionalWakeUpTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);

            var londonOfficeData = new RegionalOfficeData("London", "GMT Standard Time", headQuartersId.GetId(), utcSyncTime, regionalWakeUpTime);
            var londonOfficeProxy = await AddRegionalOffice(londonOfficeData, new Range(1, 3));
            var londonEmployeeIds = (await londonOfficeProxy.GetAlarmDeviceEmployeeMappingAsync()).Values.ToArray();
            Logger.LogInformation("London employee IDs: {LondonEmployeeIds}", string.Join(", ", londonEmployeeIds));
            globalEmployeeIdList.Add(londonOfficeData.Id, londonEmployeeIds);

            // var newYorkOfficeData = new RegionalOfficeData("New York", "Eastern Standard Time", headQuartersId.GetId(), utcSyncTime, regionalWakeUpTime);
            // var newYorkOfficeProxy = await AddRegionalOffice(newYorkOfficeData, new Range(3, 5));
            // var newYorkEmployeeIds = (await newYorkOfficeProxy.GetAlarmDeviceEmployeeMappingAsync()).Values.ToArray();
            // Logger.LogInformation("New York employee IDs: {NewYorkEmployeeIds}", string.Join(", ", newYorkEmployeeIds));
            // globalEmployeeIdList.Add(newYorkOfficeData.Id, newYorkEmployeeIds);

            await hqProxy.SetEmployeeIdsAsync(globalEmployeeIdList);

            await Task.CompletedTask;
        }

        public async Task IncrementTimeAsync()
        {
            var utcTime = await StateManager.GetStateAsync<TimeOnly>(UTC_TIME_KEY);
            var incrementedTime = utcTime.AddHours(1);
            await StateManager.SetStateAsync(UTC_TIME_KEY, incrementedTime);
        }

        private async Task<IRegionalOffice> AddRegionalOffice(RegionalOfficeData regionalOfficeData, Range employeeIds)
        {
            Logger.LogInformation("Creating RegionalOffice actor {RegionalOfficeName}", regionalOfficeData.Id);
            var regionalOfficeId = new ActorId(regionalOfficeData.Id);
            var regionalOfficeProxy = ProxyFactory.CreateActorProxy<IRegionalOffice>(regionalOfficeId, nameof(RegionalOfficeActor));

            await regionalOfficeProxy.SetRegionalOfficeDataAsync(regionalOfficeData);

            var employeeAndAlarmDeviceIds = await AddAlarmDevicesAndEmployees(employeeIds, regionalOfficeData);
            await regionalOfficeProxy.SetAlarmDeviceEmployeeMappingAsync(employeeAndAlarmDeviceIds);

            return regionalOfficeProxy;
        }

        private async Task<Dictionary<string, string>> AddAlarmDevicesAndEmployees(
            Range employeeNumbers,
            RegionalOfficeData regionalOfficeData)
        {
            var alarmDeviceEmployeeMapping = new Dictionary<string, string>();

            for (int i = employeeNumbers.Start.Value; i < employeeNumbers.End.Value; i++)
            {
                var employeeId = new ActorId($"Employee {i}");
                var employeeProxy = ProxyFactory.CreateActorProxy<IEmployee>(employeeId, nameof(EmployeeActor));
                Logger.LogInformation("Created employee {EmployeeId}", employeeId.GetId());

                var alarmDeviceId = new ActorId($"AlarmDevice {i}");
                var alarmDeviceProxy = ProxyFactory.CreateActorProxy<IAlarmDevice>(alarmDeviceId, nameof(AlarmDeviceActor));
                Logger.LogInformation("Created alarm device {AlarmDeviceId}", alarmDeviceId.GetId());
                alarmDeviceEmployeeMapping.Add(alarmDeviceId.GetId(), employeeId.GetId());

                await alarmDeviceProxy.SetAlarmDeviceDataAsync(
                    new AlarmDeviceData(
                        regionalOfficeData.Id,
                        regionalOfficeData.WakeUpTime));

                // Use the utcSyncTime and the TimeZoneInfo to set the regional time on the AlarmDevice.
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(regionalOfficeData.TimeZoneId);
                var regionalSyncDateTime = TimeZoneInfo.ConvertTimeFromUtc(regionalOfficeData.UtcSyncTime, timeZone);
                await alarmDeviceProxy.SetTimeAsync(regionalSyncDateTime);
                var employeeData = new EmployeeData(alarmDeviceId.GetId());
                await employeeProxy.SetEmployeeDataAsync(employeeData);
            }

            return alarmDeviceEmployeeMapping;
        }
    }
}