using AutoMapper;
using SqlUniversity.DataAccess.Models;
using SqlUniversity.Model.Dtos;
using SqlUniversity.Model.Requests;

namespace SqlUniversity.Startup
{
    public class ConfigureMapper : Profile
    {
        public ConfigureMapper()
        {
            
            CreateMap<CourseRequest, Course>();
            CreateMap<Course, CourseDto>();

            CreateMap<EnrollmentDto, Enrollment>();
            CreateMap<CreateEnrollmentRequest, Enrollment>();
            CreateMap<Enrollment, EnrollmentDto>();

            
            CreateMap<Enrollment, AddCoursesEnrollmentResponse>();
            CreateMap<Enrollment, RemoveCoursesEnrollmentResponse>();
            CreateMap<Enrollment, RemoveAllCoursesEnrollmentResponse>();
            CreateMap<Enrollment, CancelledEnrollmentResponse>();
            CreateMap<Enrollment, FinishRegistrationEnrollmentResponse>();

            
            CreateMap<Enrollment, PaidEnrollmentResponse>();
            CreateMap<Enrollment, CreateEnrollmentResponse>();
            CreateMap<EnrollmentDto, CreateEnrollmentResponse>();
            CreateMap<EnrollmentDto, RemoveCoursesEnrollmentResponse>();
            CreateMap<EnrollmentDto, RemoveAllCoursesEnrollmentResponse>();
            CreateMap<EnrollmentDto, FinishRegistrationEnrollmentResponse>();
            CreateMap<EnrollmentDto, PaidEnrollmentResponse>();
            CreateMap<EnrollmentDto, CancelledEnrollmentResponse>();
            
            
            CreateMap<CourseRule, CourseRuleResponse>();


            


            CreateMap<CreateEnrollmentResponse, CreateEnrollmentResponse>();
            CreateMap<EnrollmentDto, AddCoursesEnrollmentResponse>();
            



            CreateMap<CourseRuleRequest, CourseRule>();
            CreateMap<CourseRule, CourseRuleDto>();
        }
    }
}
