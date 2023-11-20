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

        public async Task TriggerAlarmAsync()
        {
            await RegisterTimerAsync(
                ALARM_TIMER_NAME,
                nameof(SnoozeHandler),
                null,
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(5));

            await SetIsAlarmTriggeredAsync(true);
        }

        public async Task StopAlarmAsync()
        {
            await UnregisterTimerAsync(ALARM_TIMER_NAME);
        }

        private async Task SnoozeHandler()
        {
            var alarmClockData = await GetAlarmClockDataAsync();
            Logger.LogInformation("WAKE UP AND GO TO WORK {AlarmClockId}!", Id.GetId());

            int snoozeCount = 0;
            var snoozeCountResult = await StateManager.TryGetStateAsync<int>(SNOOZE_COUNT_KEY);
            if (snoozeCountResult.HasValue)
            {
                if (snoozeCount == 3)
                {
                    // Employee will be fired
                    var regionalOfficeActorId = new ActorId(alarmClockData.RegionalOfficeId);
                    var regionalOfficeActorProxy = ProxyFactory.CreateActorProxy<IRegionalOffice>(
                        regionalOfficeActorId,
                        nameof(RegionalOfficeActor));
                    await regionalOfficeActorProxy.FireEmployeeAsync(Id.GetId());
                    await StopAlarmAsync();
                }
                else
                {
                    Logger.LogInformation("Snoozing for {ActorId}", Id);
                    snoozeCount++;
                    await StateManager.SetStateAsync(SNOOZE_COUNT_KEY, snoozeCount);
                }
            }
            else
            {
                // This is the first time the alarm has been triggered, snoozeCount = 0.
                await StateManager.SetStateAsync(SNOOZE_COUNT_KEY, snoozeCount);
            }
        }

        public async Task SetTimeAsync(DateTime time)
        {
            Logger.LogInformation("Setting time for {AlarmClockId} to {Time}", Id, time);

            await StateManager.SetStateAsync(TIME_KEY, time);

            await RegisterTimerAsync(
                TIME_TIMER_NAME,
                nameof(IncrementTimeHandler),
                null,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(5));
        }

        public async Task IncrementTimeHandler()
        {
            var time = await GetTime();
            var incrementedTime = time.AddMinutes(5);
            await StateManager.SetStateAsync(TIME_KEY, incrementedTime);

            Logger.LogInformation("Incremented time for {AlarmClockId} to {time}", Id, incrementedTime);

            var alarmClockData = await GetAlarmClockDataAsync();
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

        private async Task<DateTime> GetTime()
        {
            return await StateManager.GetStateAsync<DateTime>(TIME_KEY);
        }
    }
}