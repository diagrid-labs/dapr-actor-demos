using Dapr.Actors;
using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class AlarmClockActor : Actor, IAlarmClock
    {
        private const string ALARM_CLOCK_DATA_KEY = "alarm-clock-data";
        private const string SNOOZE_COUNT_KEY = "snooze-count";
        private const string ALARM_TIMER_NAME = "alarm-timer";
        private const string TIME_KEY = "time";
        private const string IS_ALARM_TRIGGERED_KEY = "is-alarm-triggered";
        private const string TIME_TIMER_NAME = "time-timer";

        public AlarmClockActor(ActorHost host) : base(host)
        {
        }

        public async Task SetAlarmClockDataAsync(AlarmClockData alarmClockData)
        {
            await StateManager.SetStateAsync(ALARM_CLOCK_DATA_KEY, alarmClockData);
            await SetIsAlarmTriggeredAsync(false);
        }

        public async Task<AlarmClockData> GetAlarmClockDataAsync()
        {
            return await StateManager.GetStateAsync<AlarmClockData>(ALARM_CLOCK_DATA_KEY);
        }

        private async Task TriggerAlarmAsync()
        {
            await RegisterTimerAsync(
                ALARM_TIMER_NAME,
                nameof(AlarmHandler),
                null,
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(6));

            await SetIsAlarmTriggeredAsync(true);
        }

        public async Task SnoozeAlarmAsync()
        {
            Logger.LogInformation("{ActorId} snoozing alarm", Id);
            int snoozeCount = 0;
            var snoozeCountResult = await TryGetSnoozeCountAsync();
            if (snoozeCountResult.HasValue)
            {
                Logger.LogInformation("{ActorId} Stored snoozeCount {snoozeCountResult.Value}", Id, snoozeCountResult.Value);
                snoozeCount = snoozeCountResult.Value + 1;
            }
            await SetSnoozeCountAsync(snoozeCount);
            Logger.LogInformation("{ActorId} new snoozeCount: {SnoozeCount}", Id, snoozeCount);

            await Task.CompletedTask;
        }

        public async Task StopTimersAsync()
        {
            await UnregisterTimerAsync(TIME_TIMER_NAME);
            await UnregisterTimerAsync(ALARM_TIMER_NAME);
        }

        private async Task AlarmHandler()
        {
            var alarmClockData = await GetAlarmClockDataAsync();
            if (await IsActiveEmployee(alarmClockData))
            {
                Logger.LogInformation("{AlarmClockId}: WAKE UP AND GO TO WORK!", Id.GetId());
                await CallEmployee(alarmClockData);
                int snoozeCount = 0;
                var snoozeCountResult = await TryGetSnoozeCountAsync();
                if (snoozeCountResult.HasValue)
                {
                    if (snoozeCount == 3)
                    {
                        Logger.LogInformation("{AlarmClockId}: Snooze limit exceeded. Employee will be fired.", Id.GetId());
                        await FireEmployee(alarmClockData);
                    }
                }
                else
                {
                    var absoluteLimit = alarmClockData.AlarmTime.Add(alarmClockData.MaxSnoozeTime);
                    var currentTime = await GetTimeAsync();
                    if (currentTime >= absoluteLimit)
                    {
                        Logger.LogInformation("{AlarmClockId}: Time limit exceeded. Employee will be fired.", Id.GetId());
                        await FireEmployee(alarmClockData);
                    }
                }
            }
        }

        private async Task CallEmployee(AlarmClockData alarmClockData)
        {
            var regionalOfficeActorId = new ActorId(alarmClockData.RegionalOfficeId);
            var regionalOfficeActorProxy = ProxyFactory.CreateActorProxy<IRegionalOffice>(
                regionalOfficeActorId,
                nameof(RegionalOfficeActor));
            var employeeId = await regionalOfficeActorProxy.GetEmployeeIdAsync(Id.GetId());
            var employeeProxy = ProxyFactory.CreateActorProxy<IEmployee>(
                new ActorId(employeeId),
                nameof(EmployeeActor));
            await employeeProxy.HandleAlarmAsync();
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
            var alarmClockData = await GetAlarmClockDataAsync();
            var time = await GetTimeAsync();
            var incrementedTime = time.AddMinutes(10);
            await SetTimeAsync(incrementedTime);

            Logger.LogInformation("{AlarmClockId} Set time to {time}", Id, incrementedTime);

            var isAlarmTriggered = await GetIsAlarmTriggeredAsync();

            if (incrementedTime >= alarmClockData.AlarmTime && !isAlarmTriggered)
            {
                await TriggerAlarmAsync();
            }
        }

        private async Task<bool> GetIsAlarmTriggeredAsync()
        {
            return await StateManager.GetStateAsync<bool>(IS_ALARM_TRIGGERED_KEY);
        }

        private async Task SetIsAlarmTriggeredAsync(bool isAlarmTriggered)
        {
            await StateManager.SetStateAsync(IS_ALARM_TRIGGERED_KEY, isAlarmTriggered);
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