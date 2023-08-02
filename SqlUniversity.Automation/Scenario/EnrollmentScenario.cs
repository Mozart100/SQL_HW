using SqlUniversity.Model.Dtos;
using SqlUniversity.Model.Requests;
using Xunit;
using SqlUniversity.Infrastracture;
using SqlUniversity.DataAccess.Models;
using SqlUniversity.Controllers;

namespace SqlUniversity.Automation.Scenario
{
    public class EnrollmentScenario : ScenarioBase
    {
        public EnrollmentScenario(string baseUrl) : base(baseUrl)
        {
        }

        public override string ScenarioName => "Enrollment Scenario";

        private string RegistrationUrl => $"{BaseUrl}/{EnrollmentController.RegistrationRoute}";
        private string FinishRegistrationUrl => $"{BaseUrl}/{EnrollmentController.FinishRegistrationRoute}";
        private string CanceledUrl => $"{BaseUrl}/{EnrollmentController.CancelRoute}";
        private string PaidUrl => $"{BaseUrl}/paid";

        protected override async Task RunScenario()
        {
            var userNoumber = 1;
            Console.WriteLine($"Sanity for User {userNoumber} Started !!!!!!");
            await Smoke_Test(1);
            Console.WriteLine($"Sanity for User {userNoumber++} finished successfully !!!!!!");



            Console.WriteLine($"Sanity for User {userNoumber} Started !!!!!!");
            await Smoke_Test(userNoumber);
            Console.WriteLine($"Sanity for User {userNoumber++} finished successfully !!!!!!");

            Console.WriteLine($"Sanity for User {userNoumber} Started !!!!!!");
            await Registration_AddCourses_AddCourses_Paying(userNoumber);
            Console.WriteLine($"Sanity for User {userNoumber++} finished successfully !!!!!!");


            Console.WriteLine($"Sanity for User {userNoumber} Started !!!!!!");
            await Registration_FalsePaying_AddingCourse_ValidPaying(userNoumber);
            Console.WriteLine($"Sanity for User {userNoumber++} finished successfully !!!!!!");


            Console.WriteLine($"Sanity for User {userNoumber} Started !!!!!!");
            await Registration_AddCourse_ValidCompleting(userNoumber);
            Console.WriteLine($"Sanity for User {userNoumber++} finished successfully !!!!!!");


            Console.WriteLine($"Sanity for User {userNoumber} Started !!!!!!");
            await Registration_FalsePaying_AddCoursing_Paying(userNoumber);
            Console.WriteLine($"Sanity for User {userNoumber++} finished successfully !!!!!!");


            Console.WriteLine($"Sanity for User {userNoumber} Started !!!!!!");
            await Registration_AddCoursing_ValidCompletion_ValidCancelled(userNoumber);
            Console.WriteLine($"Sanity for User {userNoumber++} finished successfully !!!!!!");

            Console.WriteLine($"Sanity for User {userNoumber} Started !!!!!!");
            await Registration_ValidCancelled(userNoumber);
            Console.WriteLine($"Sanity for User {userNoumber++} finished successfully !!!!!!");

            Console.WriteLine($"Sanity for User {userNoumber} Started !!!!!!");
            await InValidCancelled(userNoumber);
            Console.WriteLine($"Sanity for User {userNoumber++} finished successfully !!!!!!");

            Console.WriteLine($"Sanity for User {userNoumber} Started !!!!!!");
            await Registration_AddCourses_ValidCancelled(userNoumber);
            Console.WriteLine($"Sanity for User {userNoumber++} finished successfully !!!!!!");

            //--------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------
            //--------------------------------------------------------------------------------------------------


            Console.WriteLine($"Sanity for User invalid {userNoumber} Started !!!!!!");
            await InvalidRegistration(userNoumber);
            Console.WriteLine($"Sanity for User invalid {userNoumber++} finished successfully !!!!!!");

            Console.WriteLine($"Sanity for User invalid {userNoumber} Started !!!!!!");
            await Registration_InvalidAddCourses(userNoumber);
            Console.WriteLine($"Sanity for User invalid {userNoumber++} finished successfully !!!!!!");


            Console.WriteLine($"Sanity for User invalid {userNoumber} Started !!!!!!");
            await Registration_AddCourses_InvalidRemoveCourses(userNoumber);
            Console.WriteLine($"Sanity for User invalid {userNoumber++} finished successfully !!!!!!");


            Console.WriteLine($"Sanity for User invalid {userNoumber} Started !!!!!!");
            await Registration_AddCourses_CancelEnrollment_InvalidRemoveAllCourses(userNoumber);
            Console.WriteLine($"Sanity for User invalid {userNoumber++} finished successfully !!!!!!");

            Console.WriteLine($"Sanity for User invalid {userNoumber} Started !!!!!!");
            await Registration_InvalidFinishRegistration(userNoumber);
            Console.WriteLine($"Sanity for User invalid {userNoumber++} finished successfully !!!!!!");

            Console.WriteLine($"Sanity for User invalid {userNoumber} Started !!!!!!");
            await Registration_AddCourses_InvalidPayment(userNoumber);
            Console.WriteLine($"Sanity for User invalid {userNoumber++} finished successfully !!!!!!");

            Console.WriteLine($"Sanity for User invalid {userNoumber} Started !!!!!!");
            await Registration_AddCourses_Payed_InvalidCancellation(userNoumber);
            Console.WriteLine($"Sanity for User invalid {userNoumber++} finished successfully !!!!!!");

        }


