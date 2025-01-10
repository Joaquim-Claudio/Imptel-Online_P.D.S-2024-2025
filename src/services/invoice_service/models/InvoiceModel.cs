namespace invoice_service.models;

public class InvoiceModel(int id, DateOnly date, string reference, double value) {
    public int Id {get; set;} = id;
    public DateOnly Date {get; set;} = date;
    public string Reference {get; set;} = reference;
    public double Value {get; set;} = value;
}