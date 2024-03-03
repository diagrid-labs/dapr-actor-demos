// EvilCorp demo
// Marc Duiker, Nov 2023
// @marcduiker

let alarmClocks = [];

function setup() {
    frameRate(30);
    createCanvas(windowWidth, windowHeight);
}

function connectClient() {
    clientId = "evil-corp-client";
    console.log(`clientId: ${clientId}`);
    connectAbly(clientId);
}

async function createActors() {
    select("#createActorsButton").elt.disabled = true;
    let employeeCount = select("#numberOfEmployeesInput").value();

    let url = `http://evilcorp-api:7100/init/${employeeCount}`;
    let options = {
        method: 'POST',
        mode: "no-cors",
    };
    await fetch(url, options)
}

async function startTime() {
    select("#startTimeButton").elt.disabled = true;

    let url = "http://evilcorp-api:7100/start";
    let params = {
        method: "POST",
        mode: "no-cors",
    };
    await fetch(url, params)
}

function test() {
    handleAddAlarmClocks({
        alarmClockIds: ["AlarmClock-1", "AlarmClock-2", "AlarmClock-3", "AlarmClock-4", "AlarmClock-5", "AlarmClock-6", "AlarmClock-7", "AlarmClock-8", "AlarmClock-9", "AlarmClock-10"]
    });

    handleAlarmAcknowledged({Id: "AlarmClock-1"});
    handleAlarmTriggered({Id: "AlarmClock-2"});
    handleUpdateTime({Id: "AlarmClock-3", CurrentTime: "12:34", "AlarmTime": "7:00"});
    handleSnoozeIncrement({Id: "AlarmClock-4", CurrentTime: "12:34", "AlarmTime": "7:00", SnoozeCount: 1});
    handleSnoozeLimit({Id: "AlarmClock-5", CurrentTime: "12:34", "AlarmTime": "7:00", SnoozeCount: 1});
}

function draw() {
    
    background(255);
    if (alarmClocks.length > 0) {
        alarmClocks.forEach(alarmClock => {
            alarmClock.draw();
        });
    }
}

function handleAddAlarmClocks(data) {
    const itemSize = 120;
    const maxItemsPerRow = Math.floor(windowWidth / (itemSize + 15));
    let itemCount = 0;
    data.alarmClockIds.forEach(alarmClockId => {
        const clock = new AlarmClock(alarmClockId, itemCount, maxItemsPerRow);
        alarmClocks.push(clock);
        itemCount++;
    });
}

function handleUpdateTime(data) {
    const alarmClock = alarmClocks.find(alarmClock => alarmClock.id === data.Id);
    if (alarmClock) {
        alarmClock.setTimes(data.CurrentTime, data.AlarmTime);
    }
}

function handleAlarmTriggered(data) {
    const alarmClock = alarmClocks.find(alarmClock => alarmClock.id === data.Id);
    if (alarmClock) {
        alarmClock.setTriggered();
    }
}

function handleAlarmAcknowledged(data) {
    const alarmClock = alarmClocks.find(alarmClock => alarmClock.id === data.Id);
    if (alarmClock) {
        alarmClock.setAcknowledged(true);
    }
}

function handleSnoozeIncrement(data) {
    const alarmClock = alarmClocks.find(alarmClock => alarmClock.id === data.Id);
    if (alarmClock) {
        alarmClock.setSnoozeCount(data.SnoozeCount);
    }
}

function handleSnoozeLimit(data) {
    const alarmClock = alarmClocks.find(alarmClock => alarmClock.id === data.Id);
    if (alarmClock) {
        alarmClock.setReachedMaxSnoozeCount();
    }
}