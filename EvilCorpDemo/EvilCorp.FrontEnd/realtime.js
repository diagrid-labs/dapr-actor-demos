// Using a hardcoded API key is unsafe!
// Only use this on your local machine.
// Don't push to a public git repo or to a public facing server.
const ABLY_API_KEY = "INSERT_ABLY_API_KEY_HERE";

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
  const isConnected = ably?.connection.state === "connected";
  if (!isConnected) {
    const clientOptions = {
      key: ABLY_API_KEY,
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