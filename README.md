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

### Running the demo

#### Back-end

1. Copy the [Ably API Root key](https://ably.com/docs/ids-and-keys#api-key) from the Ably portal.
2. Open the `EvilCorpDemo\EvilCorp.Api\Program.cs` file and insert the Ably API key in the `ABLY_API_KEY` constant:

    ```csharp
    const string ABLY_API_KEY = "INSERT_ABLY_API_KEY_HERE";
    ```

3. Use the terminal to navigate to the `EvilCorpDemo` folder and build the solution:

    ```bash
    dotnet build
    ```

4. Use Dapr multi-app run to start the back-end:

    ```bash
    dapr run -f .
    ```

    The `EvilCorpDemo/dapr.yaml` file contains the configuration of the apps that will be started.

The back-end is now running and you can continue with the front-end.

#### Front-end

1. Open the `EvilCorpDemo/EvilCorp.FrontEnd/realtime.js` file and update the `ABLY_API_KEY` constant with the Ably API key:

    ```javascript
    const ABLY_API_KEY = "INSERT_ABLY_API_KEY_HERE";
    ```

2. Run the `EvilCorpDemo/EvilCorp.FrontEnd/index.html` file using a local web server. For example, using the [Live Server](https://marketplace.visualstudio.com/items?itemName=ritwickdey.LiveServer) extension in VS Code.
3. Open the front-end in a browser and click the `Connect` button. This establishes a connection with Ably Realtime.
4. Enter the number of employees you want to simulate and click the `Create actors` button. This will create actors for the EvilCorp Head Quarters, the regional office, the employees and the alarm clocks.
   > Be careful not to create too many employees. The free tier of ably can only handle 70 messages per second. More info in the [Ably limits docs](https://ably.com/docs/general/limits).
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
