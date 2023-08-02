using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.Model.Dtos
{
    public class EnrollmentDto : IEnrollmentDtoMapper
    {
        public int Id { get; set; }
        public EnrollmentTypeState TypeState { get; set; } = EnrollmentTypeState.InProgress;

        public int[] Courses { get; set; }

        public int EnrollmentForYear { get; set; }

        //This property is essential since a student can finish the registration without passing or reaching  the threshold
        public bool IsPassedThreshold { get; set; } = false;

        public int StuentId { get; set; }

    }
}
