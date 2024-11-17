using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddMvc().
    AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

string? db_username = builder.Configuration.GetValue<string>("DB_USER");
string? db_password = builder.Configuration.GetValue<string>("DB_PASSWORD");
string? db_host = builder.Configuration.GetValue<string>("DB_HOST");
string? db_port = builder.Configuration.GetValue<string>("DB_PORT");
string? db_name = builder.Configuration.GetValue<string>("DB_NAME");


string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?.Replace("{USERNAME}", db_username)
    ?.Replace("{PASSWORD}", db_password)
    ?.Replace("{HOST}", db_host)
    ?.Replace("{PORT}", db_port)
    ?.Replace("{DATABASE}", db_name);


builder.Services.AddSingleton<NpgsqlConnection>( provider => {
    var connection = new NpgsqlConnection(connectionString);
    connection.Open();
    return connection;
});


string? redis_host = builder.Configuration.GetValue<string>("REDIS_HOST");
string? redis_port = builder.Configuration.GetValue<string>("REDIS_PORT");
string? redis_password = builder.Configuration.GetValue<string>("REDIS_PASSWORD");

string? redisConnectionString = builder.Configuration.GetConnectionString("SessionRedis")
    ?.Replace("{HOST}", redis_host)
    ?.Replace("{PORT}", redis_port)
    ?.Replace("{PASSWORD}", redis_password);

builder.Services.AddStackExchangeRedisCache(options => 
    options.Configuration = redisConnectionString);

builder.Services.AddSession(options => 
{
    options.IdleTimeout = TimeSpan.FromHours(48);
    options.Cookie.HttpOnly = true;
    options.Cookie.Name = "connect.sid";
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.UseRouting();

app.UseSession();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.MapControllers();

app.Run();
