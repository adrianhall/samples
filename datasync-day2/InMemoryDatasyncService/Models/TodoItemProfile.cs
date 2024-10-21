using AutoMapper;

namespace InMemoryDatasyncService.Models;

public class TodoItemProfile : Profile
{
    public TodoItemProfile()
    {
        CreateMap<TodoItem, TodoItemDTO>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.NullSubstitute(DateTimeOffset.UnixEpoch))
            .ReverseMap();
    }
}
