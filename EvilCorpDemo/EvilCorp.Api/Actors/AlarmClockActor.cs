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
        private readonly IRealtimeNotification _realtimeNotification;

        public AlarmClockActor(ActorHost host, IRealtimeNotification realtimeNotification) : base(host)
        {
            _realtimeNotification = realtimeNotification;
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
            Logger.LogInformation("{ActorId} Is snoozed", Id);
            var snoozeCount = await GetSnoozeCountAsync();
            snoozeCount += 1;
            Logger.LogInformation("{ActorId} Snooze count = {SnoozeCount}", Id, snoozeCount);
            await SetSnoozeCountAsync(snoozeCount);

            await _realtimeNotification.SendSnoozeAlarmMessageAsync(
                new AlarmClockMessage(
                    Id.GetId(),
                    (await GetAlarmClockDataAsync()).AlarmTime,
                    await GetTimeAsync(),
                    snoozeCount));
        }

        public async Task StopTimersAsync()
        {
            Logger.LogInformation("{ActorId} Stopping timer", Id);
            await SetIsAlarmAcknowledgedAsync(true);
            await UnregisterTimerAsync(TIME_TIMER_NAME);

            await _realtimeNotification.SendAlarmAcknowledgedMessageAsync(
                new AlarmClockMessage(
                    Id.GetId(),
                    (await GetAlarmClockDataAsync()).AlarmTime,
                    await GetTimeAsync(),
                    await GetSnoozeCountAsync(),
                    true));
        }

        private async Task AlarmHandler(AlarmClockData alarmClockData)
        {
            if (await IsActiveEmployee(alarmClockData))
            {
                Logger.LogInformation("{AlarmClockId}: WAKE UP AND GO TO WORK!", Id.GetId());
                await GetEmployeeResponseAndExecuteAsync(alarmClockData);

                var snoozeCount = await GetSnoozeCountAsync();
                if (snoozeCount > alarmClockData.MaxAllowedSnoozeCount)
                {
                    Logger.LogInformation("{AlarmClockId}: Snooze limit exceeded. Employee will be fired!", Id.GetId());
                    await FireEmployee(alarmClockData);
                    
                    await _realtimeNotification.SendSnoozeLimitMessageAsync(
                        new AlarmClockMessage(
                            Id.GetId(),
                            alarmClockData.AlarmTime,
                            await GetTimeAsync(),
                            snoozeCount));
                }
            }
        }

        private async Task GetEmployeeResponseAndExecuteAsync(AlarmClockData alarmClockData)
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
            await UnregisterReminderAsync(TIME_TIMER_NAME);
            await SetIsAlarmAcknowledgedAsync(true);

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
            await SendTimeUpdate(await GetAlarmClockDataAsync(), time);

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
            var incrementedTime = time.AddMinutes(alarmClockData.TimeIncrementMinutes);
            await SetTimeAsync(incrementedTime);
            await SendTimeUpdate(alarmClockData, incrementedTime);

            Logger.LogInformation("{AlarmClockId} Set time to {time}", Id, incrementedTime);

            if (incrementedTime >= alarmClockData.AlarmTime)
            {
                await AlarmHandler(alarmClockData);
            }
        }

        private async Task SendTimeUpdate(AlarmClockData alarmClockData, DateTime incrementedTime)
        {
            await _realtimeNotification.SendUpdateTimeMessageAsync(
                new AlarmClockMessage(
                    Id.GetId(),
                    alarmClockData.AlarmTime,
                    incrementedTime
                ));
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

        private async Task<int> GetSnoozeCountAsync()
        {
            var result = await StateManager.TryGetStateAsync<int>(SNOOZE_COUNT_KEY);

            return result.HasValue ? result.Value : 0;
        }
    }
}