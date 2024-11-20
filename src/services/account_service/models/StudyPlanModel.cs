namespace account_service.models;

public class StudyPlanModel(
    ClassModel clss, 
    string unit, 
    string acadYear)
{
    public ClassModel Clss {get; set;} = clss;

    public string Unit {get; set;} = unit;

    public string AcadYear {get; set;} = acadYear;
}