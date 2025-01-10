using System.Text.Json;
using invoice_service.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;

namespace invoice_service.controllers;


[ApiController]
[Route("/api/invoices/")]
public class FindController(NpgsqlConnection connection, 
                            IDistributedCache session) : Controller{
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;

    [HttpPost("find")]
    public async Task<IActionResult> FindActive([FromBody] Query query) {
        // Log parameters
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await CheckProfile();
        if(!result.Item1){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/invoices/find {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired.") : Unauthorized();
        }

        if(!ValidateQuery(query.Keywords)){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/invoices/find {protocol}\" 400");
            return BadRequest();
        }

        string keywords = FormatQuery(query.Keywords);
        StudentModel? student = await GetStudent(keywords);

        if(student == null) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/invoices/find {protocol}\" 404");
            return NotFound("Student not found.");
        }

        try {
            string sqlQuery = "";

        } catch(Exception e) {

            throw new Exception(e.ToString());
        }
        
        return Ok();
    }


    private async Task<StudentModel?> GetStudent(string keywords) {

        try {
            string query = "SELECT s1.id, s1.internid, s1.name "+
                            "FROM student AS s1 "+
                            "WHERE s1.internid = ($1) or s1.name ILIKE ($2);";

            //FIXME: Remove
            Console.WriteLine("\n" + query + "\n");

            NpgsqlCommand cmd = new (query, _connection){
                Parameters = {
                    new() {Value = keywords},
                    new() {Value = $"%{keywords}%"}
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            if(!reader.HasRows) {
                return null;
            }

            await reader.ReadAsync();

            StudentModel student = new(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2)
            );

            return student;
            
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

    //TODO: implement validation to prevent SQL Inject attacks
    private static bool ValidateQuery(string query) {
        if(string.IsNullOrWhiteSpace(query)) return false;

        return true;
    }

    private static string FormatQuery(string query) {
        return query.Replace(" ", "%");
    }
}