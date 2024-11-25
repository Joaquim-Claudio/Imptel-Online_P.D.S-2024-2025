namespace account_service.models;

public class SecretaryModel (int? id, string? internId, string name, string? email, string? address, string? phone, 
        DateOnly birthDate, string? hashPassword, string role, string docId, string position, int buildingId) 
        : UserModel(id, internId, name, email, address, phone, birthDate, hashPassword, role, docId) 
        
        {
        public string Position{get; set;} = position;
        public int BuildingId{get; set;} = buildingId;
        }