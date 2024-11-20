using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace account_service.controllers;


[ApiController]
[Route("/api/accounts/")]
public class LogoutController(IDistributedCache session) : Controller {
    private readonly IDistributedCache _session = session;

    [HttpGet("logout")]
    public async Task<IActionResult> Logout() {

        string? sid = HttpContext.Request.Cookies["connect.sid"];

        if(string.IsNullOrWhiteSpace(sid)) return Unauthorized();

        await _session.RemoveAsync(sid);
        HttpContext.Response.Cookies.Delete("connect.sid");
        return Ok();
    }
}