using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.Model.Dtos
{
    public class CourseDto : ICourseDtoMapper
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LectureId { get; set; }
        public bool IsMandatoryCourse { get; set; }
        public int CoursePoints { get; set; }
    }
}