        private async Task Registration_AddCourses_Payed_InvalidCancellation(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);


            var courses = new List<int> { 1, 2, 3, 4, 5 };
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Completed, addCoursesEnrollmentResponse.TypeState);

            var payedEnrollment = await RunPostCommand<PaidEnrollmentRequest, PaidEnrollmentResponse>($"{PaidUrl}/{enrollmentId}", new PaidEnrollmentRequest(), isPostRequest: false);
            Assert.True(payedEnrollment.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Payed, payedEnrollment.TypeState);
            Assert.Null(payedEnrollment.Errors);

            var cancelledEnrollmentResponse = await DeleteCommand<CancelledEnrollmentResponse>($"{CanceledUrl}/{enrollmentId}");
            Assert.False(cancelledEnrollmentResponse.IsOperationPassed);
            Assert.NotEmpty(cancelledEnrollmentResponse.Errors);

        }

        private async Task Registration_AddCourses_InvalidPayment(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);


            var courses = new HashSet<int>(new List<int> { 1, 2 });
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.InProgress, addCoursesEnrollmentResponse.TypeState);


            var payedEnrollment = await RunPostCommand<PaidEnrollmentRequest, PaidEnrollmentResponse>($"{PaidUrl}/{enrollmentId}", new PaidEnrollmentRequest(), isPostRequest: false);
            Assert.False(payedEnrollment.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.InProgress, payedEnrollment.TypeState);
            Assert.NotEmpty(payedEnrollment.Errors);
        }

        private async Task Registration_InvalidFinishRegistration(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);


            var finishRegistrationResponse = await RunPostCommand<FinishRegistrationEnrollmentRequest, FinishRegistrationEnrollmentResponse>($"{FinishRegistrationUrl}/{enrollmentId}", new FinishRegistrationEnrollmentRequest(), isPostRequest: false);
            Assert.Equal(EnrollmentTypeState.InProgress, finishRegistrationResponse.TypeState);
            Assert.False(finishRegistrationResponse.IsOperationPassed);
            Assert.NotEmpty(finishRegistrationResponse.Errors);

        }


        private async Task Registration_AddCourses_CancelEnrollment_InvalidRemoveAllCourses(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);


            var courses = new HashSet<int>(new List<int> { 1, 2, 3, 4, 5 });
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Completed, addCoursesEnrollmentResponse.TypeState);
            Assert.True(addCoursesEnrollmentResponse.IsPassedThreshold);


            var cancelledEnrollmentResponse = await DeleteCommand<CancelledEnrollmentResponse>($"{CanceledUrl}/{enrollmentId}");
            Assert.True(cancelledEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Cancelled, cancelledEnrollmentResponse.TypeState);
            Assert.True(cancelledEnrollmentResponse.IsPassedThreshold);


            var removeAllCoursesEnrollmentResponse = await RunPostCommand<RemoveAllCoursesEnrollmentRequest, RemoveAllCoursesEnrollmentResponse>($"{BaseUrl}/removeallcourses/{enrollmentId}", new RemoveAllCoursesEnrollmentRequest(), isPostRequest: false);
            Assert.False(removeAllCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Cancelled, removeAllCoursesEnrollmentResponse.TypeState);
            Assert.NotEmpty(removeAllCoursesEnrollmentResponse.Errors);

        }

        private async Task Registration_AddCourses_InvalidRemoveCourses(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);


            var courses = new HashSet<int>(new List<int> { 1, 2, 3, });
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.InProgress, addCoursesEnrollmentResponse.TypeState);
            Assert.False(addCoursesEnrollmentResponse.IsPassedThreshold);

            //Removing without specifing any courses
            var removeCoursesEnrollmentResponse = await RunPostCommand<RemoveCoursesEnrollmentRequest, RemoveCoursesEnrollmentResponse>($"{BaseUrl}/removecourses/{enrollmentId}", new RemoveCoursesEnrollmentRequest { CoursesIds = new int[] { } }, isPostRequest: false);
            Assert.Equal(courses.ToArray(), removeCoursesEnrollmentResponse.Courses);
            Assert.Equal(EnrollmentTypeState.InProgress, removeCoursesEnrollmentResponse.TypeState);
            Assert.False(removeCoursesEnrollmentResponse.IsOperationPassed);
            Assert.NotEmpty(removeCoursesEnrollmentResponse.Errors);


            //Removing with wrong courseIds
            var courseToRemove = 1000;
            var coursesToRemove = new int[] { courseToRemove };
            removeCoursesEnrollmentResponse = await RunPostCommand<RemoveCoursesEnrollmentRequest, RemoveCoursesEnrollmentResponse>($"{BaseUrl}/removecourses/{enrollmentId}", new RemoveCoursesEnrollmentRequest { CoursesIds = coursesToRemove }, isPostRequest: false);
            Assert.Equal(courses.ToArray(), removeCoursesEnrollmentResponse.Courses);
            Assert.Equal(EnrollmentTypeState.InProgress, removeCoursesEnrollmentResponse.TypeState);
            Assert.False(removeCoursesEnrollmentResponse.IsOperationPassed);
            Assert.NotEmpty(removeCoursesEnrollmentResponse.Errors);

        }



        private async Task Registration_InvalidAddCourses(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);


            var courses = new HashSet<int>(new List<int>());
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.False(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.InProgress, addCoursesEnrollmentResponse.TypeState);
            Assert.False(addCoursesEnrollmentResponse.IsPassedThreshold);
            Assert.NotEmpty(addCoursesEnrollmentResponse.Errors);



            courses = new HashSet<int>(new List<int> { 23, 66 });
            addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.False(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.InProgress, addCoursesEnrollmentResponse.TypeState);
            Assert.False(addCoursesEnrollmentResponse.IsPassedThreshold);
            Assert.NotEmpty(addCoursesEnrollmentResponse.Errors);


        }
        private async Task InvalidRegistration(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = 0 });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.False(createEnrollmentResponse.IsOperationPassed);
            Assert.NotNull(createEnrollmentResponse.Errors);

        }

        private async Task Registration_AddCourses_ValidCancelled(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);


            var courses = new HashSet<int>(new List<int> { 1, 2, 3, 4, 5 });
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Completed, addCoursesEnrollmentResponse.TypeState);
            Assert.True(addCoursesEnrollmentResponse.IsPassedThreshold);


            var cancelledEnrollmentResponse = await DeleteCommand<CancelledEnrollmentResponse>($"{CanceledUrl}/{enrollmentId}");
            Assert.True(cancelledEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Cancelled, cancelledEnrollmentResponse.TypeState);
            Assert.True(cancelledEnrollmentResponse.IsPassedThreshold);

        }

        private async Task InValidCancelled(int userId)
        {
            var enrollmentId = userId;
            var cancelledEnrollmentResponse = await DeleteCommand<CancelledEnrollmentResponse>($"{CanceledUrl}/{enrollmentId}");
            Assert.False(cancelledEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.InProgress, cancelledEnrollmentResponse.TypeState);
            Assert.NotEmpty(cancelledEnrollmentResponse.Errors);
        }

        private async Task Registration_ValidCancelled(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);

            var cancelledEnrollmentResponse = await DeleteCommand<CancelledEnrollmentResponse>($"{CanceledUrl}/{enrollmentId}");
            Assert.True(cancelledEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Cancelled, cancelledEnrollmentResponse.TypeState);
            Assert.False(cancelledEnrollmentResponse.IsPassedThreshold);

        }

        private async Task Registration_AddCoursing_ValidCompletion_ValidCancelled(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);

            var courses = new HashSet<int>(new List<int> { 1, 2, 3, 4, 5 });
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Completed, addCoursesEnrollmentResponse.TypeState);
            Assert.True(addCoursesEnrollmentResponse.IsPassedThreshold);

            var cancelledEnrollmentResponse = await DeleteCommand<CancelledEnrollmentResponse>($"{CanceledUrl}/{enrollmentId}");
            Assert.True(cancelledEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Cancelled, cancelledEnrollmentResponse.TypeState);
            Assert.True(cancelledEnrollmentResponse.IsPassedThreshold);

        }

        private async Task Registration_FalsePaying_AddCoursing_Paying(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);


            var payedEnrollment = await RunPostCommand<PaidEnrollmentRequest, PaidEnrollmentResponse>($"{PaidUrl}/{enrollmentId}", new PaidEnrollmentRequest(), isPostRequest: false);
            Assert.False(payedEnrollment.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.InProgress, payedEnrollment.TypeState);
            Assert.False(payedEnrollment.IsPassedThreshold);


            var courses = new HashSet<int>(new List<int> { 1, 2, 3, 4, 5 });
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Completed, addCoursesEnrollmentResponse.TypeState);
            Assert.True(addCoursesEnrollmentResponse.IsPassedThreshold);



            payedEnrollment = await RunPostCommand<PaidEnrollmentRequest, PaidEnrollmentResponse>($"{PaidUrl}/{enrollmentId}", new PaidEnrollmentRequest(), isPostRequest: false);
            Assert.True(payedEnrollment.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Payed, payedEnrollment.TypeState);
            Assert.True(payedEnrollment.IsPassedThreshold);

        }

        //xxxxxxxxxx
        private async Task Registration_AddCourse_ValidCompleting(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);


            var courses = new HashSet<int>(new List<int> { 1, 2 });
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.Equal(addCoursesEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(addCoursesEnrollmentResponse.IsPassedThreshold);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);


            var finishRegistrationResponse = await RunPostCommand<FinishRegistrationEnrollmentRequest, FinishRegistrationEnrollmentResponse>($"{FinishRegistrationUrl}/{enrollmentId}", new FinishRegistrationEnrollmentRequest(), isPostRequest: false);
            Assert.True(finishRegistrationResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Completed, finishRegistrationResponse.TypeState);
            Assert.False(finishRegistrationResponse.IsPassedThreshold);
        }

        private async Task Registration_FalsePaying_AddingCourse_ValidPaying(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);


            var payedEnrollment = await RunPostCommand<PaidEnrollmentRequest, PaidEnrollmentResponse>($"{PaidUrl}/{enrollmentId}", new PaidEnrollmentRequest(), isPostRequest: false);
            Assert.False(payedEnrollment.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.InProgress, payedEnrollment.TypeState);
            Assert.False(payedEnrollment.IsPassedThreshold);


            var courses = new HashSet<int>(new List<int> { 1, 2, 3, 4, 5 });
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Completed, addCoursesEnrollmentResponse.TypeState);
            Assert.True(addCoursesEnrollmentResponse.IsPassedThreshold);


            payedEnrollment = await RunPostCommand<PaidEnrollmentRequest, PaidEnrollmentResponse>($"{PaidUrl}/{enrollmentId}", new PaidEnrollmentRequest(), isPostRequest: false);
            Assert.True(payedEnrollment.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Payed, payedEnrollment.TypeState);
            Assert.True(payedEnrollment.IsPassedThreshold);
        }

        private async Task Registration_AddCourses_AddCourses_Paying(int userId)
        {
            var createEnrollmentResponse = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = createEnrollmentResponse.Id;
            Assert.True(createEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.Equal(createEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(createEnrollmentResponse.IsPassedThreshold);
            Assert.True(createEnrollmentResponse.IsOperationPassed);


            var courses = new HashSet<int>(new List<int> { 1, 2 });
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.Equal(addCoursesEnrollmentResponse.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(addCoursesEnrollmentResponse.IsPassedThreshold);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);


            courses = new HashSet<int>(new List<int> { 3, 4, 5 });
            addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Completed, addCoursesEnrollmentResponse.TypeState);
            Assert.True(addCoursesEnrollmentResponse.IsPassedThreshold);


            var payedEnrollment = await RunPostCommand<PaidEnrollmentRequest, PaidEnrollmentResponse>($"{PaidUrl}/{enrollmentId}", new PaidEnrollmentRequest(), isPostRequest: false);
            Assert.True(payedEnrollment.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.Payed, payedEnrollment.TypeState);
            Assert.True(payedEnrollment.IsPassedThreshold);
        }



        private async Task Smoke_Test(int userId)
        {
            var firstEnrolment = await RunPostCommand<CreateEnrollmentRequest, CreateEnrollmentResponse>(RegistrationUrl, new CreateEnrollmentRequest { StuentId = userId });
            var enrollmentId = firstEnrolment.Id;

            Assert.True(firstEnrolment.Courses.IsNullOrEmpty());
            Assert.Equal(firstEnrolment.TypeState, EnrollmentTypeState.InProgress);
            Assert.False(firstEnrolment.IsPassedThreshold);
            Assert.True(firstEnrolment.IsOperationPassed);
            Assert.Null(firstEnrolment.Errors);

            var courses = new HashSet<int>(new List<int> { 2, 3 });
            var addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.Equal(courses.ToArray(), addCoursesEnrollmentResponse.Courses);
            Assert.False(addCoursesEnrollmentResponse.IsPassedThreshold);
            Assert.Equal(EnrollmentTypeState.InProgress, addCoursesEnrollmentResponse.TypeState);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Null(addCoursesEnrollmentResponse.Errors);


            var finishRegistrationResponse = await RunPostCommand<FinishRegistrationEnrollmentRequest, FinishRegistrationEnrollmentResponse>($"{FinishRegistrationUrl}/{enrollmentId}", new FinishRegistrationEnrollmentRequest(), isPostRequest: false);
            Assert.False(finishRegistrationResponse.IsPassedThreshold);
            Assert.Equal(EnrollmentTypeState.Completed, finishRegistrationResponse.TypeState);
            Assert.False(finishRegistrationResponse.IsPassedThreshold);

            var additionCourses = new int[] { 1, 5, 6 };
            addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = additionCourses }, isPostRequest: false);
            foreach (var additionCourse in additionCourses)
            {
                courses.Add(additionCourse);
            }

            Assert.Equal(courses.ToArray(), addCoursesEnrollmentResponse.Courses);
            Assert.True(addCoursesEnrollmentResponse.IsPassedThreshold);
            Assert.Equal(EnrollmentTypeState.Completed, addCoursesEnrollmentResponse.TypeState);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Null(addCoursesEnrollmentResponse.Errors);



            var courseToRemove = 1;
            var coursesToRemove = new int[] { courseToRemove };
            courses.Remove(courseToRemove);
            var removeCoursesEnrollmentResponse = await RunPostCommand<RemoveCoursesEnrollmentRequest, RemoveCoursesEnrollmentResponse>($"{BaseUrl}/removecourses/{enrollmentId}", new RemoveCoursesEnrollmentRequest { CoursesIds = coursesToRemove }, isPostRequest: false);
            Assert.Equal(courses.ToArray(), removeCoursesEnrollmentResponse.Courses);
            Assert.False(removeCoursesEnrollmentResponse.IsPassedThreshold);
            Assert.Equal(EnrollmentTypeState.InProgress, removeCoursesEnrollmentResponse.TypeState);
            Assert.True(removeCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Null(removeCoursesEnrollmentResponse.Errors);
            courses.Clear();



            finishRegistrationResponse = await RunPostCommand<FinishRegistrationEnrollmentRequest, FinishRegistrationEnrollmentResponse>($"{FinishRegistrationUrl}/{enrollmentId}", new FinishRegistrationEnrollmentRequest(), isPostRequest: false);
            Assert.False(finishRegistrationResponse.IsPassedThreshold);
            Assert.Equal(EnrollmentTypeState.Completed, finishRegistrationResponse.TypeState);
            Assert.True(finishRegistrationResponse.IsOperationPassed);
            Assert.Null(finishRegistrationResponse.Errors);



            var removeAllCoursesEnrollmentResponse = await RunPostCommand<RemoveAllCoursesEnrollmentRequest, RemoveAllCoursesEnrollmentResponse>($"{BaseUrl}/removeallcourses/{enrollmentId}", new RemoveAllCoursesEnrollmentRequest(), isPostRequest: false);
            Assert.True(removeAllCoursesEnrollmentResponse.Courses.IsNullOrEmpty());
            Assert.False(removeAllCoursesEnrollmentResponse.IsPassedThreshold);
            Assert.True(removeAllCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Equal(EnrollmentTypeState.InProgress, removeAllCoursesEnrollmentResponse.TypeState);
            Assert.Null(removeAllCoursesEnrollmentResponse.Errors);


            courses = new HashSet<int>(new List<int> { 1, 2, 3, 4, 5 });
            addCoursesEnrollmentResponse = await RunPostCommand<AddCoursesEnrollmentRequest, AddCoursesEnrollmentResponse>($"{BaseUrl}/{enrollmentId}", new AddCoursesEnrollmentRequest { CoursesIds = courses.ToArray() }, isPostRequest: false);
            Assert.True(addCoursesEnrollmentResponse.IsPassedThreshold);
            Assert.Equal(EnrollmentTypeState.Completed, addCoursesEnrollmentResponse.TypeState);
            Assert.True(addCoursesEnrollmentResponse.IsOperationPassed);
            Assert.Null(addCoursesEnrollmentResponse.Errors);


            finishRegistrationResponse = await RunPostCommand<FinishRegistrationEnrollmentRequest, FinishRegistrationEnrollmentResponse>($"{FinishRegistrationUrl}/{enrollmentId}", new FinishRegistrationEnrollmentRequest(), isPostRequest: false);
            Assert.Equal(EnrollmentTypeState.Completed, finishRegistrationResponse.TypeState);
            Assert.False(finishRegistrationResponse.IsOperationPassed);
            Assert.NotNull(finishRegistrationResponse.Errors);
        }
    }
}
