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
        this.size = 120;
        this.x = this.padding + this.col * this.size + this.col * this.padding;
        this.y = this.padding + this.row * this.size + this.row * this.padding;
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
        rect(this.x, this.y, this.size, this.size, 10);
        
        strokeWeight(0);
        textAlign(CENTER);
        textFont('monospace');
        fill(this.getTextColor());
        textSize(20);
        text(this.label, this.x + this.size/2, this.y + this.size/3);
        if (this.currentTime) {
            textSize(16);
            text(this.currentTime, this.x + this.size*(3/4), this.y + this.size/1.8);
        }
        if (this.alarmTime) {
            textSize(12);
            text(this.alarmTime, this.x + this.size/4, this.y + this.size/1.8);
        }
        if (this.snoozeCount > 0) {
            textSize(16);
            text(this.snoozeCount, this.x + this.size/2, this.y + this.size/1.3);
        }
    }
}