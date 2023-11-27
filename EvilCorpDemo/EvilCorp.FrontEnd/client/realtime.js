let ably;
let channel;
const channelName = "evil-corp";
const addAlarmClocksMessage = "addAlarmClocks";
const updateTimeMessage = "updateTime";
const alarmTriggeredMessage = "alarmTriggered";
const alarmAcknowledgedMessage = "alarmAcknowledged";
const snoozeIncrementMessage = "snoozeIncrement";
const snoozeLimitMessage = "snoozeLimit";

async function connectAbly(clientId) {
  let apiKeyResponse = await fetch("/get-ably-api-key");
  let apiKey = await apiKeyResponse.text();
  const isConnected = ably?.connection.state === "connected";
  if (!isConnected) {
    const clientOptions = {
      key: apiKey,
      clientId: clientId,
      echoMessages: false,
    };
    ably = new Ably.Realtime.Promise(clientOptions);
    ably.connection.on("connected", () => {
      console.log("Connected 🎉");
      select("#connectButton").elt.innerText = "Disconnect";
    });
    ably.connection.on("closed", () => {
      console.log("Disconnected 😿");
      select("#connectButton").elt.innerText = "Connect";
    });
    channel = await ably.channels.get(channelName);
    channel.subscribe(addAlarmClocksMessage, (message) => {
      handleAddAlarmClocks(message.data);
      });
    channel.subscribe(updateTimeMessage, (message) => {
      handleUpdateTime(message.data);
    });
    channel.subscribe(alarmTriggeredMessage, (message) => {
      handleAlarmTriggered(message.data);
    });
    channel.subscribe(alarmAcknowledgedMessage, (message) => {
      handleAlarmAcknowledged(message.data);
    });
    channel.subscribe(snoozeIncrementMessage, (message) => {
      handleSnoozeIncrement(message.data);
    });
    channel.subscribe(snoozeLimitMessage, (message) => {
      handleSnoozeLimit(message.data);
    });
  } else {
    ably.close();
  }
}