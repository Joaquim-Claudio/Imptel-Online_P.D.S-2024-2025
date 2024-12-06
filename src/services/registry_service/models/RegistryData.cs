namespace registry_service.models;

public class SimpleRegistryData(int id, string course, string _class, string student, string internid)
{
    public int Id { get; set; } = id;
    public string Course { get; set; } = course;
    public string Class { get; set; } = _class;
    public string Student { get; set; } = student;
    public string Internid { get; set; } = internid;
}