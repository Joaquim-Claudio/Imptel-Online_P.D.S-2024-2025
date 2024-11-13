using Microsoft.AspNetCore.Identity;


namespace account_service.models;

public enum Role {
    Secretary, 
    Student, 
    Teacher
}

public class UserModel(
            string name,
            string? email,
            string? address,
            string? phone,
            DateOnly? birthDate,
            string hashPassword,
            Role role ) 

{

    public string Name {get; set;} = name;
    public string? Email {get; set;} = email;
    public string? Address {get; set;} = address;
    public string? Phone {get; set;} = phone;
    public DateOnly? BirthDate {get; set;} = birthDate;
    public string HashPassword {get; set;} = hashPassword;
    public Role Role {get; set;} = role;

}