using System.Text.Json;
using account_service.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace account_service.controllers;


[ApiController]
[Route("/api/accounts/")]
public class AuthController(IDistributedCache session) : Controller {
    private readonly IDistributedCache _session = session;


    [HttpGet("auth")]
    public async Task<IActionResult> Authenticate() {
        string? sid = HttpContext.Request.Cookies["connect.sid"];

        if (string.IsNullOrWhiteSpace(sid)) return Unauthorized();

        string? jsonData = await _session.GetStringAsync(sid);

        if(string.IsNullOrWhiteSpace(jsonData)) {
            HttpContext.Response.Cookies.Delete("connect.sid"); 
            return Unauthorized("Session expired");
        }

        UserData? userData = JsonSerializer.Deserialize<UserData>(jsonData) ?? throw new Exception();

        switch(userData.Role) {
            case "Student":
                userData = JsonSerializer.Deserialize<StudentData>(jsonData);
            break;
            case "Teacher":
                userData = JsonSerializer.Deserialize<TeacherData>(jsonData);
            break;
            case "Secretary":
                userData = JsonSerializer.Deserialize<SecretaryData>(jsonData);
            break;
        }

        return Ok(userData);
    }
}