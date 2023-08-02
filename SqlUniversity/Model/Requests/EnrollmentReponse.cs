using SqlUniversity.Model.Dtos;
using SqlUniversity.Services.Validations;

namespace SqlUniversity.Model.Requests
{
    public class EnrollmentReponse : EnrollmentDto
    {
        //Indicates whether the response was processed or not (valid or invalid).
        public bool IsOperationPassed { get; set; }
        public string Message { get; set; }

        public IEnumerable<UniversityError> Errors { get; set; }
    }



    public class ErrorSection
    {
        public string Message { get; set; }
        public IEnumerable<UniversityError> Errors { get; set; }
    }

    public class NetBetReponseBase<TRequest> where TRequest : class
    {
        public TRequest Request { get; set; }
        public bool IsOperationPassed { get; set; } = true;
        public ErrorSection ErrorSection { get; set; }
    }
}
