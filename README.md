# Dapr Actor Demos

## EvilCorp Actor Demo

A fictional company called EvilCorp 😈 wants their employees to be more productive and have decided to implement a system with smart alarm clocks that will wake up their employees at 7am. If the employees have not acknowledged the alarm within 3 snoozes, the alarm will send a message to the headquarters to lay off the employee 😱.

The front-end of the demo is a basic HTML page that uses [p5js](https://p5js.org/) to draw the information on the page. The back-end is a [Dapr](https://dapr.io/) application written in ASP.NET that uses actors for the headquarters, the regional office, the employees, and the alarm clocks. Communication between the back-end and the front-end is realized with [Ably](https://ably.com/).

![EvilCorp Demo](./media/evilcorp-demo.gif)

When the demo is run you'll see a realtime display of the alarm clocks in the EvilCorp Dashboard. Alarm clocks are represented by a square. The color of the square indicates the state of the alarm clock:

- Green: Alarm has been acknowledged on time.
- Blue: Alarm has been snoozed.
- Red: Alarm has been snoozed for too long, employee will be laid off.

### Prerequisites for running the demo locally

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
- [Ably account](https://ably.com/signup) (free tier is sufficient)

### Configuring the demo

#### Back-end

1. Copy the [Ably API Root key](https://ably.com/docs/ids-and-keys#api-key) from the Ably portal.
2. Rename the `EvilCorpDemo/EvilCorp.Api/secrets.json.example` file to `EvilCorpDemo/EvilCorp.Api/secrets.json` and paste Ably API key in the `AblyApiKey` field:

    ```json
    {
        "AblyApiKey": "YOUR_ABLY_API_KEY_HERE"
    }
    ```

    > The `secrets.json` file is excluded from source control by the `.gitignore` file, so you don't have to worry the API key will be exposed to the public.

3. Use the terminal to navigate to the `EvilCorpDemo` folder and build the solution:

    ```bash
    dotnet build
    ```

#### Front-end

1. Rename the `EvilCorpDemo/EvilCorp.FrontEnd/secrets.json.example` file to `EvilCorpDemo/EvilCorp.FrontEnd/secrets.json` and paste Ably API key in the `AblyApiKey` field:

    ```json
    {
        "AblyApiKey": "YOUR_ABLY_API_KEY_HERE"
    }
    ```

2. Use the terminal to navigate to the `EvilCorpDemo/EvilCorp.FrontEnd` folder and install the dependencies for the front-end:

    ```bash
    npm install
    ```

### Running the demo

1. Use the terminal to navigate to the `EvilCorpDemo` folder.
2. Use Dapr multi-app run to start both the back-end and front-end with a single command:

    ```bash
    dapr run -f .
    ```

    The `EvilCorpDemo/dapr.yaml` file contains the configuration of the apps that will be started.

3. Open the [front-end](http://localhost:5500) in a browser and click the `Connect` button. This establishes a connection with Ably Realtime.
4. Enter the number of employees you want to simulate and click the `Create actors` button. This will create actors for the EvilCorp head quarters, the regional office, the employees and the alarm clocks.
   > Be careful not to create too many employees. The free tier of Ably is rate-limited to handle 70 messages per second. More info in the [Ably limits docs](https://ably.com/docs/general/limits).
5. Click the `Start time` button to start the simulation. This will set the time of all the alarm clocks and after each two seconds 10 minutes will pass in the simulation. Once the alarm clocks reach 7:00, the employee actors will randomly acknowledge or snooze the alarm.

## Basic Actor Samples

### HelloWorldActor

### StatefulActor

### TimerActor

### ReminderActor

### ActorToActor

### HttpCallActor

## Resources

1. [Dapr Actors overview](https://docs.dapr.io/developing-applications/building-blocks/actors/actors-overview/).
2. [Dapr Actors API reference](https://docs.dapr.io/reference/api/actors_api/)

## More information

Any questions or comments about this sample? Join the [Dapr discord](https://bit.ly/dapr-discord) and post a message the `APIS > #actors` channel.
Have you made something with Dapr? Post a message in the `#show-and-tell` channel, we love to see your creations!
