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

        public  async Task InitActors()
        {
            // Create the HeadQuarters actor
            var headQuartersId = new ActorId("HQ");
            var hqProxy = ProxyFactory.CreateActorProxy<IHeadQuarters>(headQuartersId, nameof(HeadQuartersActor));

            // At 1:00 UTC regional office will sync the time with all the AlarmDevices
            var utcSyncTime = new TimeOnly(1,0);
            await StateManager.SetStateAsync(UTC_TIME_KEY, utcSyncTime);
            
            var globalEmployeeIdList = new Dictionary<string, string[]>();

            var londonRegionId = "London";
            var londonEmployeeIds = await AddRegionalOffice(londonRegionId, "GMT Standard Time", headQuartersId, 2, utcSyncTime);
            var newYorkRegionId = "New York";
            var newYorkEmployeeIds = await AddRegionalOffice(newYorkRegionId, "Eastern Standard Time", headQuartersId, 2, utcSyncTime);
            globalEmployeeIdList.Add(londonRegionId, londonEmployeeIds);
            globalEmployeeIdList.Add(newYorkRegionId, newYorkEmployeeIds);

            await hqProxy.SetEmployeeIdsAsync(globalEmployeeIdList);

            await Task.CompletedTask;
        }

        public async Task IncrementTime()
        {
            var utcTime = await StateManager.GetStateAsync<TimeOnly>(UTC_TIME_KEY);
            var incrementedTime =  utcTime.AddHours(1);
            await StateManager.SetStateAsync(UTC_TIME_KEY, incrementedTime);
        }

        private async Task<string[]> AddRegionalOffice(string regionalOfficeName, string timeZoneName, ActorId headQuartersId, int employeeCount, TimeOnly utcSyncTime)
        {
            var regionalOfficeId = new ActorId(regionalOfficeName);
            var regionalOfficeProxy = ProxyFactory.CreateActorProxy<IRegionalOffice>(regionalOfficeId, nameof(RegionalOfficeActor));

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);
            var wakeUpTime = new TimeOnly(7, 0);
            var regionalOfficeData = new RegionalOfficeData(timeZone, headQuartersId.GetId(), wakeUpTime);
            await regionalOfficeProxy.SetRegionalOfficeDataAsync(regionalOfficeData);

            var employeeAndAlarmDeviceIds = await AddEmployeesAndAlarmDevices(employeeCount, utcSyncTime, regionalOfficeId.GetId(), regionalOfficeData);
            await regionalOfficeProxy.SetAlarmDeviceIdsAsync(employeeAndAlarmDeviceIds.alarmDeviceIds);

            return employeeAndAlarmDeviceIds.employeeIds;
        }

        private async Task<(string[] employeeIds, string[] alarmDeviceIds)> AddEmployeesAndAlarmDevices(
            int employeeCount,
            TimeOnly utcSyncTime,
            string regionalOfficeId,
            RegionalOfficeData regionalOfficeData)
        {
            var employeeIdList = new List<string>();
            var alarmDeviceIdList = new List<string>();
            for (int i = 0; i < employeeCount; i++)
            {
                var employeeId = new ActorId($"Employee {i}");
                var employeeProxy = ProxyFactory.CreateActorProxy<IEmployee>(employeeId, nameof(EmployeeActor));
                Console.WriteLine("Created employee {EmployeeId}", employeeId.GetId());
                employeeIdList.Add(employeeId.GetId());

                var alarmDeviceId = new ActorId($"AlarmDevice {i}");
                var alarmDeviceProxy = ProxyFactory.CreateActorProxy<IAlarmDevice>(alarmDeviceId, nameof(AlarmDeviceActor));
                Console.WriteLine("Created alarm device {AlarmDeviceId}", alarmDeviceId.GetId());
                alarmDeviceIdList.Add(alarmDeviceId.GetId());

                await alarmDeviceProxy.SetAlarmDeviceDataAsync(
                    new AlarmDeviceData(
                        regionalOfficeId,
                        employeeId.GetId(),
                        regionalOfficeData.WakeUpTime));

                // Use the utcSyncTime and the TimeZoneInfo to set the regional time on the AlarmDevice.
                var syncDate = DateOnly.FromDateTime(DateTime.Today);
                var syncDateTime = syncDate.ToDateTime(utcSyncTime);
                var regionalSyncDateTime = TimeZoneInfo.ConvertTimeFromUtc(syncDateTime, regionalOfficeData.TimeZone);
                var regionalSyncTime = TimeOnly.FromDateTime(regionalSyncDateTime);
                await alarmDeviceProxy.SetTimeAsync(regionalSyncTime);

                var employeeData = new EmployeeData(alarmDeviceId.GetId());
                await employeeProxy.SetEmployeeDataAsync(employeeData);
            }

            return new (employeeIdList.ToArray(), alarmDeviceIdList.ToArray());
        }
    }
}