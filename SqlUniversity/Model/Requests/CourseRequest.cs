using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.Model.Requests
{
    public class CourseRequest : ICourseMapper
    {
        public string Name { get; set; }
        public int LectureId { get; set; }
        public bool IsMandatoryCourse { get; set; }
        public int CoursePoints { get; set; }
        public int[] YearsSuited { get; set; }


    }

    public class CourseResponse : UniversityReponseBase<CourseRequest>
    {
        public int CourseID { get; set; }
    }
}
