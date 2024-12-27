using System.Text.Json;
using enrollment_service.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;

namespace enrollment_service.controllers;

[ApiController]
[Route("/api/enrollments/")]
public class CreateController(NpgsqlConnection connection,
                                IDistributedCache session) : Controller {
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;

    
    [HttpPost("create")]
    public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentData enrollment) {
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await CheckProfile();
        if(!result.Item1){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/enrollments/create {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired.") : Unauthorized();
        }

        if(!ValidateData(enrollment)) {
            
            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/enrollments/create {protocol}\" 400");
            return BadRequest();
        }


        try {
            string? current_year = await _session.GetStringAsync("CURRENT_ACAD_YEAR");

            if(string.IsNullOrWhiteSpace(current_year)) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/enrollments/create {protocol}\" 404");
                return NotFound("Failed to retrieve 'current year'. Contact the support team.");
            }

            string query =  "INSERT INTO Enrollment (class_id, course_id, acadyear, level) "+
                            $"VALUES ( ($1), ($2), ($3), '{enrollment.Level}');";

            //FIXME: Remove
            Console.WriteLine("\n" + query + "\n");

            var cmd = new NpgsqlCommand(query, _connection){
                Parameters = {
                    new() {Value = enrollment.ClassId},
                    new() {Value = enrollment.CourseId},
                    new() {Value = enrollment.AcadYear}
                }
            };

            await cmd.ExecuteNonQueryAsync();

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/enrollments/create {protocol}\" 201");
            return Created("/", null);


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