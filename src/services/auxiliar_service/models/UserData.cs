namespace auxiliar_service.models;

public class UserData {
    public string? InternId {get; set;}
    public string? Name {get; set;}
    public string? Role {get; set;}
    public string? AcadYear {get; set;}

    public UserData() {}

    public UserData (string internId, string name, string role, string? acadYear) {
        this.InternId = internId;
        this.Name = name;
        this.Role = role;
        this.AcadYear = acadYear;

    }
}