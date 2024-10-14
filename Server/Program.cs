using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using music_manager_starter.Data;
using music_manager_starter.Server.Hubs;
using System.Security.AccessControl;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<DataDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!app.Environment.EnvironmentName.Equals("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DataDbContext>();
        dbContext.Database.Migrate();
    }
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapHub<SongHub>("/songhub");
app.MapFallbackToFile("index.html");

app.Run();

// Added this to expose it to integration tests.
public partial class Program { }