using Microsoft.EntityFrameworkCore;
using SupportPortalDomain;
using SupportPortalInfrastructure.Data;
using SupportPortalInfrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Register DB context (replace connection string name as needed)
builder.Services.AddDbContext<SupportPortalDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories from infrastructure
builder.Services.AddRepositories();

// Register the Mapper from SupportPortalInfrastructure
builder.Services.AddScoped<DBMapper>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
