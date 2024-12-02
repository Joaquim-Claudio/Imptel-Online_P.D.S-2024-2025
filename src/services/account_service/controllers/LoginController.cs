using System.Text.Json;
using account_service.models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Npgsql;


namespace account_service.controllers;


[ApiController]
[Route("/api/accounts/")]
public class LoginController (NpgsqlConnection connection, 
                                IDistributedCache session,
                                IDataProtectionProvider provider) : Controller {
    
    private readonly static double SESSION_EXPIRE_TIME_IN_HOURS = 12;
    public readonly static string DATA_PROTECTOR_NAME = "accounts.protector";

    private readonly NpgsqlConnection _connection = connection;
    private readonly IDistributedCache _session = session;
    private readonly IDataProtector _protector = provider.CreateProtector(DATA_PROTECTOR_NAME);
    private static readonly PasswordHasher<Object> passwordService = new();


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
    {
        string protocol = HttpContext.Request.Protocol;
        string? remote_ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        try {
            
            var AuthData = await Auth(credentials);
            UserData? user = AuthData.Item1; int id = AuthData.Item2;

            // Return HTTP 401 Error if no user is matched 
            if(user is null || id == -1) {
                Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/login {protocol}\" 401");
                return Unauthorized();
            }
            
            switch (user.Role) {

                // Case Role == 'Student'
                case "Student":

                    string studentQuery = "SELECT b1.name, b1.address, b1.phone, b1.email, c1.name "+
                                            "FROM registry AS r1 "+
                                            "INNER JOIN building AS b1 ON r1.building_id=b1.id "+
                                            "INNER JOIN enrollment AS e1 ON r1.enrollment_id=e1.id "+
                                            "INNER JOIN course AS c1 ON e1.course_id = c1.id " +
                                            "WHERE r1.student_id = ($1) AND r1.status='Active';";


                    var stdCmd = new NpgsqlCommand(studentQuery, _connection) {
                        Parameters = {new() { Value=id }}
                    };

                    NpgsqlDataReader stdReader = await stdCmd.ExecuteReaderAsync();
                    if(!stdReader.HasRows) {
                        Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/login {protocol}\" 404");
                        return NotFound();
                    } 

                    await stdReader.ReadAsync();

                    StudentData studentData = new (user,
                        new BuildingModel(
                            stdReader.GetString(0),
                            stdReader.GetString(1),
                            stdReader.GetString(2),
                            stdReader.GetString(3)),
                            stdReader.GetString(4)
                    );

                    await stdReader.CloseAsync();

                    if (studentData.InternId == null) throw new Exception();
                    
                    string? std_sid = _protector.Protect(studentData.InternId);
                    await _session.SetStringAsync(std_sid, 
                        JsonSerializer.Serialize<StudentData>(studentData), 
                        new DistributedCacheEntryOptions{
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(SESSION_EXPIRE_TIME_IN_HOURS)
                        });

                    HttpContext.Response.Cookies.Append("connect.sid", std_sid, 
                        new CookieOptions{
                            HttpOnly = true,
                            Secure = true
                        });


                    Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/login {protocol}\" 200");
                    return Ok();


                // Case Role == 'Teacher'
                case "Teacher":
                    string teacherQuery = "SELECT c1.id, c1.name, c1.roomid, u1.name, s1.acadyear "+
                                            "FROM studyplan AS s1 "+
                                            "INNER JOIN \"Class\" AS c1 ON s1.class_id=c1.id "+
                                            "INNER JOIN unit AS u1 ON s1.unit_id=u1.id "+
                                            "WHERE s1.teacher_id = ($1) "+
                                            "ORDER BY c1.name;";


                    var teaCmd = new NpgsqlCommand(teacherQuery, _connection) {
                        Parameters = {new() {Value=id}}
                    };

                    NpgsqlDataReader teaReader = await teaCmd.ExecuteReaderAsync();
                    if(!teaReader.HasRows) {
                        Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/login {protocol}\" 404");
                        return NotFound();
                    }

                    List<StudyPlanModel> classes = [];

                    while(await teaReader.ReadAsync()) {

                        ClassModel clss = new (teaReader.GetInt32(0), teaReader.GetString(1), teaReader.GetString(2));
                        string unit = teaReader.GetString(3);
                        string acadYear = teaReader.GetString(4);

                        classes.Add(new StudyPlanModel(clss, unit, acadYear));
                    }

                    TeacherData teacherData = new (user, classes);

                    await teaReader.CloseAsync();

                    if (teacherData.InternId == null) throw new Exception();

                    string? tea_sid = _protector.Protect(teacherData.InternId);

                    await _session.SetStringAsync(tea_sid , 
                    JsonSerializer.Serialize<TeacherData>(teacherData),
                    new DistributedCacheEntryOptions{
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(SESSION_EXPIRE_TIME_IN_HOURS)
                    });
                    
                    HttpContext.Response.Cookies.Append("connect.sid", tea_sid,
                    new CookieOptions{
                        HttpOnly = true,
                        Secure = true
                    });


                    Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/login {protocol}\" 200");
                    return Ok();

                // Case Role == 'Secretary'
                case "Secretary":
                    
                    string secQuery = "SELECT b1.name, b1.address, b1.phone, b1.email "+
                                        "FROM Secretary AS s1 "+
                                        "INNER JOIN building AS b1 ON s1.building_id=b1.id "+
                                        "WHERE s1.id = ($1)";


                    var secCmd = new NpgsqlCommand(secQuery, _connection) {
                        Parameters = {new() {Value=id}}
                    };

                    NpgsqlDataReader secReader = await secCmd.ExecuteReaderAsync();
                    if(!secReader.HasRows) {
                        Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/login {protocol}\" 404");
                        return NotFound();
                    } 
                    await secReader.ReadAsync();

                    SecretaryData secretaryData = new(user, new(
                        secReader.GetString(0), 
                        secReader.GetString(1), 
                        secReader.GetString(2),
                        secReader.GetString(3)
                        )
                    );

                    await secReader.CloseAsync();

                    if (secretaryData.InternId == null) throw new Exception();

                    string? sec_sid = _protector.Protect(secretaryData.InternId);

                    await _session.SetStringAsync(sec_sid, 
                    JsonSerializer.Serialize<SecretaryData>(secretaryData),
                    new DistributedCacheEntryOptions{

                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(SESSION_EXPIRE_TIME_IN_HOURS)
                    });


                    HttpContext.Response.Cookies.Append("connect.sid", sec_sid,
                    new CookieOptions{
                        HttpOnly = true,
                        Secure = true
                    });


                    Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/login {protocol}\" 200");
                    return Ok();


                case "Helpdesk":
                    
                    if (user.InternId == null) throw new Exception();

                    string? hp_sid = _protector.Protect(user.InternId);

                    await _session.SetStringAsync(hp_sid , 
                    JsonSerializer.Serialize<UserData>(user),
                    new DistributedCacheEntryOptions{
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(SESSION_EXPIRE_TIME_IN_HOURS)
                    });
                    
                    HttpContext.Response.Cookies.Append("connect.sid", hp_sid,
                    new CookieOptions{
                        HttpOnly = true,
                        Secure = true
                    });

                    Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/login {protocol}\" 200");
                    return Ok();

                case "Admin":
                    
                    if (user.InternId == null) throw new Exception();

                    string? adm_sid = _protector.Protect(user.InternId);

                    await _session.SetStringAsync(adm_sid , 
                    JsonSerializer.Serialize<UserData>(user),
                    new DistributedCacheEntryOptions{
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(SESSION_EXPIRE_TIME_IN_HOURS)
                    });
                    
                    HttpContext.Response.Cookies.Append("connect.sid", adm_sid,
                    new CookieOptions{
                        HttpOnly = true,
                        Secure = true
                    });

                    Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/login {protocol}\" 200");
                    return Ok();


                default:

                    Console.WriteLine($"[{DateTime.Now}] From: {remote_ip} \"POST /api/accounts/login {protocol}\" 404");
                    return NotFound();
            }


        } catch(Exception e) {
            _connection.Close();
            await _connection.OpenAsync();
            throw new Exception(e.ToString());
        }
    }

    // Executes user athentication process
    // Returns a DataReader
    public async Task<(UserData?, int)> Auth (UserCredentials credentials) {

        try {

            // SQL query to retrieve a matching user 
            string authenticationQuery = "SELECT u1.id, u1.hashpassword, u1.internid, u1.name, u1.role "+
                                            "FROM \"User\" AS u1 "+
                                            "WHERE u1.internid = ($1);";


            using var cmd1 = new NpgsqlCommand(authenticationQuery, _connection) {
                Parameters = {
                    new () {Value = credentials.Username}
                }
            };

            // SQL command execution
            using NpgsqlDataReader reader = await cmd1.ExecuteReaderAsync();
            
            // Breaks if no user matches
            if(!reader.HasRows) return (null, -1);

            await reader.ReadAsync();

            int id = reader.GetInt32(0);
            string hashedPassword = reader.GetString(1);


            // Breaks if the password doesn't match
            if(passwordService.VerifyHashedPassword(new(), hashedPassword, credentials.Password) == PasswordVerificationResult.Failed)
                return  (null, -1);

            // Success: Creates a UserData to return
            UserData user = new( reader.GetString(2), reader.GetString(3), reader.GetString(4) );

            return (user, id);

        } catch (Exception e) {
            
            Console.Write("Failed to authenticate.");
            throw new Exception(e.ToString());
        }
    }

}