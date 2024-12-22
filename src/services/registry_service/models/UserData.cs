namespace registry_service.models;

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

public class RegistryData(int studentId, int enrollmentId, int buildingId, DateOnly date, string status) {
    public int StudentId {get; set;} = studentId;
    public int EnrollmentId {get; set;} = enrollmentId;
    public int BuildingId {get; set;} = buildingId;
    public DateOnly Date {get; set;} = date;
    public string Status {get; set;} = status;
}


public class Query (string keywords) {
    public string Keywords {get; set;} = keywords;
}