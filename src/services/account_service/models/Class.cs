namespace account_service.models;

public class Class(
    int id,
    string name, 
    string rommId)
{
    public int Id {get; set;} = id;

    public string Name {get; set;} = name;

    public string RommId {get; set;} = rommId;
}