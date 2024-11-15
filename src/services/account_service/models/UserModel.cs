using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;


namespace account_service.models;


public enum Role {
    Student, Teacher, Secretary
}

[Table("User")]
public class UserModel(
            int id,
            string name,
            string? email,
            string? address,
            string? phone,
            DateOnly? birthDate,
            string hashPassword,
            Role role ) 

{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id {get;} = id;

    [Required]
    [Column("name")]
    public string Name {get; set;} = name;

    [Column("email")]
    public string? Email {get; set;} = email;

    [Column("address")]
    public string? Address {get; set;} = address;

    [Column("phone")]
    public string? Phone {get; set;} = phone;

    [Column("birthDate")]
    public DateOnly? BirthDate {get; set;} = birthDate;

    [Required]
    [Column("hashPassword")]
    public string HashPassword {get; set;} = hashPassword;

    [Required]
    [Column("role")]
    public Role Role {get; set;} = role;

}