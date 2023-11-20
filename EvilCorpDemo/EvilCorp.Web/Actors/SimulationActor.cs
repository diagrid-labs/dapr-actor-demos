using Dapr.Actors;
using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class SimulationActor : Actor, ISimulation
    {
        public SimulationActor(ActorHost host) : base(host)
        {
        }

        public async Task InitActorsAsync(SimulationData data)
        {
            // Create the HeadQuarters actor
            Logger.LogInformation("Creating HeadQuarters actor");
            var headQuartersId = new ActorId("headquarters");
            var headQuartersProxy = ProxyFactory.CreateActorProxy<IHeadQuarters>(headQuartersId, nameof(HeadQuartersActor));

            // At 6:00 UTC regional offices will sync the time with all the AlarmClocks
            var utcSyncTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 6, 0, 0);
            var globalEmployeeIdList = new Dictionary<string, string[]>();

            var londonOfficeData = new RegionalOfficeData("London", "GMT Standard Time", headQuartersId.GetId(), utcSyncTime);
            var londonOfficeProxy = await AddRegionalOffice(londonOfficeData, new Range(1, data.EmployeeCount + 1));
            var londonAlarmClockEmployeeMapping = await londonOfficeProxy.GetAlarmClockEmployeeMappingAsync();
            
            Logger.LogInformation("London EmployeeIds: {LondonEmployeeIds}", string.Join(", ", londonAlarmClockEmployeeMapping.Values.ToArray()));
            Logger.LogInformation("London AlarmClockIds: {LondonAlarmClockIds}", string.Join(", ", londonAlarmClockEmployeeMapping.Keys.ToArray()));

            globalEmployeeIdList.Add(londonOfficeData.Id, londonAlarmClockEmployeeMapping.Values.ToArray());

            // var newYorkOfficeData = new RegionalOfficeData("New York", "Eastern Standard Time", headQuartersId.GetId(), utcSyncTime);
            // var newYorkOfficeProxy = await AddRegionalOffice(newYorkOfficeData, new Range(3, 5));
            // var newYorkEmployeeIds = (await newYorkOfficeProxy.GetAlarmClockEmployeeMappingAsync()).Values.ToArray();
            // Logger.LogInformation("New York employee IDs: {NewYorkEmployeeIds}", string.Join(", ", newYorkEmployeeIds));
            // globalEmployeeIdList.Add(newYorkOfficeData.Id, newYorkEmployeeIds);

            await headQuartersProxy.SetRegionalOfficeIdsAsync(new string[] { londonOfficeData.Id } );
            await headQuartersProxy.SetEmployeeIdsAsync(globalEmployeeIdList);
        }

        public async Task StartTimeAsync()
        {
            var headQuartersId = new ActorId("headquarters");
            var headQuartersProxy = ProxyFactory.CreateActorProxy<IHeadQuarters>(headQuartersId, nameof(HeadQuartersActor));
            var regionalOfficeIds = await headQuartersProxy.GetRegionalOfficeIdsAsync();
            foreach (var regionalOfficeId in regionalOfficeIds)
            {
                var regionalOfficeProxy = ProxyFactory.CreateActorProxy<IRegionalOffice>(
                    new ActorId(regionalOfficeId),
                    nameof(RegionalOfficeActor));
                var regionalOfficeData = await regionalOfficeProxy.GetRegionalOfficeDataAsync();
                await regionalOfficeProxy.SetAlarmClockTimeAsync(regionalOfficeData.UtcSyncTime);
            }
        }

        private async Task<IRegionalOffice> AddRegionalOffice(RegionalOfficeData regionalOfficeData, Range employeeIds)
        {
            Logger.LogInformation("Creating RegionalOffice actor {RegionalOfficeName}", regionalOfficeData.Id);
            var regionalOfficeId = new ActorId(regionalOfficeData.Id);
            var regionalOfficeProxy = ProxyFactory.CreateActorProxy<IRegionalOffice>(regionalOfficeId, nameof(RegionalOfficeActor));

            await regionalOfficeProxy.SetRegionalOfficeDataAsync(regionalOfficeData);

            var employeeAndAlarmClockIds = await AddAlarmClocksAndEmployees(employeeIds, regionalOfficeData);
            await regionalOfficeProxy.SetAlarmClockEmployeeMappingAsync(employeeAndAlarmClockIds);

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

                var alarmClockId = new ActorId($"AlarmClock-{i}");
                var alarmClockProxy = ProxyFactory.CreateActorProxy<IAlarmClock>(alarmClockId, nameof(AlarmClockActor));

                alarmClockEmployeeMapping.Add(alarmClockId.GetId(), employeeId.GetId());

                var regionalWakeUpTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);

                await alarmClockProxy.SetAlarmClockDataAsync(
                    new AlarmClockData(
                        regionalOfficeData.Id,
                        regionalWakeUpTime,
                        timeIncrementMinutes: 10,
                        maxAllowedSnoozeCount: 3));

                var employeeData = new EmployeeData(alarmClockId.GetId());
                await employeeProxy.SetEmployeeDataAsync(employeeData);
            }

            return alarmClockEmployeeMapping;
        }
    }
}