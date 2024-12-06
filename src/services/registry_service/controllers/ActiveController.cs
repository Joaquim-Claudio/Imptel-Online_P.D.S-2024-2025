using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using registry_service.models;

namespace registry_service.controllers;

[ApiController]
[Route("/api/registries/")]
public class ActiveController(NpgsqlConnection connection,
                                IDistributedCache session) : Controller {
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;

    [HttpGet("active")]
    public async Task<IActionResult> FindAll() {

        // Log parameters
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await CheckProfile();
        if(!result.Item1){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/registries/active {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired.") : Unauthorized();
        }

        try {
            string? _current_year = GetCurrentYear().Result;
            string query = "SELECT r1.id, c2.name, c1.name, s1.name, s1.internid "+
                            "FROM Registry AS r1 "+
                            "INNER JOIN Student AS s1 ON r1.student_id = s1.id "+
                            "INNER JOIN Enrollment AS e1 ON r1.enrollment_id = e1.id "+
                            "INNER JOIN \"Class\" AS c1 ON e1.class_id = c1.id "+
                            "INNER JOIN Course AS c2 ON e1.course_id = c2.id "+
                            "WHERE e1.acadyear = ($1) "+
                            "GROUP BY r1.id, c2.name, c1.name, s1.name, s1.internid "+
                            "ORDER BY c2.name, c1.name, s1.name";
            //FIXME: Remove
            Console.WriteLine("\n" + query + "\n");

            NpgsqlCommand cmd = new(query, _connection){
                Parameters = {
                    new() {Value = _current_year}
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            if(!reader.HasRows) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/registries/active {protocol}\" 404");
                return NotFound();
            }

            List<SimpleRegistryData> registries = [];

            while (reader.ReadAsync().Result) {
                SimpleRegistryData row = new(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4));

                registries.Add(row);
            }

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/registries/active {protocol}\" 200");
            return Ok(registries);


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

    private async Task<string> GetCurrentYear() {

        return await _session.GetStringAsync("CURRENT_ACAD_YEAR") ?? throw new Exception("Failed to retrieve session data.");
    }


}