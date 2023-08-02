namespace SqlUniversity.DataAccess.Models
{
    /// <summary>
    /// This interface solely for AutoMapper
    /// </summary>
    public interface ICourseMapper
    {
        string Name { get; set; }
        int LectureId { get; set; }
        bool IsMandatoryCourse { get; set; }
        int CoursePoints { get; set; }
    }

    /// <summary>
    /// For Dto Mapper.
    /// </summary>
    public interface ICourseDtoMapper : ICourseMapper
    {
        int Id { get; set; }
    }

    public class Course : EntityDbBase, ICourseMapper, ICourseDtoMapper
    {
        public string Name { get; set; }
        public int LectureId { get; set; }
        public bool IsMandatoryCourse { get; set; }
        public int CoursePoints { get; set; }
    }
}
