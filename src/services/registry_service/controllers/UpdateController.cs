using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using registry_service.models;

namespace registry_service.controllers;

[ApiController]
[Route("/api/registries/")]
public class UpdateController (NpgsqlConnection connection,
                                IDistributedCache session) : Controller {
    
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;

    [HttpPut("update")]
    public async Task<IActionResult> UpdateRegistry([FromBody] RegistryData registry) {
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await CheckProfile();
        if(!result.Item1) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/registries/update {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();
        }

        if(!ValidateData(registry)) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/registries/update {protocol}\" 400");
            return BadRequest();
        }

        try {
            string query =  "UPDATE Registry as r1 "+
                            $"SET status = '{registry.Status}', approved = ($2), building_id = ($3), update_date = ($4) "+
                            "WHERE id = ($1);";  

            //FIXME: Remove
            Console.WriteLine("\n" + query + "\n");

            var cmd = new NpgsqlCommand(query, _connection){
                Parameters = {
                    new() {Value = registry.Id},
                    new() {Value = registry.Approved},
                    new() {Value = registry.BuildingId},
                    new() {Value = registry.UpdateDate}
                }
            };

            await cmd.ExecuteNonQueryAsync();

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/registries/update {protocol}\" 204");
            return NoContent();

        } catch (Exception e) {

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

    private static bool ValidateData(RegistryData registry) {
        if(registry.StudentId.GetType() != typeof(int)) return false;
        if(registry.EnrollmentId.GetType() != typeof(int)) return false;
        if(registry.BuildingId.GetType() != typeof(int)) return false;
        if(registry.Date.GetType() != typeof(DateOnly)) return false;
        if(!Enum.IsDefined(typeof(Status), registry.Status)) return false;

        return true;
    }

}