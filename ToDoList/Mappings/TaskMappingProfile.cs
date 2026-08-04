using AutoMapper;
using ToDoList.Constants;
using ToDoList.DTOs;
using ToDoList.Entities;

namespace ToDoList.Mappings
{
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<CreateTaskRequest, TaskItem>()
                .ForMember(
                dest => dest.DueDate,
                opt => opt.MapFrom(src => src.DueDate ?? DateTime.Today))
                .ForMember(
                dest => dest.StatusId,
                opt => opt.MapFrom(src => StatusConstants.Pending)); 

            CreateMap<TaskItem, TaskResponse>()
                 .ForMember(
                   dest => dest.Id,
                   opt => opt.MapFrom(src => src.TaskItemId))
                 .ForMember(
                    dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.StatusName));

            CreateMap<UpdateTaskRequest, TaskItem>();
        }
    }
}
