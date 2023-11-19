
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

app.MapPost("/init", async () => {
    await simulationProxy.InitActorsAsync();
});

app.MapPost("/increment", async () => {
    await simulationProxy.IncrementTimeAsync();
});

app.Run();
