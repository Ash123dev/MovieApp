using Hangfire;
using Microsoft.EntityFrameworkCore;
using MovieApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// Register MovieService
builder.Services.AddScoped<MovieService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Use Hangfire Dashboard
app.UseHangfireDashboard();

// Register recurring job
using (var scope = app.Services.CreateScope())
{
    var movieService = scope.ServiceProvider.GetRequiredService<MovieService>();

    // Ensure the job runs for the first time synchronously during startup
    await movieService.FetchAndStoreMoviesAsync();

    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<MovieService>(
        "fetch-movies",
        service => service.FetchAndStoreMoviesAsync(),
        Cron.Hourly
    );
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.Run();
