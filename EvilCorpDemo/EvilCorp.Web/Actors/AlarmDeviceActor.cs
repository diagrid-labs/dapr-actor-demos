using Dapr.Actors;
using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class AlarmDeviceActor : Actor, IAlarmDevice
    {
        private const string ALARM_DEVICE_DATA_KEY = "alarm-device-data";
        private const string SNOOZE_COUNT_KEY = "snooze-count";
        private const string ALARM_TIMER_NAME = "alarm-timer";
        private const string TIME_KEY = "time";
        private const string IS_ALARM_TRIGGERED_KEY = "is-alarm-triggered";
        private const string TIME_TIMER_NAME = "time-timer";

        public AlarmDeviceActor(ActorHost host) : base(host)
        {
        }

        public async Task SetAlarmDeviceDataAsync(AlarmDeviceData alarmDeviceData)
        {
            await StateManager.SetStateAsync(ALARM_DEVICE_DATA_KEY, alarmDeviceData);
            await SetIsAlarmTriggeredAsync(false);
        }

        public async Task<AlarmDeviceData> GetAlarmDeviceDataAsync()
        {
            return await StateManager.GetStateAsync<AlarmDeviceData>(ALARM_DEVICE_DATA_KEY);
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
            var alarmDeviceData = await GetAlarmDeviceDataAsync();
            Console.WriteLine("WAKE UP AND GO TO WORK {EmployeeId}!", alarmDeviceData.EmployeeId);

            int snoozeCount = 0;
            var snoozeCountResult = await StateManager.TryGetStateAsync<int>(SNOOZE_COUNT_KEY);
            if (snoozeCountResult.HasValue)
            {
                if (snoozeCount == 3)
                {
                    // Employee will be fired
                    var regionalOfficeActorId = new ActorId(alarmDeviceData.RegionalOfficeId);
                    var regionalOfficeActorProxy = ProxyFactory.CreateActorProxy<IRegionalOffice>(
                        regionalOfficeActorId,
                        nameof(RegionalOfficeActor));
                    await regionalOfficeActorProxy.FireEmployeeAsync(alarmDeviceData.EmployeeId);
                    await StopAlarmAsync();
                }
                else
                {
                    Console.WriteLine("Snoozing for {ActorId}", Id);
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

        public async Task SetTimeAsync(TimeOnly time)
        {
            Console.WriteLine("Setting time for {AlarmDeviceActorId} to {Time}", Id, time);

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

            Console.WriteLine("Incremented time for {AlarmDeviceActorId} to {time}", Id, incrementedTime);

            var alarmDeviceData = await GetAlarmDeviceDataAsync();
            var isAlarmTriggered = await GetIsAlarmTriggeredAsync();
            if (incrementedTime >= alarmDeviceData.AlarmTime && !isAlarmTriggered)
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

        private async Task<TimeOnly> GetTime()
        {
            return await StateManager.GetStateAsync<TimeOnly>(TIME_KEY);
        }
    }
}