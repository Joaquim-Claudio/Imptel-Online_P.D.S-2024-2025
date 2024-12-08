namespace registry_service.models;


public class ClassModel(int id, string name, string roomId) {
    public int Id {get; set;} = id;
    public string Name {get; set;} = name;
    public string RoomId {get; set;} = roomId;
} 