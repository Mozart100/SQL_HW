using AutoMapper;
using SqlUniversity.DataAccess.Models;
using SqlUniversity.DataAccess.Repository;
using SqlUniversity.Infrastracture;
using SqlUniversity.Model.Dtos;

namespace SqlUniversity.Services.EnrollmentStateMachine
{
    public class EnrollmentInProgressState : EnrollmentStateBase, IChangeCoursesState, ICompleteRegistrationState, ICancelledState
    {
        public EnrollmentInProgressState(AcademicYearDto academicYearDto, IEnumerable<CourseDto> courses, IEnrollmentRepository enrollmentRepository, IMapper mapper)
            : base(academicYearDto, courses, enrollmentRepository, mapper)
        {

        }

        public override EnrollmentTypeState EnrollmentTypeState => EnrollmentTypeState.InProgress;

        public bool AddCourses(IEnumerable<int> newCoursesIds)
        {
            var result = this.TryAddCoursesWithState(newCoursesIds);
            return result;
        }

        public bool RemoveCourses(IEnumerable<int> removeCoursesIds)
        {
            var result = this.TryRemoveCoursesWithState(removeCoursesIds);
            return result;
        }

        public bool ChangedToCompletion()
        {
            var result = false;

            //Student can finish registration if at least a single  course opt.
            if (EnrollmentDto.Courses.SafeAny())
            {
                var completedState = EnrollmentStateBase.CreateState<EnrollmentCompletedState>(this);
                UpdateState(completedState);
                EnrollmentRepository.UpdateEnrollmentStatus(EnrollmentDto.Id, completedState.EnrollmentTypeState);
                result = true;
            }

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
