using Dapr.Actors;
using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class AlarmClockActor : Actor, IAlarmClock
    {
        private const string ALARM_CLOCK_DATA_KEY = "alarm-clock-data";
        private const string SNOOZE_COUNT_KEY = "snooze-count";
        private const string TIME_KEY = "time";
        private const string IS_ACKNOWLEDGED_KEY = "is-acknowledged";
        private const string TIME_TIMER_NAME = "time-timer";

        public AlarmClockActor(ActorHost host) : base(host)
        {
        }

        public async Task SetAlarmClockDataAsync(AlarmClockData alarmClockData)
        {
            await StateManager.SetStateAsync(ALARM_CLOCK_DATA_KEY, alarmClockData);
            await SetIsAlarmAcknowledgedAsync(false);
        }

        public async Task<AlarmClockData> GetAlarmClockDataAsync()
        {
            return await StateManager.GetStateAsync<AlarmClockData>(ALARM_CLOCK_DATA_KEY);
        }

        public async Task SnoozeAlarmAsync()
        {
            Logger.LogInformation("{ActorId} Snoozing/ignoring alarm", Id);
            int snoozeCount = 1;
            var snoozeCountResult = await TryGetSnoozeCountAsync();
            if (snoozeCountResult.HasValue)
            {
                snoozeCount = snoozeCountResult.Value + 1;
            }
            Logger.LogInformation("{ActorId} Snooze count is {SnoozeCount}", Id, snoozeCount);
            await SetSnoozeCountAsync(snoozeCount);
        }

        public async Task StopTimersAsync()
        {
            Logger.LogInformation("{ActorId} Stopping timer", Id);
            await SetIsAlarmAcknowledgedAsync(true);
            await UnregisterTimerAsync(TIME_TIMER_NAME);
        }

        private async Task AlarmHandler()
        {
            var alarmClockData = await GetAlarmClockDataAsync();
            if (await IsActiveEmployee(alarmClockData))
            {
                Logger.LogInformation("{AlarmClockId}: WAKE UP AND GO TO WORK!", Id.GetId());
                await GetEmployeeResponseAsync(alarmClockData);

                var absoluteLimit = alarmClockData.AlarmTime.Add(alarmClockData.MaxSnoozeTime);
                var currentTime = await GetTimeAsync();
                if (currentTime >= absoluteLimit)
                {
                    Logger.LogInformation("{AlarmClockId}: Time limit exceeded. Employee will be fired!", Id.GetId());
                    await FireEmployee(alarmClockData);
                }

                var snoozeCountResult = await TryGetSnoozeCountAsync();
                if (snoozeCountResult.HasValue && snoozeCountResult.Value == 3)
                {
                    Logger.LogInformation("{AlarmClockId}: Snooze limit exceeded. Employee will be fired!", Id.GetId());
                    await FireEmployee(alarmClockData);
                }
            }
        }

        private async Task GetEmployeeResponseAsync(AlarmClockData alarmClockData)
        {
            var regionalOfficeActorId = new ActorId(alarmClockData.RegionalOfficeId);
            var regionalOfficeActorProxy = ProxyFactory.CreateActorProxy<IRegionalOffice>(
                regionalOfficeActorId,
                nameof(RegionalOfficeActor));
            var employeeId = await regionalOfficeActorProxy.GetEmployeeIdAsync(Id.GetId());
            var employeeProxy = ProxyFactory.CreateActorProxy<IEmployee>(
                new ActorId(employeeId),
                nameof(EmployeeActor));
            var responseMethod = await employeeProxy.GetAlarmResponseAsync();
            switch (responseMethod)
            {
                case "StopTimersAsync":
                    await StopTimersAsync();
                    break;
                default:
                    await SnoozeAlarmAsync();
                    break;
            }
        }

        private async Task<bool> IsActiveEmployee(AlarmClockData alarmClockData)
        {
            var regionalOfficeActorId = new ActorId(alarmClockData.RegionalOfficeId);
            var regionalOfficeActorProxy = ProxyFactory.CreateActorProxy<IRegionalOffice>(
                regionalOfficeActorId,
                nameof(RegionalOfficeActor));
            var employeeId = await regionalOfficeActorProxy.GetEmployeeIdAsync(Id.GetId());

            return employeeId != string.Empty;
        }

        private async Task FireEmployee(AlarmClockData alarmClockData)
        {
            await StopTimersAsync();
            var regionalOfficeActorId = new ActorId(alarmClockData.RegionalOfficeId);
            var regionalOfficeActorProxy = ProxyFactory.CreateActorProxy<IRegionalOffice>(
                regionalOfficeActorId,
                nameof(RegionalOfficeActor));
            await regionalOfficeActorProxy.FireEmployeeAsync(Id.GetId());
        }

        public async Task SetSyncTimeAsync(DateTime time)
        {
            await UnregisterReminderAsync(TIME_TIMER_NAME);

            Logger.LogInformation("{AlarmClockId} Set time to {Time}", Id, time);
            await SetTimeAsync(time);

            await RegisterTimerAsync(
                TIME_TIMER_NAME,
                nameof(IncrementTimeHandler),
                null,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(1));
        }

        private async Task IncrementTimeHandler()
        {
            if (await GetIsAlarmAcknowledgedAsync())
            {
                return;
            }
            var alarmClockData = await GetAlarmClockDataAsync();
            var time = await GetTimeAsync();
            var incrementedTime = time.AddMinutes(10);
            await SetTimeAsync(incrementedTime);

            Logger.LogInformation("{AlarmClockId} Set time to {time}", Id, incrementedTime);

            if (incrementedTime >= alarmClockData.AlarmTime)
            {
                await AlarmHandler();
            }
        }

        private async Task<bool> GetIsAlarmAcknowledgedAsync()
        {
            return await StateManager.GetStateAsync<bool>(IS_ACKNOWLEDGED_KEY);
        }

        private async Task SetIsAlarmAcknowledgedAsync(bool isAlarmAcknowledged)
        {
            await StateManager.SetStateAsync(IS_ACKNOWLEDGED_KEY, isAlarmAcknowledged);
        }

        private async Task SetTimeAsync(DateTime time)
        {
            await StateManager.SetStateAsync(TIME_KEY, time);
        }

        private async Task<DateTime> GetTimeAsync()
        {
            return await StateManager.GetStateAsync<DateTime>(TIME_KEY);
        }

        private async Task SetSnoozeCountAsync(int snoozeCount)
        {
            await StateManager.SetStateAsync(SNOOZE_COUNT_KEY, snoozeCount);
        }

        private async Task<ConditionalValue<int>> TryGetSnoozeCountAsync()
        {
            return await StateManager.TryGetStateAsync<int>(SNOOZE_COUNT_KEY);
        }
    }
}