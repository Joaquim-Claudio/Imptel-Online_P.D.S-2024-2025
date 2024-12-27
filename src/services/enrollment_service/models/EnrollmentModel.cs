namespace enrollment_service.models;


public class EnrollmentModel (int id, string level, string acadYear, ClassModel _class, CourseModel course) {
    public int Id {get; set;} = id;
    public string Level {get; set;} = level;
    public string AcadYear {get; set;} = acadYear;
    public ClassModel _Class {get; set;} = _class;
    public CourseModel Course {get; set;} = course;
}