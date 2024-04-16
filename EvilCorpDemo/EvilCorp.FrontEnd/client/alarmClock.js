let clockFont;

class AlarmClock {
    constructor(id, count, maxItemsPerRow) {
        //    col 0 1 2 3  4  5
        //row 0 - 0 1 2 3  4  5
        //row 1 - 6 7 8 9 10 11

        this.id = id;
        this.label = split(id, "-")[1];
        this.count = count;
        this.maxItemsPerRow = maxItemsPerRow;
        this.padding = 10;
        this.row = Math.floor(count / this.maxItemsPerRow)
        this.col = count % this.maxItemsPerRow;
        this.sizeX = 120;
        this.sizeY = 80;
        this.x = this.padding + this.col * this.sizeX + this.col * this.padding;
        this.y = this.padding + this.row * this.sizeY + this.row * this.padding;
        this.alarmTime;
        this.currentTime;
        this.isAcknowledged = false;
        this.isTriggered = false;
        this.isSnoozed = false;
        this.snoozeCount = 0;
        this.reachedMaxSnoozeCount = false;
    }

    setTimes(currentTime, alarmTime) {
        this.currentTime = this.parseDateTime(currentTime);
        this.alarmTime = this.parseDateTime(alarmTime);
        this.isTriggered = false;
        this.isSnoozed = false;
    }

    parseDateTime(dateTime) {
        const timeOnly = dateTime.split("T")[1].split(".")[0];
        const timeParts = timeOnly.split(":");
        return timeParts[0] + ":" + timeParts[1];
    }

    setAcknowledged() {
        this.isAcknowledged = true;
        this.isTriggered = false;
        this.isSnoozed = false;
    }

    setTriggered() {
        this.isTriggered = true;
        this.isSnoozed = false;
    }

    setSnoozed() {
        this.isSnoozed = true;
        this.isTriggered = false;
    }

    setSnoozeCount(snoozeCount) {
        this.snoozeCount = snoozeCount;
        this.setSnoozed();
    }

    setReachedMaxSnoozeCount() {
        this.reachedMaxSnoozeCount = true;
    }

    getFillColor() {
        let colorCode;
        if (this.reachedMaxSnoozeCount) {
            colorCode = "#FF0000";
        } else if (this.isAcknowledged) {
            colorCode = "#00FF00";
        } else if (this.isSnoozed) {
            colorCode = "#0000FF";
        } else {
            colorCode = "#FFFFFF";
        }

        return color(colorCode);
    }

    getTextColor() {
        let colorCode;
        if (this.reachedMaxSnoozeCount || this.isSnoozed) {
            colorCode = "#FFFFFF";
        } else {
            colorCode = "#000000";
        }

        return color(colorCode);
    }

    draw() {
        strokeWeight(3);
        stroke('#000000')
        fill(this.getFillColor());
        rect(this.x, this.y, this.sizeX, this.sizeY, 10);
        
        strokeWeight(0);
        textAlign(CENTER);
        textFont(clockFont);
        fill(this.getTextColor());
        textSize(16);
        text(this.label, this.x + this.sizeX/2, this.y + this.sizeY/4);
        if (this.currentTime) {
            textSize(26);
            text(this.currentTime, this.x + this.sizeX/2, this.y + this.sizeY/1.6);
        }
        if (this.snoozeCount > 0) {
            textSize(16);
            text(this.snoozeCount, this.x + this.sizeX/2, this.y + this.sizeY/1.2);
        }
    }
}