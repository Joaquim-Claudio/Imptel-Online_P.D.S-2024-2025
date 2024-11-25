namespace account_service.models;

public class TeacherModel (int? id, string? internId, string name, string? email, string? address, string? phone, 
        DateOnly birthDate, string? hashPassword, string role, string docId, string academicLevel, string course) 
        : UserModel(id, internId, name, email, address, phone, birthDate, hashPassword, role, docId) 
        
        {
        public string AcademicLevel{get; set;} = academicLevel;
        public string Course{get; set;} = course;
        }