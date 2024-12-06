namespace account_service.models;

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

    public UserData(UserData other) {
        this.InternId = other.InternId;
        this.Name = other.Name;
        this.Role = other.Role;
        this.AcadYear = other.AcadYear;
    }
}

public class StudentData : UserData {
    public BuildingModel? Building {get; set;}
    public string? Course {get; set;}

    public StudentData(){}
    public StudentData(UserData userData, BuildingModel building, string course) :base(userData) {
        this.Building = building;
        this.Course = course;
    }
}

public class TeacherData : UserData {
    public List<StudyPlanModel>? Classes {get; set;}

    public TeacherData(){}
    public TeacherData(UserData userData, List<StudyPlanModel> classes ) :base(userData) {
        this.Classes = classes;
    }
}

public class SecretaryData : UserData {
    public BuildingModel? Building {get; set;}

    public SecretaryData(){}
    public SecretaryData(UserData userData, BuildingModel building) :base(userData) {
        this.Building = building;
    }
}

    // Inner classes to match API requirements
public class UserCredentials (string username, string password) {
    public string Username {get; set;} = username;
    public string Password {get; set;} = password;
}

public class Query (string keywords) {
    public string Keywords {get; set;} = keywords;
}