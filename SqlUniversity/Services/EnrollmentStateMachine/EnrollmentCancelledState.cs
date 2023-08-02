using AutoMapper;
using SqlUniversity.DataAccess.Models;
using SqlUniversity.DataAccess.Repository;
using SqlUniversity.Model.Dtos;

namespace SqlUniversity.Services.EnrollmentStateMachine
{
    public class EnrollmentCancelledState : EnrollmentStateBase
    {
        public EnrollmentCancelledState(AcademicYearDto academicYearDto, IEnumerable<CourseDto> courses, IEnrollmentRepository enrollmentRepository, IMapper mapper)
            : base(academicYearDto, courses, enrollmentRepository, mapper)
        {
        }

        public override EnrollmentTypeState EnrollmentTypeState => EnrollmentTypeState.Cancelled;

    }
}
