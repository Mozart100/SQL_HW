namespace SqlUniversity.DataAccess.Models
{
    public enum EnrollmentTypeState
    {
        //None,
        InProgress,
        Completed,
        Payed,
        Cancelled
    }

    /// <summary>
    /// This interface solely for AutoMapper
    /// </summary>
    public interface IEnrollmentMapper
    {
        EnrollmentTypeState TypeState { get; set; }

        int[] Courses { get; set; }

        int EnrollmentForYear { get; set; }

        int StuentId { get; set; }

    }

    /// <summary>
    /// For Dto Mapper.
    /// </summary>
    public interface IEnrollmentDtoMapper : IEnrollmentMapper
    {
        int Id { get; set; }
    }

    public class Enrollment : EntityDbBase, IEnrollmentDtoMapper, IEnrollmentMapper
    {
        public EnrollmentTypeState TypeState { get; set; } = EnrollmentTypeState.InProgress;

        public int StuentId { get; set; }

        public int[] Courses { get; set; }
        public int EnrollmentForYear { get; set; }
    }

}
