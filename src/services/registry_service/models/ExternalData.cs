namespace registry_service.models;

public class ListRegistryData(int id, string course, ClassModel _class, StudentModel student) {
    public int Id { get; set; } = id;
    public string Course { get; set; } = course;
    public ClassModel _Class { get; set; } = _class;
    public StudentModel Student {get; set;} = student;
}

public class StudentRegistries(int id, string internId, string name, List<RegistryModel>? registries){
    public int Id {get; set;} = id;
    public string InternId {get; set;} = internId;
    public string Name {get; set;} = name;
    public List<RegistryModel>? Registries {get; set;} = registries;
}