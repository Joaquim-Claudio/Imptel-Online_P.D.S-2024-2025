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

        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        try {
            string? sid = HttpContext.Request.Cookies["connect.sid"];

            if (string.IsNullOrWhiteSpace(sid)) {

                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/accounts/auth {protocol}\" 401");
                return Unauthorized();
            }

            string? jsonData = await _session.GetStringAsync(sid);

            if(string.IsNullOrWhiteSpace(jsonData)) {

                HttpContext.Response.Cookies.Delete("connect.sid");
                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/accounts/auth {protocol}\" 401");
                return Unauthorized("Session expired");
            }

            UserData? userData = JsonSerializer.Deserialize<UserData>(jsonData) ?? throw new Exception();

            userData = userData.Role switch
            {
                "Student" => JsonSerializer.Deserialize<StudentData>(jsonData),
                "Teacher" => JsonSerializer.Deserialize<TeacherData>(jsonData),
                "Secretary" => JsonSerializer.Deserialize<SecretaryData>(jsonData),
                _ => JsonSerializer.Deserialize<UserData>(jsonData),
            };

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/accounts/auth {protocol}\" 200");
            return Ok(userData);

        } catch(Exception e) {
            Console.WriteLine("Failed to retrieve session data.");
            throw new Exception(e.ToString());
        }
        
    }
}