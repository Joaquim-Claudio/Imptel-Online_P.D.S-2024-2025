namespace account_service.models;

public class StudyPlan(
    Class clss, 
    string unit, 
    string acadYear)
{
    public Class Clss {get; set;} = clss;

    public string Unit {get; set;} = unit;

    public string AcadYear {get; set;} = acadYear;
}