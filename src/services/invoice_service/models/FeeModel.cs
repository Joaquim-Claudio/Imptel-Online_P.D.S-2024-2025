namespace invoice_service.models;


public enum FeeRole {
    Subscription, Tuition, Late, Renewal
}
public class FeeModel (int id, string description, double price, bool payed, StudentModel student) {
    public int Id {get; set;} = id;
    public string Description {get;set;} = description;
    public double Price {get; set;} = price;
    public bool Payed {get; set;} = payed;
    public StudentModel Student {get; set;} = student;
}

public class EnrollmentFee (int id, string description, double price, bool payed, StudentModel student, 
                            DateOnly limitDate, string role) : FeeModel(id, description, price, payed, student) {
    public DateOnly LimitDate {get; set;} = limitDate;
    public string Role {get; set;} = role;
}