using System.Text.Json;
using auxiliar_service.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;

namespace auxiliar_service.controllers.classes;

[ApiController]
[Route("/api/auxiliar/classes/")]
public class CreateController (NpgsqlConnection connection,
                                IDistributedCache session) : Controller {
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;


    [HttpPost("create")]
    public async Task<IActionResult> CreateClass([FromBody] ClassModel _class) {

        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await CheckProfile();
        if(!result.Item1){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/auxiliar/classes/create {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired.") : Unauthorized();
        }

        if(!ValidateData(_class)) {
            
            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/auxiliar/classes/create {protocol}\" 400");
            return BadRequest();
        }

        try {
            string query =  "INSERT INTO \"Class\" (name, roomid) "+
                            "VALUES ( ($1), ($2)) "+
                            "RETURNING *;";
            
            //FIXME: Remove
            Console.WriteLine("\n" + query + "\n");

            var cmd = new NpgsqlCommand(query, _connection) {
                Parameters = {
                    new () {Value = _class.Name},
                    new() {Value = _class.RoomId}
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            if(!reader.HasRows) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/auxiliar/classes/create {protocol}\" 404");
                return NotFound();
            }
            
            await reader.ReadAsync();

            ClassModel newClass = new(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2)
            );

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/auxiliar/classes/create {protocol}\" 201");
            return Created("/", newClass);

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


    private static bool ValidateData(ClassModel _class) {
        if(_class.Name.GetType() != typeof(string)) return false;
        if(_class.RoomId.GetType() != typeof(string)) return false;

        return true;
    }
}