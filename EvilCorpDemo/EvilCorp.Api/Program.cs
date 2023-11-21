
using Dapr.Actors;
using Dapr.Actors.Client;
using EvilCorp.Interfaces;
using EvilCorp.Web;
using IO.Ably;

var builder = WebApplication.CreateBuilder(args);

// Using a hardcoded API key is unsafe!
// Only use this on your local machine.
// Don't push to a public git repo or to a public facing server.
const string ABLY_API_KEY = "";

builder.Services.AddSingleton<IRestClient>(
    new AblyRest(ABLY_API_KEY)
);

// Add services to the container.
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<HeadQuartersActor>();
    options.Actors.RegisterActor<RegionalOfficeActor>();
    options.Actors.RegisterActor<AlarmClockActor>();
    options.Actors.RegisterActor<EmployeeActor>();
    options.Actors.RegisterActor<SimulationActor>();
    options.Actors.RegisterActor<RealtimeNotificationActor>();
    options.ActorScanInterval = TimeSpan.FromSeconds(10);
    options.ActorIdleTimeout = TimeSpan.FromMinutes(10);
    options.ReentrancyConfig = new ActorReentrancyConfig()
    {
        Enabled = true,
        MaxStackDepth = 32
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); Don't use this, this middleware blocks Dapr calls
app.MapActorsHandlers();

var proxyFactory = new ActorProxyFactory();
var simulationActorId = new ActorId("simulation");
var simulationProxy = proxyFactory.CreateActorProxy<ISimulation>(
    simulationActorId,
    nameof(SimulationActor));

app.MapPost("/init", async (SimulationData data) => {
    await simulationProxy.InitActorsAsync(data);
});

app.MapPost("/start", async () => {
    await simulationProxy.StartTimeAsync();
});

app.Run();