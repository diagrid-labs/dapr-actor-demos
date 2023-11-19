
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using EvilCorp.Interfaces;
using EvilCorp.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<HeadQuartersActor>();
    options.Actors.RegisterActor<RegionalOfficeActor>();
    options.Actors.RegisterActor<AlarmDeviceActor>();
    options.Actors.RegisterActor<EmployeeActor>();
    options.Actors.RegisterActor<SimulationActor>();
    options.ReentrancyConfig = new Dapr.Actors.ActorReentrancyConfig()
    {
        Enabled = true,
        MaxStackDepth = 32,
    };
});

var app = builder.Build();
var daprClient = new DaprClientBuilder().Build();
var proxyFactory = new ActorProxyFactory();
var simulationProxy = proxyFactory.CreateActorProxy<ISimulation>(
    new ActorId("simulation"),
    nameof(SimulationActor));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); Don't use this, this middleware blocks Dapr calls

app.MapActorsHandlers();

app.MapPost("/init", () => {
    simulationProxy.InitActors();
});

app.MapPost("/increment", () => {
    simulationProxy.IncrementTime();
});

app.Run();
