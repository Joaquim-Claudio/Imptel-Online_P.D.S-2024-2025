using account_service.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;
using System.Text.Json;

namespace account_service.controllers;


[ApiController]
[Route("/api/accounts/update/")]

public class UpdateController(IDistributedCache session, 
                                NpgsqlConnection connection) : Controller {
    private readonly int SESSION_EXPIRED_CODE = 0;
    private readonly int SUCCESS_CODE = 1;
    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;


    [HttpPut("self/{internId}")]
    public async Task<IActionResult> Update([FromRoute] string internId ,[FromBody] UserModel user) {
    
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();
    
        var result = await CheckSelfProfile(user.Role.ToString(), internId);
        if(!result.Item1) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/self/{internId} {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();
        }

        if(!ValidateData(user)) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/self/{internId} {protocol}\" 400");
            return BadRequest();
        }

        try {
            string query = "UPDATE \"User\" "+
                        "SET email = ($1), address = ($2), phone = ($3) " +
                        "WHERE internId = ($4) "+
                        "RETURNING id, internid, name, email, address, phone, birthdate, role, docid";


            var cmd = new NpgsqlCommand(query, _connection) {
                Parameters = {
                    new() {Value = user.Email},
                    new() {Value = user.Address},
                    new() {Value = user.Phone},
                    new() {Value = internId},
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/self/{internId} {protocol}\" 404");
                return NotFound();
            }

            await reader.ReadAsync();

            UserModel updatedUser = new(
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

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/self/{internId} {protocol}\" 201");
            return Created("/", updatedUser);

        }catch(Exception e) {

            throw new Exception(e.ToString());
        }

        

    }



    [HttpPut("student/{internId}")]
    public async Task<IActionResult> UpdateStudent([FromRoute] string internId, [FromBody] StudentModel student) {
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();


        if(!string.Equals(student.Role.ToString(), "Student")) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/student/{internId} {protocol}\" 400");
            return BadRequest();
        }

        var result = await CheckProfile(student.Role.ToString());
        if(!result.Item1) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/student/{internId} {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();
        }

        if(!ValidateData(student)) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/student/{internId} {protocol}\" 400");
            return BadRequest();
        }

        try {
            string query = "UPDATE Student "+
                        "SET email = ($1), address = ($2), phone = ($3), docId = ($4) " +
                        "WHERE internId = ($5) "+
                        "RETURNING id, internid, name, email, address, phone, birthdate, role, docid";


            var cmd = new NpgsqlCommand(query, _connection) {
                Parameters = {
                    new() {Value = student.Email},
                    new() {Value = student.Address},
                    new() {Value = student.Phone},
                    new() {Value = student.DocId},
                    new() {Value = internId},
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/student/{internId} {protocol}\" 404");
                return NotFound();
            }

            await reader.ReadAsync();

            StudentModel updatedStudent = new(
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

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/student/{internId} {protocol}\" 201");
            return Created("/", updatedStudent);

        } catch(Exception e) {

            throw new Exception(e.ToString());
        }

        

    }



    [HttpPut("teacher/{internId}")]
    public async Task<IActionResult> UpdateTeacher([FromRoute] string internId, [FromBody] TeacherModel teacher) {
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();


        if(!string.Equals(teacher.Role.ToString(), "Teacher")) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/teacher/{internId} {protocol}\" 400");
            return BadRequest();
        }

        var result = await CheckProfile(teacher.Role.ToString());
        if(!result.Item1) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/teacher/{internId} {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();
        }

        if(!ValidateData(teacher)) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/teacher/{internId} {protocol}\" 400");
            return BadRequest();
        }

        try {
            string query = "UPDATE Teacher "+
                        "SET email = ($1), address = ($2), phone = ($3), docId = ($4), academicLevel = ($5), course = ($6) " +
                        "WHERE internId = ($7) "+
                        "RETURNING id, internid, name, email, address, phone, birthdate, role, docid, academicLevel, course";

            var cmd = new NpgsqlCommand(query, _connection) {
                Parameters = {
                    new() {Value = teacher.Email},
                    new() {Value = teacher.Address},
                    new() {Value = teacher.Phone},
                    new() {Value = teacher.DocId},
                    new() {Value = teacher.AcademicLevel},
                    new() {Value = teacher.Course},
                    new() {Value = internId},
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/teacher/{internId} {protocol}\" 404");
                return NotFound();
            }

            await reader.ReadAsync();

            TeacherModel updatedTeacher = new(
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

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/teacher/{internId} {protocol}\" 201");
            return Created("/", updatedTeacher);

        } catch(Exception e) {

            throw new Exception(e.ToString());
        }

        
    }



    [HttpPut("secretary/{internId}")]
    public async Task<IActionResult> UpdateSecretary([FromRoute] string internId, [FromBody] SecretaryModel secretary) {
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();


        if(!string.Equals(secretary.Role.ToString(), "Secretary")) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/secretary/{internId} {protocol}\" 400");
            return BadRequest();
        }

        var result = await CheckProfile(secretary.Role.ToString());
        if(!result.Item1) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/secretary/{internId} {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();
        }

        if(!ValidateData(secretary)) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/secretary/{internId} {protocol}\" 400");
            return BadRequest();
        }

        try {
            string query = "UPDATE Secretary "+
                        "SET email = ($1), address = ($2), phone = ($3), docId = ($4), position = ($5), building_id = ($6) " +
                        "WHERE internId = ($7) "+
                        "RETURNING id, internid, name, email, address, phone, birthdate, role, docid, position, building_id";


            var cmd = new NpgsqlCommand(query, _connection) {
                Parameters = {
                    new() {Value = secretary.Email},
                    new() {Value = secretary.Address},
                    new() {Value = secretary.Phone},
                    new() {Value = secretary.DocId},
                    new() {Value = secretary.Position},
                    new() {Value = secretary.BuildingId},
                    new() {Value = internId},
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/secretary/{internId} {protocol}\" 404");
                return NotFound();
            }
            
            await reader.ReadAsync();

            SecretaryModel updatedSecretary = new(
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

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/secretary/{internId} {protocol}\" 201");
            return Created("/", updatedSecretary);

        } catch(Exception e) {

            throw new Exception(e.ToString());
        }

        

    }


    [HttpPut("helpdesk/{internId}")]
    public async Task<IActionResult> UpdateHelpdesk([FromRoute] string internId, [FromBody] UserModel helpdesk) {
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();


        if(!string.Equals(helpdesk.Role.ToString(), "Helpdesk")) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/helpdesk/{internId} {protocol}\" 400");
            return BadRequest();
        }

        var result = await CheckProfile(helpdesk.Role.ToString());
        if(!result.Item1) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/helpdesk/{internId} {protocol}\" 401");
            return result.Item2 == SESSION_EXPIRED_CODE ? Unauthorized("Session expired") : Unauthorized();
        }

        if(!ValidateData(helpdesk)) {

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/helpdesk/{internId} {protocol}\" 400");
            return BadRequest();
        }

        try {
            string query = "UPDATE \"User\" "+
                        "SET email = ($1), address = ($2), phone = ($3) " +
                        "WHERE internId = ($4) "+
                        "RETURNING id, internid, name, email, address, phone, birthdate, role, docid";

            var cmd = new NpgsqlCommand(query, _connection) {
                Parameters = {
                    new() {Value = helpdesk.Email},
                    new() {Value = helpdesk.Address},
                    new() {Value = helpdesk.Phone},
                    new() {Value = internId},
                }
            };

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
            if(!reader.HasRows) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/helpdesk/{internId} {protocol}\" 404");
                return NotFound();
            }
            
            await reader.ReadAsync();

            UserModel updatedHelpdesk = new(
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

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"PUT /api/accounts/update/helpdesk/{internId} {protocol}\" 201");
            return Created("/", updatedHelpdesk);

        } catch (Exception e) {
            
            throw new Exception(e.ToString());
        }

    }


    private async Task<(bool, int)> CheckSelfProfile (string role, string internId) {

        try {
            
            string? sid = HttpContext.Request.Cookies["connect.sid"];
            if(string.IsNullOrWhiteSpace(sid)) return (false, -1);

            string? jsonData = await _session.GetStringAsync(sid);

            if(string.IsNullOrWhiteSpace(jsonData)) {
                HttpContext.Response.Cookies.Delete("connect.sid");
                return (false, SESSION_EXPIRED_CODE);
            }

            UserData? user = JsonSerializer.Deserialize<UserData>(jsonData) ?? throw new Exception();
            if(!string.Equals(user.Role, role) || !string.Equals(user.InternId, internId)) return (false, -1);

            return (true, SUCCESS_CODE);

        } catch(Exception e) {

            Console.WriteLine("Failed to retrieve session data.");
            throw new Exception(e.ToString());
        }


    }

    private async Task<(bool, int)> CheckProfile (string userToUpdateRole) {

        try {

            string? sid = HttpContext.Request.Cookies["connect.sid"];
            if(string.IsNullOrWhiteSpace(sid)) return (false, -1);

            string? jsonData = await _session.GetStringAsync(sid);

            if(string.IsNullOrWhiteSpace(jsonData)) {
                HttpContext.Response.Cookies.Delete("connect.sid");
                return (false, SESSION_EXPIRED_CODE);
            }

            UserData? user = JsonSerializer.Deserialize<UserData>(jsonData) ?? throw new Exception();

            switch(userToUpdateRole) {
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

}