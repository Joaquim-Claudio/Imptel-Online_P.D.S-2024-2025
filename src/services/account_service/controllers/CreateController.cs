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
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly string SECRETARY_PREFIX = "3000";
    private readonly string TEACHER_PREFIX = "5000";
    private readonly string HELPDESK_PREFIX = "1000";

    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;
    private static readonly PasswordHasher<Object> passwordService = new();

    [HttpPost("student")]
    public async Task<IActionResult> CreateStudent([FromBody] StudentModel student) {

        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        if(!string.Equals(student.Role.ToString(), "Student")) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/student {protocol}\" 400");
            return BadRequest();
        }

        var result = await CheckProfile(student.Role.ToString());
        if(!result.Item1) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/student {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();
        }

        if(!ValidateData(student)) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/student {protocol}\" 400");
            return BadRequest();
        }

        try {

            string? internId = await GenerateInternId(student.Role.ToString());
            string? password = GenerateDefaultPassword(internId);

            student.InternId = internId;
            student.HashPassword = passwordService.HashPassword(new(), password);

            string query = "INSERT INTO Student (internid, name, email, address, phone, birthdate, hashpassword, role, docid) "+
                            $"VALUES ( ($1), ($2), ($3), ($4), ($5), ($6), ($7), '{student.Role}', ($8) ) "+
                            "RETURNING id, internid, name, email, address, phone, birthdate, role, docid";


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

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/student {protocol}\" 404");
                return NotFound();
            }
            
            await reader.ReadAsync();


            StudentModel newStudent = new(
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

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/student {protocol}\" 201");
            return Created("/", newStudent);

        } catch (Exception e) {

            throw new Exception(e.ToString());
        }

        
    }



    [HttpPost("teacher")]
    public async Task<IActionResult> CreateTeacher([FromBody] TeacherModel teacher) {

        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        if(!string.Equals(teacher.Role.ToString(), "Teacher")) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/teacher {protocol}\" 400");
            return BadRequest();
        }

        var result = await CheckProfile(teacher.Role.ToString());
        if(!result.Item1) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/teacher {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();
        }

        if(!ValidateData(teacher)) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/teacher {protocol}\" 400");
            return BadRequest();
        }

        try {

            string? internId = await GenerateInternId(teacher.Role.ToString());
            string? password = GenerateDefaultPassword(internId);

            teacher.InternId = internId;
            teacher.HashPassword = passwordService.HashPassword(new(), password);

            string query = "INSERT INTO Teacher (internid, name, email, address, phone, birthdate, hashpassword, role, docid, academicLevel, course) "+
                            $"VALUES ( ($1), ($2), ($3), ($4), ($5), ($6), ($7), '{teacher.Role}', ($8), ($9), ($10)  ) "+
                            "RETURNING id, internid, name, email, address, phone, birthdate, role, docid, academicLevel, course";


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

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/teacher {protocol}\" 404");
                return NotFound();
            }

            await reader.ReadAsync();


            TeacherModel newTeacher = new(
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

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/teacher {protocol}\" 201");
            return Created("/", newTeacher);

        } catch(Exception e) {

            throw new Exception(e.ToString());
        }
        
    }


    [HttpPost("secretary")]
    public async Task<IActionResult> CreateSecretary([FromBody] SecretaryModel secretary) {

        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        if(!string.Equals(secretary.Role.ToString(), "Secretary")) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/secretary {protocol}\" 400");
            return BadRequest();
        }

        var result = await CheckProfile(secretary.Role.ToString());
        if(!result.Item1) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/secretary {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();
        }

        if(!ValidateData(secretary)){

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/secretary {protocol}\" 400");
            return BadRequest();
        }

        try {
            string? internId = await GenerateInternId(secretary.Role.ToString());
            string? password = GenerateDefaultPassword(internId);

            secretary.InternId = internId;
            secretary.HashPassword = passwordService.HashPassword(new(), password);

            string query = "INSERT INTO Secretary (internid, name, email, address, phone, birthdate, hashpassword, role, docid, position, building_id) "+
                            $"VALUES ( ($1), ($2), ($3), ($4), ($5), ($6), ($7), '{secretary.Role}', ($8), ($9), ($10) ) "+
                            "RETURNING id, internid, name, email, address, phone, birthdate, role, docid, position, building_id";


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

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) {
                
                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/secretary {protocol}\" 404");
                return NotFound();
            }

            await reader.ReadAsync();

            SecretaryModel newSecretary = new(
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

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/secretary {protocol}\" 201");
            return Created("/", newSecretary);

        } catch(Exception e) {

            throw new Exception(e.ToString());
        }
    }


    [HttpPost("helpdesk")]
    public async Task<IActionResult> CreateHelpdesk([FromBody] UserModel helpdesk) {

        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        if(!string.Equals(helpdesk.Role.ToString(), "Helpdesk")) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/helpdesk {protocol}\" 400");
            return BadRequest();
        }

        var result = await CheckProfile(helpdesk.Role.ToString());
        if(!result.Item1) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/helpdesk {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();
        }

        if(!ValidateData(helpdesk)) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/helpdesk {protocol}\" 400");
            return BadRequest();
        }

        try {

            string? internId = await GenerateInternId(helpdesk.Role.ToString());
            string? password = GenerateDefaultPassword(internId);

            helpdesk.InternId = internId;
            helpdesk.HashPassword = passwordService.HashPassword(new(), password);

            string query = "INSERT INTO \"User\" (internid, name, email, address, phone, birthdate, hashpassword, role, docId) "+
                            $"VALUES ( ($1), ($2), ($3), ($4), ($5), ($6), ($7), '{helpdesk.Role}', ($8)) "+
                            "RETURNING id, internid, name, email, address, phone, birthdate, role, docId";


            var cmd = new NpgsqlCommand(query, _connection) {
                Parameters = {
                    new() {Value = helpdesk.InternId},
                    new() {Value = helpdesk.Name},
                    new() {Value = helpdesk.Email},
                    new() {Value = helpdesk.Address},
                    new() {Value = helpdesk.Phone},
                    new() {Value = helpdesk.BirthDate},
                    new() {Value = helpdesk.HashPassword},
                    new() {Value = helpdesk.DocId},
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) {
                
                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/helpdesk {protocol}\" 404");
                return NotFound();
            }

            await reader.ReadAsync();


            UserModel newHelpdesk = new(
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

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/create/helpdesk {protocol}\" 201");
            return Created("/", newHelpdesk);

        } catch (Exception e) {

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


    private static bool ValidateData(UserModel user) {
        if(user.Id != null) return false;
        if(!string.IsNullOrWhiteSpace(user.InternId)) return false;
        if(!string.IsNullOrWhiteSpace(user.HashPassword)) return false;
        if(user.Name.GetType() != typeof(string) || string.IsNullOrWhiteSpace(user.Name)) return false;
        if(user.DocId.GetType() != typeof(string) || string.IsNullOrWhiteSpace(user.DocId)) return false;
        if(user.Email?.GetType() != typeof(string)) return false;
        if(user.Address?.GetType() != typeof(string)) return false;
        if(user.Phone?.GetType() != typeof(string)) return false;

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

                internId = currentYear?.Split("/")[0] + nextId;
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