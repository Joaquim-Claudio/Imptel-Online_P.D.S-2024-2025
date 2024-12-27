using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using registry_service.models;

namespace registry_service.controllers;

[ApiController]
[Route("/api/registries/")]
public class FindController(NpgsqlConnection connection,
                                IDistributedCache session) : Controller {
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;
    
    [HttpPost("find")]
    public async Task<IActionResult> FindByKeywords([FromBody] Query query){
        
         // Log parameters
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await CheckProfile();
        if(!result.Item1){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/registries/find {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired.") : Unauthorized();
        }

        if(!ValidateQuery(query.Keywords)){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/registries/find {protocol}\" 400");
            return BadRequest();
        }

        string keywords = FormatQuery(query.Keywords);
        StudentModel? student = await GetStudent(keywords);

        if(student == null) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/registries/find {protocol}\" 404");
            return NotFound("Student not found.");
        }

        try {
            string sqlQuery =   "SELECT r1.id, r1.date, r1.update_date, r1.status, r1.approved, "+
                                    "c1.id, c1.name, c1.duration, c2.id, c2.name, c2.roomid, "+
                                    "e1.id, e1.level, e1.acadyear "+
                                "FROM registry as r1 "+
                                "INNER JOIN enrollment as e1 on r1.enrollment_id=e1.id "+
                                "INNER JOIN course as c1 on e1.course_id = c1.id "+
                                "INNER JOIN \"Class\" as c2 on e1.class_id = c2.id "+
                                "WHERE r1.student_id = ($1) "+
                                "ORDER BY r1.date DESC;";

            //FIXME: Remove
            Console.WriteLine("\n" + sqlQuery + "\n");

            NpgsqlCommand cmd = new (sqlQuery, _connection){
                Parameters = {
                    new() {Value = student.Id}
                }
            };


            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<RegistryModel> registries = [];

            while(reader.ReadAsync().Result) { 
                CourseModel course = new(reader.GetInt32(5), reader.GetString(6), reader.GetInt32(7));

                ClassModel _class = new(reader.GetInt32(8), reader.GetString(9), reader.GetString(10));
                EnrollmentModel enrollment = new(reader.GetInt32(11), reader.GetString(12), reader.GetString(13), _class, course);

                RegistryModel registry = new(
                    reader.GetInt32(0),
                    reader.GetFieldValue<DateOnly>(1),
                    reader.GetFieldValue<DateOnly>(2),
                    reader.GetString(3),
                    !reader.IsDBNull(4) ? reader.GetBoolean(4) : null,
                    enrollment,
                    null
                );

                registries.Add(registry);

            }
            
            StudentRegistries studentRegistries = new(
                student.Id,
                student.InternId,
                student.Name,
                registries
            );

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/registries/find {protocol}\" 200");
            return Ok(studentRegistries);

        } catch(Exception e) {

            throw new Exception(e.ToString());
        }
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