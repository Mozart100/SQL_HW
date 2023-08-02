using SqlUniversity.DataAccess.Models;
using SqlUniversity.Model.Dtos;
using SqlUniversity.Services.Validations;

namespace SqlUniversity.Model.Requests
{
    public class ErrorSection
    {
        public string Message { get; set; }
        public IEnumerable<UniversityError> Errors { get; set; }
    }

    public class UniversityReponseBase<TRequest> where TRequest : class
    {
        public TRequest Request { get; set; }
        public bool IsOperationPassed { get; set; } = true;
        public ErrorSection ErrorSection { get; set; }
    }


    public class EnrollmentNetBetReponseBase<TRequest> : UniversityReponseBase<TRequest>  where TRequest : class
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
