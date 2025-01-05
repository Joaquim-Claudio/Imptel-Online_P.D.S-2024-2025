namespace invoice_service.models;

public class StudentModel(int id, string internId, string name) {
    public int Id{get; set;} = id;
    public string InternId {get; set;} = internId;
    public string Name {get; set;} = name;
}