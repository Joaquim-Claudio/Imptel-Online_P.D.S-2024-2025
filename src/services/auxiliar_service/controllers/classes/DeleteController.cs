using System.Text.Json;
using auxiliar_service.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;

namespace auxiliar_service.controllers.classes;

[ApiController]
[Route("/api/auxiliar/classes/")]
public class DeleteController (NpgsqlConnection connection,
                                IDistributedCache session) : Controller {
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;


    [HttpDelete("delete/{classId}")]
    public async Task<IActionResult> DeleteClass(int classId) {

        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await CheckProfile();
        if(!result.Item1){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"DELETE /api/auxiliar/classes/delete {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired.") : Unauthorized();
        }

        if(!ValidateData(classId)) {
            
            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"DELETE /api/auxiliar/classes/delete {protocol}\" 400");
            return BadRequest();
        }

        try {
            string query =  "DELETE FROM \"Class\" "+
                            "WHERE id = ($1);";

            
            //FIXME: Remove
            Console.WriteLine("\n" + query + "\n");

            var cmd = new NpgsqlCommand(query, _connection) {
                Parameters = {
                    new () {Value = classId}
                }
            };

            await cmd.ExecuteNonQueryAsync();
            
            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"DELETE /api/auxiliar/classes/delete {protocol}\" 200");
            return Ok();

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

            if(string.Equals(user.Role, "Admin")) return (true, SUCCESS_CODE);

            return (false, -1);

        } catch(Exception e) {

            Console.WriteLine("Failed to retrieve session data.");
            throw new Exception(e.ToString());
        }
        
    }


    private static bool ValidateData(int id) {
        if(id.GetType() != typeof(int)) return false;

        return true;
    }
}