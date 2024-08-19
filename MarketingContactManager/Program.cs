using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using MarketingContactManager.SchemaFilters;
using MarketingContactManager.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<ContactModelFilter>();
});

Env.Load();
var server = Environment.GetEnvironmentVariable("DB_SERVER");
var database = Environment.GetEnvironmentVariable("DB_NAME");
var trustedConnection = Environment.GetEnvironmentVariable("DB_TRUSTED_CONNECTION");
var trustServerCertificate = Environment.GetEnvironmentVariable("DB_TRUST_SERVER_CERTIFICATE");
var connectionString = $"Server={server};Database={database};Trusted_Connection={trustedConnection};TrustServerCertificate={trustServerCertificate};";

builder.Services.AddDbContext<ContactContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
