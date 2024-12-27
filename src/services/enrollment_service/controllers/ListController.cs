using System.Text.Json;
using enrollment_service.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;

namespace enrollment_service.controllers;

[ApiController]
[Route("/api/enrollments/")]
public class ListController(NpgsqlConnection connection,
                            IDistributedCache session) : Controller {
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;


    [HttpGet("currentyear")]
    public async Task<IActionResult> FindAll() {
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await CheckProfile();
        if(!result.Item1){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/enrollments/currentyear {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired.") : Unauthorized();
        }

        try {
            string? current_year = await _session.GetStringAsync("CURRENT_ACAD_YEAR");

            if(string.IsNullOrWhiteSpace(current_year)) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/enrollments/currentyear {protocol}\" 404");
                return NotFound("Failed to retrieve 'current year'. Contact the support team.");
            }

            string query = "SELECT c1.id, c1.name, c1.duration, "+
                                "c2.id, c2.name, c2.roomid, "+
                                "e1.id, e1.level, e1.acadyear "+
                            "FROM Enrollment AS e1 "+
                            "INNER JOIN course AS c1 ON e1.course_id = c1.id "+
                            "INNER JOIN \"Class\" AS c2 ON e1.class_id = c2.id "+
                            "WHERE e1.acadyear = ($1); ";

            //FIXME: Remove
            Console.WriteLine("\n" + query + "\n");


            var cmd = new NpgsqlCommand(query, _connection){
                Parameters = {
                    new() {Value = current_year}
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            if(!reader.HasRows) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/enrollments/currentyear {protocol}\" 404");
                return NotFound();
            }

            List<EnrollmentModel> enrollments = [];

            while(reader.ReadAsync().Result) {
                CourseModel course = new(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2));
                ClassModel _class = new(reader.GetInt32(3), reader.GetString(4), reader.GetString(5));
                EnrollmentModel enrollment = new(
                    reader.GetInt32(6), 
                    reader.GetString(7), 
                    reader.GetString(8),
                    _class,
                    course);
                
                enrollments.Add(enrollment);
            }

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/enrollments/currentyear {protocol}\" 200");
            return Ok(enrollments);


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