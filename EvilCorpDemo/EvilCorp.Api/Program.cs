using Microsoft.AspNetCore.Mvc;
using Dapr.Actors;
using Dapr.Actors.Client;
using IO.Ably;
using EvilCorp.Interfaces;
using EvilCorp.Web;

var builder = WebApplication.CreateBuilder(args);

var httpClient = new HttpClient {
    BaseAddress = new Uri("http://localhost:3500/v1.0/")
};
var response = await httpClient.GetAsync("secrets/localsecretstore/AblyApiKey");
var secrets = await response.Content.ReadFromJsonAsync<Secrets>();
builder.Services.AddSingleton<IRestClient>(new AblyRest(secrets?.AblyApiKey));
builder.Services.AddSingleton<IRealtimeNotification, RealtimeNotification>();

// Add services to the container.
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<HeadQuartersActor>();
    options.Actors.RegisterActor<RegionalOfficeActor>();
    options.Actors.RegisterActor<AlarmClockActor>();
    options.Actors.RegisterActor<EmployeeActor>();
    options.Actors.RegisterActor<SimulationActor>();
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

app.MapPost("/init/{employeeCount}", async ([FromRoute]int employeeCount) => {
    await simulationProxy.InitActorsAsync(employeeCount);
});

app.MapPost("/start", async () => {
    await simulationProxy.StartTimeAsync();
});

app.Run();

class Secrets
{
    public string? AblyApiKey { get; set; }
}