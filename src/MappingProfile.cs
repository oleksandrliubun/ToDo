using AutoMapper;
using ToDo.Models.TaskViewModels;
namespace ToDo
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TaskViewModel, Data.Task>();
            CreateMap<Data.Task, TaskViewModel>();
        }
    }
}
