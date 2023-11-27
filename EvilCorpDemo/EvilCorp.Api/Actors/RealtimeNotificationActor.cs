using IO.Ably;
using IO.Ably.Rest;
using EvilCorp.Interfaces;

namespace EvilCorp.Web
{
    public class RealtimeNotification : IRealtimeNotification
    {
        private readonly IRestChannel _channel;

        public RealtimeNotification(IRestClient realtimeClient)
        {
            _channel = realtimeClient.Channels.Get("evil-corp");
        }

        public async Task SendAlarmClockIdsMessageAsync(string[] alarmClockIds)
        {
            await SendNotification("addAlarmClocks", new { alarmClockIds } );
        }

        public async Task SendUpdateTimeMessageAsync(AlarmClockMessage alarmClockMessage)
        {
            await SendNotification("updateTime", alarmClockMessage );
        }

        public async Task SendAlarmTriggeredMessageAsync(AlarmClockMessage alarmClockMessage)
        {
            await SendNotification("alarmTriggered", alarmClockMessage );
        }

        public async Task SendAlarmAcknowledgedMessageAsync(AlarmClockMessage alarmClockMessage)
        {
            await SendNotification("alarmAcknowledged", alarmClockMessage );
        }

        public async Task SendSnoozeAlarmMessageAsync(AlarmClockMessage alarmClockMessage)
        {
            await SendNotification("snoozeIncrement", alarmClockMessage );
        }

        public async Task SendSnoozeLimitMessageAsync(AlarmClockMessage alarmClockMessage)
        {
            await SendNotification("snoozeLimit", alarmClockMessage );
        }

        private async Task SendNotification(string messageType, object data)
        {
            await _channel.PublishAsync(messageType, data);
        }

    }
}