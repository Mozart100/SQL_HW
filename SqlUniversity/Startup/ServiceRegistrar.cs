using SqlUniversity.DataAccess.Repository;
using SqlUniversity.Services;
using SqlUniversity.Services.Validations;

namespace SqlUniversity.Startup
{
    public static class ServiceRegistrar
    {
        public static IServiceCollection CustomServiceRegistration(this IServiceCollection services)
        {
            services.AddSingleton<IEnrollmentService, EnrollmentService>();
            
            services.AddSingleton<ICourseService, CourseService>();
            services.AddSingleton<ICoursetRepository, CoursetRepository>();
            services.AddSingleton<IEnrollmentRepository, EnrollmentRepository>();
            services.AddSingleton<IAssignCourseYearyRepository, AssignCourseYearyRepository>();
            services.AddSingleton<ICourseRulesRepository, CourseRulesRepository>();
            services.AddSingleton<IAcademicProcessorService, AcademicProcessorService>();
            services.AddSingleton<IEnrollmentValidatorService, EnrollmentValidatorService>();

            return services;
        }

        public static IServiceCollection NativeServiceRegistration(this IServiceCollection services)
        {
           services.AddControllers();
           services.AddEndpointsApiExplorer();
           services.AddSwaggerGen();
           services.AddAutoMapper(typeof(Program).Assembly);

            

            return services;
        }
    }
}