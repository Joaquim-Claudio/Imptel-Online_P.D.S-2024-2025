namespace registry_service.models;


public enum Status{
    Active, Finished, Inactive
}

public class RegistryModel (
            int? id, 
            DateOnly date, 
            string status, 
            bool? approved, 
            StudentModel student, 
            EnrollmentModel? enrollment,
            BuildingModel building )
{
    public int? Id {get; set;} = id;
    public DateOnly Date {get; set;} = date; 
    public string Status {get; set;} = status; 
    public bool? Approved {get; set;} = approved; 
    public StudentModel Student {get; set;} = student; 
    public EnrollmentModel? Enrollment {get; set;} = enrollment;
    public BuildingModel Building {get; set;} = building;
}