using Dapr.Actors;
using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class AlarmDeviceActor : Actor, IAlarmDevice
    {
        private const string ALARM_DEVICE_DATA_KEY = "alarm-device-data";
        private const string SNOOZE_COUNT_KEY = "snooze-count";
        private const string TIMER_NAME = "snooze";

        public AlarmDeviceActor(ActorHost host) : base(host)
        {
        }

        public async Task SetAlarmDeviceDataAsync(AlarmDeviceData alarmDeviceData)
        {
            await StateManager.SetStateAsync(ALARM_DEVICE_DATA_KEY, alarmDeviceData);
        }

        public async Task<AlarmDeviceData> GetAlarmDeviceDataAsync()
        {
            return await StateManager.GetStateAsync<AlarmDeviceData>(ALARM_DEVICE_DATA_KEY);
        }

        public async Task TriggerAlarmAsync()
        {
            await RegisterTimerAsync(
                TIMER_NAME,
                nameof(AlarmHandler),
                null,
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(5));
        }

        public async Task StopAlarmAsync()
        {
            await UnregisterTimerAsync(TIMER_NAME);
        }

        public async Task AlarmHandler()
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
    }
}