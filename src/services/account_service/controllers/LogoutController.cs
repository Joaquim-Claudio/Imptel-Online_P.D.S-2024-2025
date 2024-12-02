using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace account_service.controllers;


[ApiController]
[Route("/api/accounts/")]
public class LogoutController(IDistributedCache session) : Controller {
    private readonly IDistributedCache _session = session;

    [HttpGet("logout")]
    public async Task<IActionResult> Logout() {

        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        try {
            
            string? sid = HttpContext.Request.Cookies["connect.sid"];

            if(string.IsNullOrWhiteSpace(sid)){
                
                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/accounts/logout {protocol}\" 401");
                return Unauthorized();
            }

            await _session.RemoveAsync(sid);
            HttpContext.Response.Cookies.Delete("connect.sid");

            Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"GET /api/accounts/logout {protocol}\" 200");
            return Ok();

        } catch (Exception e) {

            Console.WriteLine("Failed to retrieve session data.");
            throw new Exception(e.ToString());
        }
    }
        
}