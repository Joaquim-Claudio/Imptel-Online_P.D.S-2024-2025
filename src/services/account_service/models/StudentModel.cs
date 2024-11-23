namespace account_service.models;

public class StudentModel(int? id, string? internId, string name, string? email, string? address,
        string? phone, DateOnly birthDate, string? hashPassword, string role, string docId) 
        : UserModel(id, internId,name, email, address, phone, birthDate, hashPassword, role, docId) 
        { }