using Microsoft.EntityFrameworkCore;
using Npgsql;
using subscription_watch.Data;
using subscription_watch.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<PostgresOptions>(
    builder.Configuration.GetSection("Database"));

var postgresOptions = builder.Configuration.GetSection("Database")
    .Get<PostgresOptions>() ?? throw new InvalidOperationException("PostgreSQL configuration not found");

var connectionString = new NpgsqlConnectionStringBuilder
{
    Host = postgresOptions.Host,
    Port = postgresOptions.Port,
    Password = postgresOptions.Password,
    Username = postgresOptions.Username,
    Database = postgresOptions.Database,
    SslMode = Enum.Parse<SslMode>(postgresOptions.SslMode, true),
    Pooling = postgresOptions.Pooling
}.ToString();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(15),
            errorCodesToAdd: null);

        if (postgresOptions.IncludeErrorDetail)
        {
            options.EnableDetailedErrors();
        }
    })
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
