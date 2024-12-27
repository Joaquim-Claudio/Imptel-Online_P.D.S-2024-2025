namespace enrollment_service.models;

public class EnrollmentData (
                    int? id, 
                    int classId, 
                    int courseId,
                    string level,
                    string acadYear) {

    public int? Id {get; set;} = id;
    public int ClassId {get; set;} = classId;
    public int CourseId {get; set;} = courseId;
    public string Level {get; set;} = level;
    public string AcadYear {get; set;} = acadYear;
}