using Dapr.Actors.Runtime;
using EvilCorp.Interfaces;
using IO.Ably;
using IO.Ably.Rest;

namespace EvilCorp.Web
{
    public class RealtimeNotificationActor : Actor, IRealtimeNotification
    {
        private readonly IRestChannel _channel;

        public RealtimeNotificationActor(ActorHost host, IRestClient realtimeClient) : base(host)
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