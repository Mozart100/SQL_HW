namespace SqlUniversity.DataAccess.Models
{
    public class AssignCourseYearly : EntityDbBase
    {
        public int CourseId { get; set; }
        public int Year { get; set; }
    }
}
