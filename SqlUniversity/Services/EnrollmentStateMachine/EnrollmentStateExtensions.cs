using SqlUniversity.Infrastracture;

namespace SqlUniversity.Services.EnrollmentStateMachine
{
    public static class EnrollmentStateExtensions
    {
        public static bool TryRemoveCoursesWithState(this EnrollmentStateBase state, IEnumerable<int> removeCoursesIds)
        {
            var studentCourses = new HashSet<int>(state.EnrollmentDto.Courses ?? Array.Empty<int>());
            var result = false;

            foreach (var removeCourseId in removeCoursesIds)
            {
                //If at least a single course removed then should we update in the db.
                studentCourses.Remove(removeCourseId);
            }

            var coutsePointsTotal = 0;
            var mandatoryCourses = new HashSet<int>(state.AcademicYearDto.MandatoryCourses);
            foreach (var courseId in studentCourses)
            {
                if (state.Courses.TryGetValue(courseId, out var course))
                {
                    coutsePointsTotal += course.CoursePoints;
                    mandatoryCourses.Remove(courseId);
                }
            }

            //At least a single course was removed, hence update to db is required.
            if (state.EnrollmentDto.Courses.SafeCount() != studentCourses.Count)
            {
                state.EnrollmentDto.Courses = studentCourses.ToArray();

                if (!mandatoryCourses.Any() || state.AcademicYearDto.RequiredPoints > coutsePointsTotal)
                {
                    state.EnrollmentDto.IsPassedThreshold = false;
                    var completedState = EnrollmentStateBase.CreateState<EnrollmentInProgressState>(state);
                    state.UpdateState(completedState);
                    state.EnrollmentRepository.UpdateCoursesAndState(state.EnrollmentDto.Id, studentCourses.ToArray(), completedState.EnrollmentTypeState);
                }
                else
                {
                    state.EnrollmentRepository.UpdateCourses(state.EnrollmentDto.Id, studentCourses.ToArray());
                    state.EnrollmentDto.IsPassedThreshold = true;
                }

                result = true;
            }

            return result;

        }

        public static bool TryAddCoursesWithState(this EnrollmentStateBase state, IEnumerable<int> newCoursesIds)
        {
            var studentCourses = new HashSet<int>(state.EnrollmentDto.Courses ?? Array.Empty<int>());
            var result = false;

            foreach (var courseId in newCoursesIds)
            {
                //If at least a single course added then should we update in the db.
                studentCourses.Add(courseId);
            }

            var coutsePointsTotal = 0;
            var mandatoryCourses = new HashSet<int>(state.AcademicYearDto.MandatoryCourses);
            foreach (var courseId in studentCourses)
            {
                if (state.Courses.TryGetValue(courseId, out var course))
                {
                    coutsePointsTotal += course.CoursePoints;
                    mandatoryCourses.Remove(courseId);
                }
            }

            //At least a single course was added, hence update to db is required.
            if (state.EnrollmentDto.Courses.SafeCount() != studentCourses.Count)
            {
                state.EnrollmentDto.Courses = studentCourses.ToArray();

                if (!mandatoryCourses.Any() && state.AcademicYearDto.RequiredPoints <= coutsePointsTotal)
                {
                    state.EnrollmentDto.IsPassedThreshold = true;
                    var completedState = EnrollmentStateBase.CreateState<EnrollmentCompletedState>(state);
                    state.UpdateState(completedState);
                    state.EnrollmentRepository.UpdateCoursesAndState(state.EnrollmentDto.Id, studentCourses.ToArray(), completedState.EnrollmentTypeState);
                }
                else
                {
                    state.EnrollmentRepository.UpdateCourses(state.EnrollmentDto.Id, studentCourses.ToArray());
                    state.EnrollmentDto.IsPassedThreshold = false;
                }

                result = true;
            }

            return result;
        }
    }
}
