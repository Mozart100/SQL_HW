using AutoMapper;
using SqlUniversity.DataAccess.Models;
using SqlUniversity.DataAccess.Repository;
using SqlUniversity.Infrastracture;
using SqlUniversity.Model.Dtos;

namespace SqlUniversity.Services.EnrollmentStateMachine
{
    public class EnrollmentCompletedState : EnrollmentStateBase, IChangeCoursesState, IPaidState, ICancelledState
    {
        public EnrollmentCompletedState(AcademicYearDto academicYearDto, IEnumerable<CourseDto> courses, IEnrollmentRepository enrollmentRepository, IMapper mapper)
            : base(academicYearDto, courses, enrollmentRepository, mapper)
        {
        }

        public override EnrollmentTypeState EnrollmentTypeState => EnrollmentTypeState.Completed;

        public bool AddCourses(IEnumerable<int> newCoursesIds)
        {
            var result = this.TryAddCoursesWithState(newCoursesIds);
            return result;
        }

        public bool ChangedToPaidState()
        {
            var paidState = EnrollmentStateBase.CreateState<EnrollmentPaidState>(this);
            UpdateState(paidState);
            EnrollmentRepository.UpdateEnrollmentStatus(EnrollmentDto.Id, paidState.EnrollmentTypeState);

            return true;
        }

        public bool RemoveCourses(IEnumerable<int> removeCoursesIds)
        {
            var result = this.TryRemoveCoursesWithState(removeCoursesIds);
            return result;
        }

        public bool ChangedToCancelState()
        {
            var completedState = EnrollmentStateBase.CreateState<EnrollmentCancelledState>(this);
            UpdateState(completedState);
            EnrollmentRepository.UpdateEnrollmentStatus(EnrollmentDto.Id, completedState.EnrollmentTypeState);

            return true;
        }
    }
}
