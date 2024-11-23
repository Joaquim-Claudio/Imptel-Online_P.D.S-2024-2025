using System.Text.Json;
using account_service.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;

namespace account_service.controllers;

[ApiController]
[Route("/api/accounts/create/")]
public class CreateController(IDistributedCache session,
                                NpgsqlConnection connection) : Controller {

    private readonly string SECRETARY_PREFIX = "3000";
    private readonly string TEACHER_PREFIX = "5000";
    private readonly string HELPDESK_PREFIX = "1000";

    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;
    private static readonly PasswordHasher<Object> passwordService = new();

    [HttpPost("student")]
    public async Task<IActionResult> CreateStudent([FromBody] StudentModel student) {

        if(!string.Equals(student.Role.ToString(), "Student")) return BadRequest();

        if(!CheckProfile(student.Role).Result) return Unauthorized();

        if(!ValidateData(student)) return BadRequest();

        string? internId = await GenerateInternId(student.Role.ToString());
        string? password = GenerateDefaultPassword(internId);

        student.InternId = internId;
        student.HashPassword = passwordService.HashPassword(new(), password);

        string query = "INSERT INTO Student (internid, name, email, address, phone, birthdate, hashpassword, role, docid) "+
                        $"VALUES ( ($1), ($2), ($3), ($4), ($5), ($6), ($7), '{student.Role}', ($8) ) "+
                        "RETURNING id, internid, name, email, address, phone, birthdate, hashpassword, role, docid";

        // FIXME: Remove 
        Console.WriteLine(query + "\n");

        var cmd = new NpgsqlCommand(query, _connection) {
            Parameters = {
                new() {Value = student.InternId},
                new() {Value = student.Name},
                new() {Value = student.Email},
                new() {Value = student.Address},
                new() {Value = student.Phone},
                new() {Value = student.BirthDate},
                new() {Value = student.HashPassword},
                new() {Value = student.DocId},
            }
        };

        NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();


        StudentModel newStudent = new(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.GetString(5),
            reader.GetFieldValue<DateOnly>(6),
            reader.GetString(7),
            reader.GetString(8),
            reader.GetString(9)
        );

        await reader.CloseAsync();
        return Created("/", newStudent);
    }



    [HttpPost("teacher")]
    public async Task<IActionResult> CreateTeacher([FromBody] TeacherModel teacher) {

        if(!string.Equals(teacher.Role.ToString(), "Teacher")) return BadRequest();

        if(!CheckProfile(teacher.Role).Result) return Unauthorized();

        if(!ValidateData(teacher)) return BadRequest();

        string? internId = await GenerateInternId(teacher.Role.ToString());
        string? password = GenerateDefaultPassword(internId);

        teacher.InternId = internId;
        teacher.HashPassword = passwordService.HashPassword(new(), password);

        string query = "INSERT INTO Teacher (internid, name, email, address, phone, birthdate, hashpassword, role, docid, academicLevel, course) "+
                        $"VALUES ( ($1), ($2), ($3), ($4), ($5), ($6), ($7), '{teacher.Role}', ($8), ($9), ($10)  ) "+
                        "RETURNING id, internid, name, email, address, phone, birthdate, hashpassword, role, docid, academicLevel, course";

        // FIXME: Remove 
        Console.WriteLine(query + "\n");

        var cmd = new NpgsqlCommand(query, _connection) {
            Parameters = {
                new() {Value = teacher.InternId},
                new() {Value = teacher.Name},
                new() {Value = teacher.Email},
                new() {Value = teacher.Address},
                new() {Value = teacher.Phone},
                new() {Value = teacher.BirthDate},
                new() {Value = teacher.HashPassword},
                new() {Value = teacher.DocId},
                new() {Value = teacher.AcademicLevel},
                new() {Value = teacher.Course},
            }
        };

        NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();


        TeacherModel newTeacher = new(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.GetString(5),
            reader.GetFieldValue<DateOnly>(6),
            reader.GetString(7),
            reader.GetString(8),
            reader.GetString(9),
            reader.GetString(10),
            reader.GetString(11)
        );

        await reader.CloseAsync();
        return Created("/", newTeacher);
    }


    [HttpPost("secretary")]
    public async Task<IActionResult> CreateSecretary([FromBody] SecretaryModel secretary) {

        if(!string.Equals(secretary.Role.ToString(), "Secretary")) return BadRequest();

        if(!CheckProfile(secretary.Role).Result) return Unauthorized();

        if(!ValidateData(secretary)) return BadRequest();

        string? internId = await GenerateInternId(secretary.Role.ToString());
        string? password = GenerateDefaultPassword(internId);

        secretary.InternId = internId;
        secretary.HashPassword = passwordService.HashPassword(new(), password);

        string query = "INSERT INTO Secretary (internid, name, email, address, phone, birthdate, hashpassword, role, docid, position, building_id) "+
                        $"VALUES ( ($1), ($2), ($3), ($4), ($5), ($6), ($7), '{secretary.Role}', ($8), ($9), ($10) ) "+
                        "RETURNING id, internid, name, email, address, phone, birthdate, hashpassword, role, docid, position, building_id";

        // FIXME: Remove 
        Console.WriteLine(query + "\n");

        var cmd = new NpgsqlCommand(query, _connection) {
            Parameters = {
                new() {Value = secretary.InternId},
                new() {Value = secretary.Name},
                new() {Value = secretary.Email},
                new() {Value = secretary.Address},
                new() {Value = secretary.Phone},
                new() {Value = secretary.BirthDate},
                new() {Value = secretary.HashPassword},
                new() {Value = secretary.DocId},
                new() {Value = secretary.Position},
                new() {Value = secretary.BuildingId},
            }
        };

        NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();

        SecretaryModel newSecretary = new(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.GetString(5),
            reader.GetFieldValue<DateOnly>(6),
            reader.GetString(7),
            reader.GetString(8),
            reader.GetString(9),
            reader.GetString(10),
            reader.GetInt32(11)
        );

        await reader.CloseAsync();
        return Created("/", newSecretary);
    }


    [HttpPost("helpdesk")]
    public async Task<IActionResult> CreateHelpdesk([FromBody] UserModel user) {

        if(!string.Equals(user.Role.ToString(), "Helpdesk")) return BadRequest();

        if(!CheckProfile(user.Role).Result) return Unauthorized();

        if(!ValidateData(user)) return BadRequest();

        string? internId = await GenerateInternId(user.Role.ToString());
        string? password = GenerateDefaultPassword(internId);

        user.InternId = internId;
        user.HashPassword = passwordService.HashPassword(new(), password);

        string query = "INSERT INTO \"User\" (internid, name, email, address, phone, birthdate, hashpassword, role, docId) "+
                        $"VALUES ( ($1), ($2), ($3), ($4), ($5), ($6), ($7), '{user.Role}', ($8)) "+
                        "RETURNING id, internid, name, email, address, phone, birthdate, hashpassword, role, docId";

        // FIXME: Remove 
        Console.WriteLine(query + "\n");

        var cmd = new NpgsqlCommand(query, _connection) {
            Parameters = {
                new() {Value = user.InternId},
                new() {Value = user.Name},
                new() {Value = user.Email},
                new() {Value = user.Address},
                new() {Value = user.Phone},
                new() {Value = user.BirthDate},
                new() {Value = user.HashPassword},
                new() {Value = user.DocId},
            }
        };

        NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();


        UserModel newUser = new(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.GetString(5),
            reader.GetFieldValue<DateOnly>(6),
            reader.GetString(7),
            reader.GetString(8),
            reader.GetString(9)
        );

        await reader.CloseAsync();
        return Created("/", newUser);
    }


    private async Task<bool> CheckProfile (string newUserRole) {
        
        string? sid = HttpContext.Request.Cookies["connect.sid"];
        if(string.IsNullOrWhiteSpace(sid)) return false;

        string? jsonData = await _session.GetStringAsync(sid);

        if(string.IsNullOrWhiteSpace(jsonData)) {
            HttpContext.Response.Cookies.Delete("connect.sid");
            return false;
        }

        UserData? user = JsonSerializer.Deserialize<UserData>(jsonData) ?? throw new Exception();

        switch(newUserRole) {
            case "Student": 
                if(string.Equals(user.Role, "Secretary") 
                || string.Equals(user.Role, "Helpdesk")
                || string.Equals(user.Role, "Admin")) return true;
            break;

            case "Teacher": 
                if(string.Equals(user.Role, "Helpdesk")
                || string.Equals(user.Role, "Admin")) return true;
            break;

            case "Secretary": 
                if(string.Equals(user.Role, "Helpdesk")
                || string.Equals(user.Role, "Admin")) return true;
            break;

            case "Helpdesk": 
                if(string.Equals(user.Role, "Admin")) return true;
            break;
        }

        return false;
    }


    private static bool ValidateData(UserModel user) {
        if(user.Id != null) return false;
        if(!string.IsNullOrWhiteSpace(user.InternId)) return false;
        if(!string.IsNullOrWhiteSpace(user.HashPassword)) return false;
        if(string.IsNullOrWhiteSpace(user.Name)) return false;
        if(string.IsNullOrWhiteSpace(user.DocId)) return false;
        if(string.IsNullOrWhiteSpace(user.Email)) user.Email = "";
        if(string.IsNullOrWhiteSpace(user.Address)) user.Address = "";
        if(string.IsNullOrWhiteSpace(user.Phone)) user.Phone = "";

        return true;
    }


    private static string GenerateDefaultPassword(string internId) {
        string prefix= "Imptel";
        return prefix + internId;
    }



    private async Task<string> GenerateInternId(string role) {
        string internId = "";
        string? nextId;

        switch (role)
        {
            case "Student":
                string? currentYear = await _session.GetStringAsync("CURRENT_ACAD_YEAR");
                nextId = await _session.GetStringAsync("NEXT_STUDENT_SUFIX") ?? throw new Exception("Next id not found!");
                string stdNextId = (int.Parse(nextId) + 1).ToString().PadLeft(4, '0');
                await _session.SetStringAsync("NEXT_STUDENT_SUFIX", stdNextId);

                internId = currentYear + nextId;
                break;

            case "Teacher":
                nextId = await _session.GetStringAsync("NEXT_TEACHER_SUFIX") ?? throw new Exception("Next id not found!");
                string teaNextId = (int.Parse(nextId) + 1).ToString().PadLeft(4, '0');
                await _session.SetStringAsync("NEXT_TEACHER_SUFIX", teaNextId);

                internId = TEACHER_PREFIX + nextId;
                break;

            case "Secretary":
                nextId = await _session.GetStringAsync("NEXT_SECRETARY_SUFIX") ?? throw new Exception("Next id not found!");
                string secNextId = (int.Parse(nextId) + 1).ToString().PadLeft(4, '0');
                await _session.SetStringAsync("NEXT_SECRETARY_SUFIX", secNextId);

                internId = SECRETARY_PREFIX + nextId;
                break;

            case "Helpdesk":
                nextId = await _session.GetStringAsync("NEXT_HELPDESK_SUFIX") ?? throw new Exception("Next id not found!");
                string hpNextId = (int.Parse(nextId) + 1).ToString().PadLeft(4, '0');
                await _session.SetStringAsync("NEXT_HELPDESK_SUFIX", hpNextId);

                internId = HELPDESK_PREFIX + nextId;
                break;

        }

        return internId;
    }
}