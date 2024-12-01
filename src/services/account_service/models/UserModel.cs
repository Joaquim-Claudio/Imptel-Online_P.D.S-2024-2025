namespace account_service.models;


public enum Role {
    Student, Teacher, Secretary, Helpdesk, Admin
}

public class UserModel(
            int? id,
            string? internId,
            string name,
            string? email,
            string? address,
            string? phone,
            DateOnly birthDate,
            string? hashPassword,
            string role,
            string docId ) 

{

    public int? Id {get;} = id;

    public string? InternId {get; set;} = internId;

    public string Name {get; set;} = name;

    public string? Email {get; set;} = email;

    public string? Address {get; set;} = address;

    public string? Phone {get; set;} = phone;

    public DateOnly BirthDate {get; set;} = birthDate;

    public string? HashPassword {get; set;} = hashPassword;

    public string Role {get; set;} = role;
    
    public string DocId {get; set;} = docId;

}

public class StudentModel(int? id, string? internId, string name, string? email, string? address,
        string? phone, DateOnly birthDate, string? hashPassword, string role, string docId) 
        : UserModel(id, internId,name, email, address, phone, birthDate, hashPassword, role, docId) 
        { }

public class TeacherModel(int? id, string? internId, string name, string? email, string? address, string? phone,
        DateOnly birthDate, string? hashPassword, string role, string docId, string academicLevel, string course) 
        : UserModel(id, internId, name, email, address, phone, birthDate, hashPassword, role, docId) 
        
        {
        public string AcademicLevel{get; set;} = academicLevel;
        public string Course{get; set;} = course;
        }


public class SecretaryModel (int? id, string? internId, string name, string? email, string? address, string? phone, 
        DateOnly birthDate, string? hashPassword, string role, string docId, string position, int buildingId) 
        : UserModel(id, internId, name, email, address, phone, birthDate, hashPassword, role, docId) 
        
        {
        public string Position{get; set;} = position;
        public int BuildingId{get; set;} = buildingId;
        }
