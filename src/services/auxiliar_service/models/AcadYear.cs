namespace auxiliar_service.models;

public class AcadYear (int id, string name, DateOnly createdAt, DateOnly? finishedAt) {
    public int Id {get; set;} = id;
    public string Name {get; set;} = name;
    public DateOnly CreatedAt {get; set;} = createdAt;
    public DateOnly? FinishedAt {get; set;} = finishedAt;
}