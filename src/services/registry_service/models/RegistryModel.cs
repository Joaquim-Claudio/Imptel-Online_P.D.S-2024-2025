namespace registry_service.models;


public enum Status{
    Active, Finished, Inactive
}

public class RegistryModel (
            int? id, 
            DateOnly date,
            DateOnly updateDate,
            string status, 
            bool? approved,
            EnrollmentModel? enrollment,
            BuildingModel? building )
{
    public int? Id {get; set;} = id;
    public DateOnly Date {get; set;} = date;
    public DateOnly UpdateDate {get; set;} = updateDate;
    public string Status {get; set;} = status; 
    public bool? Approved {get; set;} = approved; 
    public EnrollmentModel? Enrollment {get; set;} = enrollment;
    public BuildingModel? Building {get; set;} = building;
}