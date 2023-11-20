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
            var headQuartersProxy = ProxyFactory.CreateActorProxy<IHeadQuarters>(headQuartersId, nameof(HeadQuartersActor));

            // At 3:00 UTC regional offices will sync the time with all the AlarmClocks
            var utcSyncTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 3, 0, 0);
            Logger.LogInformation("Setting UTC time to {UtcSyncTime}", utcSyncTime);
            await StateManager.SetStateAsync(UTC_TIME_KEY, utcSyncTime);

            var globalEmployeeIdList = new Dictionary<string, string[]>();
            
            var regionalWakeUpTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);

            var londonOfficeData = new RegionalOfficeData("London", "GMT Standard Time", headQuartersId.GetId(), utcSyncTime, regionalWakeUpTime);
            var londonOfficeProxy = await AddRegionalOffice(londonOfficeData, new Range(1, 3));
            var londonEmployeeIds = (await londonOfficeProxy.GetAlarmClockEmployeeMappingAsync()).Values.ToArray();
            Logger.LogInformation("London employee IDs: {LondonEmployeeIds}", string.Join(", ", londonEmployeeIds));
            globalEmployeeIdList.Add(londonOfficeData.Id, londonEmployeeIds);

            // var newYorkOfficeData = new RegionalOfficeData("New York", "Eastern Standard Time", headQuartersId.GetId(), utcSyncTime, regionalWakeUpTime);
            // var newYorkOfficeProxy = await AddRegionalOffice(newYorkOfficeData, new Range(3, 5));
            // var newYorkEmployeeIds = (await newYorkOfficeProxy.GetAlarmClockEmployeeMappingAsync()).Values.ToArray();
            // Logger.LogInformation("New York employee IDs: {NewYorkEmployeeIds}", string.Join(", ", newYorkEmployeeIds));
            // globalEmployeeIdList.Add(newYorkOfficeData.Id, newYorkEmployeeIds);

            await headQuartersProxy.SetEmployeeIdsAsync(globalEmployeeIdList);

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

            var employeeAndAlarmClockIds = await AddAlarmClocksAndEmployees(employeeIds, regionalOfficeData);
            await regionalOfficeProxy.SetAlarmClockEmployeeMappingAsync(employeeAndAlarmClockIds);
            await regionalOfficeProxy.SetAlarmClockTimeAsync(regionalOfficeData.UtcSyncTime);

            return regionalOfficeProxy;
        }

        private async Task<Dictionary<string, string>> AddAlarmClocksAndEmployees(
            Range employeeNumbers,
            RegionalOfficeData regionalOfficeData)
        {
            var alarmClockEmployeeMapping = new Dictionary<string, string>();

            for (int i = employeeNumbers.Start.Value; i < employeeNumbers.End.Value; i++)
            {
                var employeeId = new ActorId($"Employee-{i}");
                var employeeProxy = ProxyFactory.CreateActorProxy<IEmployee>(employeeId, nameof(EmployeeActor));
                Logger.LogInformation("Created employee {EmployeeId}", employeeId.GetId());

                var alarmClockId = new ActorId($"AlarmClock-{i}");
                var alarmClockProxy = ProxyFactory.CreateActorProxy<IAlarmClock>(alarmClockId, nameof(AlarmClockActor));
                Logger.LogInformation("Created alarm clock {AlarmClockId}", alarmClockId.GetId());
                alarmClockEmployeeMapping.Add(alarmClockId.GetId(), employeeId.GetId());

                await alarmClockProxy.SetAlarmClockDataAsync(
                    new AlarmClockData(
                        regionalOfficeData.Id,
                        regionalOfficeData.WakeUpTime));

                var employeeData = new EmployeeData(alarmClockId.GetId());
                await employeeProxy.SetEmployeeDataAsync(employeeData);
            }

            return alarmClockEmployeeMapping;
        }
    }
}