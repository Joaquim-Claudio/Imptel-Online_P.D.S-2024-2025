using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.MapGet("/", () => "Hello World!");


app.UseRouting();
app.MapControllers();

app.Run();
