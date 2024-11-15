namespace account_service.models;

public class BuildingModel(
    string name, 
    string address, 
    string phone, 
    string email)
{
    public string Name {get; set;} = name;

    public string Address {get; set;} = address;

    public string Phone {get; set;} = phone;

    public string Email {get; set;} = email;
}