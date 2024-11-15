using System.Data;
using account_service.models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace account_service.controllers;

[ApiController]
[Route("/api/accounts/")]
public class AccountController (NpgsqlConnection connection) : Controller {

    private readonly NpgsqlConnection _connection = connection;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserCredentials credentials)
    {

        try {
            
            await using NpgsqlDataReader reader1 = await Auth(credentials);

            // Return HTTP 401 Error if no user is matched 
            if(!reader1.HasRows) {
                await reader1.CloseAsync();
                return Unauthorized();
            }

            reader1.Read();

            int id = reader1.GetInt32(0);
            string internId = reader1.GetString(1), 
            name = reader1.GetString(2),
            role = reader1.GetString(3);
            reader1.Close();

            switch (role) {

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

                    StudentData studentData = new (internId, name, role,
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

                    TeacherData teacherData = new (
                        internId,
                        name,
                        role,
                        classes
                    );

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

                    SecretaryData secretaryData = new(internId, name, role, new(
                        secReader.GetString(0), 
                        secReader.GetString(1), 
                        secReader.GetString(2),
                        secReader.GetString(3)
                        )
                    );

                    await secReader.CloseAsync();
                    return Ok(secretaryData);

                default:
                    await reader1.CloseAsync();
                    return NotFound();
            }


        } catch(Exception e) {
            throw new Exception(e.ToString());
        }
    }

    // Executes user athentication process
    // Returns a DataReader
    public async Task<NpgsqlDataReader> Auth (UserCredentials credentials) {
        try {


            string authenticationQuery = "SELECT u1.id, u1.internid, u1.name, u1.role "+
                                            "FROM \"User\" AS u1 "+
                                            "WHERE u1.internid = ($1) AND u1.hashpassword = ($2);";

            Console.WriteLine(authenticationQuery);

            using var cmd1 = new NpgsqlCommand(authenticationQuery, _connection) {
                Parameters = {
                    new () {Value = credentials.Username},
                    new () {Value = credentials.Password}
                }
            };

            // SQL command execution
            return await cmd1.ExecuteReaderAsync();

        } catch (Exception e) {
            throw new Exception(e.ToString());
        }
    }



    // Inner classes to match API requirements
    public class UserCredentials (string username, string password) {
        public string Username {get; set;} = username;
        public string Password {get; set;} = password;
    }


    public class StudentData (string internId, string name, string role, BuildingModel building, string course){

        public string InternId {get; set;} = internId;
        public string Name {get; set;} = name;
        public string Role {get; set;} = role;
        public BuildingModel Building {get; set;} = building;
        public string Course {get; set;} = course;
    }

    public class TeacherData (string internId, string name, string role, List<StudyPlan> classes ) {
        public string InternId {get; set;} = internId;
        public string Name {get; set;} = name;
        public string Role {get; set;} = role;
        public List<StudyPlan> Classes {get; set;} = classes;
    }

    public class SecretaryData (string internId, string name, string role, BuildingModel building) {
        public string InternId {get; set;} = internId;
        public string Name {get; set;} = name;
        public string Role {get; set;} = role;
        public BuildingModel Building {get; set;} = building;
    }

}