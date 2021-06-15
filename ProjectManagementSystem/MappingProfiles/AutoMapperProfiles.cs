using AutoMapper;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.ViewModels;


namespace ProjectManagementSystem.MappingProfiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ApplicationUser, RegisterViewModel>().ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.RoleName)).ReverseMap();
            CreateMap<ApplicationUser, EditViewModel>();
            CreateMap<Project, ProjectViewModel>().ReverseMap();
            CreateMap<Task, TaskViewModel>().ForMember(dest => dest.Assignee, opt => opt.NullSubstitute(new ApplicationUser())).ReverseMap();
        }
    }
}
