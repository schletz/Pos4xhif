using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProcessDemo.Model;
using ProcessDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<QueuedWorker>(provider =>
    new QueuedWorker(
        provider.GetRequiredService<IServiceScopeFactory>(),
        provider.GetRequiredService<ILogger<QueuedWorker>>(),
        maxQueueLength: 4,
        maxProcesses: 2,
        timeout: 30000
));
builder.Services.AddDbContext<PingContext>(opt =>
{
    opt.UseSqlite("Data Source=PingResults.db");
});

builder.Services.AddRazorPages();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
using (var db = scope.ServiceProvider.GetRequiredService<PingContext>())
{
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}
app.Run();
