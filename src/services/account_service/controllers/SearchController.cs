using System.Text.Json;
using account_service.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;

namespace account_service.controllers;


[ApiController]
[Route("/api/accounts/find/")]
public class SearchController(IDistributedCache session, 
                                NpgsqlConnection connection) : Controller {
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;

    [HttpGet("student")]
    public async Task<IActionResult> FindStudent([FromBody] Query query) {
        var result = await CheckProfile(Role.Student.ToString());
        if(!result.Item1) return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();

        if(!ValidateQuery(query.Keywords)) return BadRequest();

        string keywords = FormatQuery(query.Keywords);

        try {

            string sqlQuery = "SELECT id, internid, name, email, address, phone, birthdate, role, docid "+
                                "FROM Student "+
                                $"WHERE internid = ($1) or name ILIKE '%{keywords}%';";


            NpgsqlCommand cmd = new (sqlQuery, _connection){
                Parameters = {
                    new() {Value = keywords}
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) return NotFound();

            await reader.ReadAsync();

            StudentModel student = new(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetFieldValue<DateOnly>(6),
                "",
                reader.GetString(7),
                reader.GetString(8)
            );

            if(reader.ReadAsync().Result) return Conflict();

            return Ok(student);

        } catch(Exception e) {

            throw new Exception(e.ToString());
        }
    }


    [HttpGet("teacher")]
    public async Task<IActionResult> FindTeacher([FromBody] Query query) {
        var result = await CheckProfile(Role.Teacher.ToString());
        if(!result.Item1) return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();

        if(!ValidateQuery(query.Keywords)) return BadRequest();

        string keywords = FormatQuery(query.Keywords);

        try {

            string sqlQuery = "SELECT id, internid, name, email, address, phone, birthdate, role, docid, academicLevel, course "+
                                "FROM Teacher "+
                                $"WHERE internid = ($1) or name ILIKE '%{keywords}%';";


            NpgsqlCommand cmd = new (sqlQuery, _connection){
                Parameters = {
                    new() {Value = keywords}
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) return NotFound();

            await reader.ReadAsync();

            TeacherModel teacher = new(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetFieldValue<DateOnly>(6),
                "",
                reader.GetString(7),
                reader.GetString(8),
                reader.GetString(9),
                reader.GetString(10)
            );

            if(reader.ReadAsync().Result) return Conflict();

            return Ok(teacher);

        } catch(Exception e) {

            throw new Exception(e.ToString());
        }
    }


    [HttpGet("secretary")]
    public async Task<IActionResult> FindSecretary([FromBody] Query query) {
        var result = await CheckProfile(Role.Secretary.ToString());
        if(!result.Item1) return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();

        if(!ValidateQuery(query.Keywords)) return BadRequest();

        string keywords = FormatQuery(query.Keywords);


        try {

            string sqlQuery = "SELECT id, internid, name, email, address, phone, birthdate, role, docid, position, building_id "+
                                "FROM Secretary "+
                                $"WHERE internid = ($1) or name ILIKE '%{keywords}%';";


            NpgsqlCommand cmd = new (sqlQuery, _connection){
                Parameters = {
                    new() {Value = keywords}
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) return NotFound();

            await reader.ReadAsync();

            SecretaryModel secretary = new(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetFieldValue<DateOnly>(6),
                "",
                reader.GetString(7),
                reader.GetString(8),
                reader.GetString(9),
                reader.GetInt32(10)
            );

            if(reader.ReadAsync().Result) return Conflict();

            return Ok(secretary);

        } catch(Exception e) {

            throw new Exception(e.ToString());
        }
    }

    [HttpGet("helpdesk")]
    public async Task<IActionResult> FindHelpdesk([FromBody] Query query) {
        var result = await CheckProfile(Role.Helpdesk.ToString());
        if(!result.Item1) return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();

        if(!ValidateQuery(query.Keywords)) return BadRequest();

        string keywords = FormatQuery(query.Keywords);

        try {

            string sqlQuery = "SELECT id, internid, name, email, address, phone, birthdate, role, docid "+
                                "FROM \"User\" "+
                                $"WHERE internid = ($1) or name ILIKE '%{keywords}%';";

            NpgsqlCommand cmd = new (sqlQuery, _connection){
                Parameters = {
                    new() {Value = keywords}
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) return NotFound();

            await reader.ReadAsync();

            UserModel helpdesk = new(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetString(5),
                reader.GetFieldValue<DateOnly>(6),
                "",
                reader.GetString(7),
                reader.GetString(8)
            );

            if(reader.ReadAsync().Result) return Conflict();

            return Ok(helpdesk);

        } catch(Exception e) {

            throw new Exception(e.ToString());
        }
    }


    private async Task<(bool, int)> CheckProfile (string newUserRole) {

        try {

            string? sid = HttpContext.Request.Cookies["connect.sid"];
            if(string.IsNullOrWhiteSpace(sid)) return (false, -1);

            string? jsonData = await _session.GetStringAsync(sid);

            if(string.IsNullOrWhiteSpace(jsonData)) {
                HttpContext.Response.Cookies.Delete("connect.sid");
                return (false, SESSION_EXPIRED_CODE);
            }

            UserData? user = JsonSerializer.Deserialize<UserData>(jsonData) ?? throw new Exception();

            switch(newUserRole) {
                case "Student": 
                    if(string.Equals(user.Role, "Secretary") 
                    || string.Equals(user.Role, "Helpdesk")
                    || string.Equals(user.Role, "Admin")) return (true, SUCCESS_CODE);
                break;

                case "Teacher": 
                    if(string.Equals(user.Role, "Helpdesk")
                    || string.Equals(user.Role, "Admin")) return (true, SUCCESS_CODE);
                break;

                case "Secretary": 
                    if(string.Equals(user.Role, "Helpdesk")
                    || string.Equals(user.Role, "Admin")) return (true, SUCCESS_CODE);
                break;

                case "Helpdesk": 
                    if(string.Equals(user.Role, "Admin")) return (true, SUCCESS_CODE);
                break;
            }

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