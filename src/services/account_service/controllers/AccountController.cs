using System.Data;
using account_service.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Npgsql;
using Npgsql.Replication;

namespace account_service.controllers;

[ApiController]
[Route("/api/accounts/")]
public class AccountController (NpgsqlConnection connection, IDistributedCache session) : Controller {

    private readonly NpgsqlConnection _connection = connection;
    private readonly IDistributedCache _session = session;
    private static readonly PasswordHasher<Object> passwordService = new();


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
    {

        try {
            
            var AuthData = await Auth(credentials);
            UserData? user = AuthData.Item1; int id = AuthData.Item2;

            // Return HTTP 401 Error if no user is matched 
            if(user is null || id == -1) return Unauthorized();

            
            switch (user.Role) {

                // Case Role == 'Student'
                case "Student":

                    string studentQuery = "SELECT b1.name, b1.address, b1.phone, b1.email, c1.name "+
                                            "FROM registry AS r1 "+
                                            "INNER JOIN building AS b1 ON r1.building_id=b1.id "+
                                            "INNER JOIN enrollment AS e1 ON r1.enrollment_id=e1.id "+
                                            "INNER JOIN course AS c1 ON e1.course_id = c1.id " +
                                            "WHERE r1.student_id = ($1) AND r1.status='Active';";

                    Console.WriteLine(studentQuery + " id=" + id + "\n");

                    var stdCmd = new NpgsqlCommand(studentQuery, _connection) {
                        Parameters = {new() { Value=id }}
                    };

                    NpgsqlDataReader stdReader = await stdCmd.ExecuteReaderAsync();

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
                    
                    return Ok(studentData);


                // Case Role == 'Teacher'
                case "Teacher":
                    string teacherQuery = "SELECT c1.id, c1.name, c1.roomid, u1.name, s1.acadyear "+
                                            "FROM studyplan AS s1 "+
                                            "INNER JOIN \"Class\" AS c1 ON s1.class_id=c1.id "+
                                            "INNER JOIN unit AS u1 ON s1.unit_id=u1.id "+
                                            "WHERE s1.teacher_id = ($1) "+
                                            "ORDER BY c1.name;";
 
                    Console.WriteLine(teacherQuery + " id=" + id + "\n");

                    var teaCmd = new NpgsqlCommand(teacherQuery, _connection) {
                        Parameters = {new() {Value=id}}
                    };

                    NpgsqlDataReader teaReader = await teaCmd.ExecuteReaderAsync();

                    List<StudyPlan> classes = [];

                    while(await teaReader.ReadAsync()) {

                        Class clss = new (teaReader.GetInt32(0), teaReader.GetString(1), teaReader.GetString(2));
                        string unit = teaReader.GetString(3);
                        string acadYear = teaReader.GetString(4);

                        classes.Add(new StudyPlan(clss, unit, acadYear));
                    }

                    TeacherData teacherData = new (user, classes);

                    await teaReader.CloseAsync();
                    return Ok(teacherData);

                // Case Role == 'Secretary'
                case "Secretary":
                    
                    string secQuery = "SELECT b1.name, b1.address, b1.phone, b1.email "+
                                        "FROM secretary AS s1 "+
                                        "INNER JOIN building AS b1 ON s1.building_id=b1.id "+
                                        "WHERE s1.id = ($1)";

                    var secCmd = new NpgsqlCommand(secQuery, _connection) {
                        Parameters = {new() {Value=id}}
                    };

                    NpgsqlDataReader secReader = await secCmd.ExecuteReaderAsync();

                    await secReader.ReadAsync();

                    SecretaryData secretaryData = new(user, new(
                        secReader.GetString(0), 
                        secReader.GetString(1), 
                        secReader.GetString(2),
                        secReader.GetString(3)
                        )
                    );

                    await secReader.CloseAsync();
                    return Ok(secretaryData);

                default:
                    return NotFound();
            }


        } catch(Exception e) {
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
            
            // Dev: Prints the query for debug
            Console.WriteLine(authenticationQuery);


            using var cmd1 = new NpgsqlCommand(authenticationQuery, _connection) {
                Parameters = {
                    new () {Value = credentials.Username}
                }
            };

            // SQL command execution
            NpgsqlDataReader reader = await cmd1.ExecuteReaderAsync();
            
            // Breaks if no user matches
            if(!reader.HasRows) {
                await reader.CloseAsync();
                return (null, -1);
            }

            await reader.ReadAsync();

            int id = reader.GetInt32(0);
            string hashedPassword = reader.GetString(1);


            // Breaks if the password doesn't match
            if(passwordService.VerifyHashedPassword(null, hashedPassword, credentials.Password) == PasswordVerificationResult.Failed){
                await reader.CloseAsync();
                return  (null, -1);
            }
            

            // Success: Creates a UserData to return
            UserData user = new( reader.GetString(2), reader.GetString(3), reader.GetString(4) );

            await reader.CloseAsync();

            //await HttpContext.Session.LoadAsync();
            //HttpContext.Session.SetString("item2", "valor2");
            await _session.SetStringAsync("item2", "valor2", new DistributedCacheEntryOptions{
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(48)
            });

            string? item1 = await _session.GetStringAsync("item2");
            // var item1 = HttpContext.Session.GetString("item1");
            Console.WriteLine("Item1: " + item1);

            return (user, id);

        } catch (Exception e) {
            throw new Exception(e.ToString());
        }
    }



    // Inner classes to match API requirements
    public class UserCredentials (string username, string password) {
        public string Username {get; set;} = username;
        public string Password {get; set;} = password;
    }


    public class UserData {
        public string InternId {get; set;}
        public string Name {get; set;}
        public string Role {get; set;}

        public UserData (string internId, string name, string role) {
            this.InternId = internId;
            this.Name = name;
            this.Role = role;
        }

        public UserData(UserData other) {
            this.InternId = other.InternId;
            this.Name = other.Name;
            this.Role = other.Role;
        }
    }

    public class StudentData(UserData userData, BuildingModel building, string course) : UserData(userData){
        public BuildingModel Building {get; set;} = building;
        public string Course {get; set;} = course;
    }

    public class TeacherData (UserData userData, List<StudyPlan> classes ) : UserData(userData){
        public List<StudyPlan> Classes {get; set;} = classes;
    }

    public class SecretaryData (UserData userData, BuildingModel building) : UserData(userData){
        public BuildingModel Building {get; set;} = building;
    }

}