using System.Text.Json;
using enrollment_service.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;

namespace enrollment_service.controllers;

[ApiController]
[Route("/api/enrollments/")]
public class UpdateController (NpgsqlConnection connection,
                                IDistributedCache session) : Controller {
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;

    
    [HttpPut("update")]
    public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentData enrollment) {
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await CheckProfile();
        if(!result.Item1){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/enrollments/update {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired.") : Unauthorized();
        }

        if(!ValidateData(enrollment)) {
            
            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/enrollments/update {protocol}\" 400");
            return BadRequest();
        }


        try {

            string query =  "UPDATE Enrollment "+
                            $"SET class_id = ($2), course_id = ($3), level = '{enrollment.Level}' "+
                            "WHERE id = ($1); ";

            //FIXME: Remove
            Console.WriteLine("\n" + query + "\n");

            var cmd = new NpgsqlCommand(query, _connection){
                Parameters = {
                    new() {Value = enrollment.Id},
                    new() {Value = enrollment.ClassId},
                    new() {Value = enrollment.CourseId}
                }
            };

            await cmd.ExecuteNonQueryAsync();

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/enrollments/update {protocol}\" 204");
            return NoContent();


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

    private static bool ValidateData(EnrollmentData enrollment) {
        if(enrollment.ClassId.GetType() != typeof(int)) return false;
        if(enrollment.CourseId.GetType() != typeof(int)) return false;
        if(enrollment.AcadYear.GetType() != typeof(string)) return false;
        if(enrollment.Level.GetType() != typeof(string)) return false;

        return true;
    }
}