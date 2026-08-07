using SupportPortalUI.Components;
using SupportPortalUI.ApiClients;
using SupportPortalUI.ApiClients.Interfaces;
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

// Register typed API clients using SupportPortalApi:BaseUrl from configuration (fallback to localhost)
var apiBase = builder.Configuration["SupportPortalApi:BaseUrl"] ?? "http://localhost:5239/";
// Register our retry handler and attach it to the typed clients
builder.Services.AddTransient<SupportPortalUI.Http.RetryHandler>();

builder.Services.AddHttpClient<IEscalationsApiClient, EscalationsApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
builder.Services.AddHttpClient<ITicketsApiClient, TicketsApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
builder.Services.AddHttpClient<IProjectsApiClient, ProjectsApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
builder.Services.AddHttpClient<IIntegrationsApiClient, IntegrationsApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
// Additional ApiClients (notes, link phases, supporting refs)
builder.Services.AddHttpClient<ITicketNotesApiClient, TicketNotesApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
builder.Services.AddHttpClient<IProjectNotesApiClient, ProjectNotesApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
builder.Services.AddHttpClient<ILinkProjectPhasesApiClient, LinkProjectPhasesApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();

builder.Services.AddHttpClient<ICustomersApiClient, CustomersApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
builder.Services.AddHttpClient<IIndustriesApiClient, IndustriesApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
builder.Services.AddHttpClient<IIntegrationStatusesApiClient, IntegrationStatusesApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
builder.Services.AddHttpClient<IIntegrationTypesApiClient, IntegrationTypesApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
builder.Services.AddHttpClient<IPhasesApiClient, PhasesApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
builder.Services.AddHttpClient<ISeveritiesApiClient, SeveritiesApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();
builder.Services.AddHttpClient<ISupportStatusesApiClient, SupportStatusesApiClient>(c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<SupportPortalUI.Http.RetryHandler>();

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
