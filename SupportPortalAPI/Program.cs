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

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();
app.Run();
