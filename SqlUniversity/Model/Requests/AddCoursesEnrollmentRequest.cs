namespace SqlUniversity.Model.Requests
{
    public class AddCoursesEnrollmentRequest
    {
        public int [] CoursesIds { get; set; }
    }

    public class AddCoursesEnrollmentResponse : EnrollmentReponse
    {
    }
}
