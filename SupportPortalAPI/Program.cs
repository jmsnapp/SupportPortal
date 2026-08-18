using Microsoft.EntityFrameworkCore;
using SupportPortalAPI.Filters;
using SupportPortalAPI.Validation;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Repositories;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Register DB context (replace connection string name as needed)
builder.Services.AddDbContext<SupportPortalDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories from infrastructure
builder.Services.AddRepositories();

// Add a permissive CORS policy for development to allow the Blazor frontend and other clients to call the API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("SupportPortalCors", policy =>
    {
        // Permissive for development: allow any origin, header and method.
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers(options =>
{
    options.ModelMetadataDetailsProviders.Add(new SkipNestedPortalObjectValidation());
    options.Filters.Add<DbUpdateExceptionFilter>();
});
builder.Services.AddOpenApi();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = Text.Plain;

            await context.Response.WriteAsync("An unexpected error occurred.");

        });
    });
}

app.UseHttpsRedirection();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    // Enable the permissive CORS policy in development so the UI can call the API.
    app.UseCors("SupportPortalCors");
    app.MapOpenApi();
}

//app.UseAuthorization();
app.MapControllers();
app.Run();

// Exposed so the integration tests can boot the real pipeline via WebApplicationFactory.
// Top-level statements generate an internal Program; this makes it addressable.
public partial class Program { }
