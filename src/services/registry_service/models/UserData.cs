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

public class RegistryData(int? id, int studentId, int enrollmentId, int buildingId, 
                            DateOnly date, DateOnly? updateDate, string status, bool? approved) {
    public int? Id {get; set;} = id;
    public int StudentId {get; set;} = studentId;
    public int EnrollmentId {get; set;} = enrollmentId;
    public int BuildingId {get; set;} = buildingId;
    public DateOnly Date {get; set;} = date;
    public DateOnly? UpdateDate {get; set;} = updateDate;
    public string Status {get; set;} = status;
    public bool? Approved {get; set;} = approved;
}


public class Query (string keywords) {
    public string Keywords {get; set;} = keywords;
}