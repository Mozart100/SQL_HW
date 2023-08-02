using AutoMapper;
using SqlUniversity.DataAccess.Models;
using SqlUniversity.DataAccess.Repository;
using SqlUniversity.Model.Dtos;

namespace SqlUniversity.Services.EnrollmentStateMachine
{
    public interface IEnrollmentStateBase
    {
        EnrollmentDto EnrollmentDto { get; }
    }

    public interface ICancelledState : IEnrollmentStateBase
    {
        bool ChangedToCancelState();

    }

    public interface IPaidState : IEnrollmentStateBase
    {
        bool ChangedToPaidState();
    }

    public interface ICompleteRegistrationState : IEnrollmentStateBase
    {
        bool ChangedToCompletion();
    }

    public interface IChangeCoursesState : IEnrollmentStateBase
    {
        bool AddCourses(IEnumerable<int> newCoursesIds);
        bool RemoveCourses(IEnumerable<int> removeCoursesIds);
    }


    public class EnrollmentStateMachine
    {
        public EnrollmentStateMachine(EnrollmentInProgressState state)
        {
            CurrentState = state;
            state.SetStateCallback(ChangeStateTo);
        }

        public EnrollmentStateBase CurrentState { get; set; }

        public bool TryCaseToState<TState>(out TState enrollmentCompletedState)
        {
            var result = false;
            enrollmentCompletedState = default(TState);

            if (CurrentState is TState state)
            {
                enrollmentCompletedState = state;
                result = true;
            }

            return result;
        }

        private void ChangeStateTo(EnrollmentStateBase newState)
        {
            CurrentState = newState;
        }
    }

    public abstract class EnrollmentStateBase
    {
        public readonly IEnrollmentRepository EnrollmentRepository;
        public readonly IMapper Mapper;
        public readonly Dictionary<int, CourseDto> Courses;
        protected Action<EnrollmentStateBase> StateCallback;

        public EnrollmentStateBase(AcademicYearDto academicYearDto,
            IEnumerable<CourseDto> courses,
            IEnrollmentRepository enrollmentRepository,
            IMapper mapper)
        {
            AcademicYearDto = academicYearDto;
            this.EnrollmentRepository = enrollmentRepository;
            this.Mapper = mapper;


            Courses = new Dictionary<int, CourseDto>();

            foreach (var course in courses)
            {
                Courses.Add(course.Id, course);
            }
        }

        public EnrollmentDto EnrollmentDto { get; private set; }
        public AcademicYearDto AcademicYearDto { get; }
        public void SetStateCallback(Action<EnrollmentStateBase> callback)
        {
            StateCallback = callback;
        }

        public static TState CreateState<TState>(EnrollmentStateBase state) where TState : EnrollmentStateBase//, new()
        {
            var instance = (TState)Activator.CreateInstance(typeof(TState), state.AcademicYearDto, state.Courses.Values, state.EnrollmentRepository, state.Mapper);
            instance.SetEnrollmentDto(state.EnrollmentDto);
            instance.SetStateCallback(state.StateCallback);
            instance.EnrollmentDto.TypeState = instance.EnrollmentTypeState;
            return instance;
        }

        public void SetEnrollmentDto(EnrollmentDto enrollmentDto)
        {
            EnrollmentDto = enrollmentDto;
        }

        public void UpdateState(EnrollmentStateBase newState)
        {
            StateCallback(newState);
        }

        public abstract EnrollmentTypeState EnrollmentTypeState { get; }

        public virtual Enrollment SaveState(EnrollmentDto enrollmentDto)
        {
            SetEnrollmentDto(enrollmentDto);

            var enrollment = Mapper.Map<Enrollment>(enrollmentDto);
            var saveEnrollment = EnrollmentRepository.Insert(enrollment);
            EnrollmentDto.Id = saveEnrollment.Id;

            return saveEnrollment;
        }
    }
}
