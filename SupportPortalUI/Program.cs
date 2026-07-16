using SupportPortalUI.Components;
using Microsoft.EntityFrameworkCore;
using SupportPortalInfrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register EF Core DbContext.
// This reads the connection string from the environment variable 'SUPPORTPORTAL_CONNECTION'.
// Note: per your instruction I did NOT create the env var on the machine.
var connection = Environment.GetEnvironmentVariable("SUPPORTPORTAL_CONNECTION");
builder.Services.AddDbContext<SupportPortalDBContext>(options =>
    options.UseSqlServer(connection));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
