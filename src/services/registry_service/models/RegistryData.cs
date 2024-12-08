namespace registry_service.models;

public class RegistryData(int id, string course, ClassModel _class, StudentModel student)
{
    public int Id { get; set; } = id;
    public string Course { get; set; } = course;
    public ClassModel _Class { get; set; } = _class;
    public StudentModel Student {get; set;} = student;
}