using Microsoft.AspNetCore.Mvc;
using Npgsql;
using auxiliar_service.models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace auxiliar_service.controllers;

[ApiController]
[Route("/api/auxiliar/")]
public class GetAcadYears(NpgsqlConnection connection,
                            IDistributedCache session) : Controller {
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;


    [HttpGet("acadyears")]
    public async Task<IActionResult> GetAllAcadYears() {

        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await CheckProfile();
        if(!result.Item1){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/auxiliar/acadyears {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired.") : Unauthorized();
        }

        try {
            string query = "SELECT * FROM Acadyear ORDER BY name DESC";
            
            //FIXME: Remove
            Console.WriteLine("\n" + query + "\n");

            var cmd = new NpgsqlCommand(query, _connection);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            if(!reader.HasRows) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/auxiliar/acadyears {protocol}\" 404");
                return NotFound();
            }

            List<AcadYear> years = [];

            while(reader.ReadAsync().Result) {
                AcadYear acadYear = new (
                    reader.GetInt32(0), 
                    reader.GetString(1), 
                    reader.GetFieldValue<DateOnly>(2),
                    !reader.IsDBNull(3) ? reader.GetFieldValue<DateOnly>(3) : null
                );

                years.Add(acadYear);
            }

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/auxiliar/acadyears {protocol}\" 200");
            return Ok(years);


        } catch(Exception e) {

            throw new Exception(e.ToString());
        }

    }


    private async Task<(bool, int)> CheckProfile () {

        try {

            string? sid = HttpContext.Request.Cookies["connect.sid"];
            if(string.IsNullOrWhiteSpace(sid)) return (false, -1);

            string? jsonData = await _session.GetStringAsync(sid);

            if(string.IsNullOrWhiteSpace(jsonData)) {
                HttpContext.Response.Cookies.Delete("connect.sid");
                return (false, SESSION_EXPIRED_CODE);
            }

            UserData? user = JsonSerializer.Deserialize<UserData>(jsonData) ?? throw new Exception();

            if(string.Equals(user.Role, "Secretary") 
            || string.Equals(user.Role, "Helpdesk")
            || string.Equals(user.Role, "Admin")) return (true, SUCCESS_CODE);



            return (false, -1);

        } catch(Exception e) {

            Console.WriteLine("Failed to retrieve session data.");
            throw new Exception(e.ToString());
        }
        
        
    }
}