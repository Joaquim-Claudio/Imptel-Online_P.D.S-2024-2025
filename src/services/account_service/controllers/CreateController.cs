using System.Data;
using System.Linq.Expressions;
using System.Text.Json;
using account_service.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;

namespace account_service.controllers;

[ApiController]
[Route("/api/accounts/")]
public class CreateController(IDistributedCache session,
                                NpgsqlConnection connection) : Controller {

    private readonly IDistributedCache _session = session;
    private readonly NpgsqlConnection _connection = connection;
    private static readonly PasswordHasher<Object> passwordService = new();

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] UserModel user) {

        if(!CheckSecretaryProfile().Result) return Unauthorized();

        if(!ValidateData(user)) return BadRequest();

        string? internId = await GenerateInternId(user.Role.ToString());
        string? password = GenerateDefaultPassword(internId);

        user.InternId = internId;
        user.HashPassword = passwordService.HashPassword(new(), password);

        string query = "INSERT INTO \"User\" (internid, name, email, address, phone, birthdate, hashpassword, role, docid) "+
                        $"VALUES ( ($1), ($2), ($3), ($4), ($5), ($6), ($7), '{user.Role}', ($8) ) "+
                        "RETURNING *";

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



    private async Task<bool> CheckSecretaryProfile () {
        
        string? sid = HttpContext.Request.Cookies["connect.sid"];
        if(string.IsNullOrWhiteSpace(sid)) return false;

        string? jsonData = await _session.GetStringAsync(sid);

        if(string.IsNullOrWhiteSpace(jsonData)) {
            HttpContext.Response.Cookies.Delete("connect.sid"); 
            return false;
        }

        UserData? user = JsonSerializer.Deserialize<UserData>(jsonData) ?? throw new Exception();

        if(!string.Equals(user.Role, "Secretary")) return false;

        return true;
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
        string? currentYear = await _session.GetStringAsync("CURRENT_ACAD_YEAR");
        string? nextId="";

        switch (role) {
            case "Student":
                nextId = await _session.GetStringAsync("NEXT_STUDENT_SUFIX") ?? throw new Exception("Next id not found!");
                string stdNextId = (int.Parse(nextId) + 1).ToString().PadLeft(4, '0');
                await _session.SetStringAsync("NEXT_STUDENT_SUFIX", stdNextId);
            break;

            case "Teacher":
                nextId = await _session.GetStringAsync("NEXT_TEACHER_SUFIX") ?? throw new Exception("Next id not found!");
                string teaNextId = (int.Parse(nextId) + 1).ToString().PadLeft(4, '0');
                await _session.SetStringAsync("NEXT_TEACHER_SUFIX", teaNextId);
            break;

            case "Secretary":
                nextId = await _session.GetStringAsync("NEXT_SECRETARY_SUFIX") ?? throw new Exception("Next id not found!");
                string secNextId = (int.Parse(nextId) + 1).ToString().PadLeft(4, '0');
                await _session.SetStringAsync("NEXT_TEACHER_SUFIX", secNextId);
            break;

        }

        return currentYear + nextId;
    }
}