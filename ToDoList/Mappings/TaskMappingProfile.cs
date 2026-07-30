using AutoMapper;
using ToDoList.DTOs;
using ToDoList.Entities;

namespace ToDoList.Mappings
{
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<CreateTaskRequest, TaskItem>();

            CreateMap<TaskItem, TaskResponse>()
                .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.StatusName));

            CreateMap<UpdateTaskRequest, TaskItem>();
        }
    }
}
