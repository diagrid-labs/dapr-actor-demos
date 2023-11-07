
using ActorDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<GameActor>();
    options.Actors.RegisterActor<HeroActor>();
    options.Actors.RegisterActor<ZombieActor>();
    options.Actors.RegisterActor<PositionsActor>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection(); Don't use this, this middleware blocks Dapr calls
app.UseStaticFiles();

app.UseRouting();

// app.UseAuthorization();

// app.MapRazorPages();

app.Run();
