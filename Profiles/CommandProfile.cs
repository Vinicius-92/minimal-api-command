using AutoMapper;

public class CommandProfile : Profile
{
    public CommandProfile()
    {
        CreateMap<Command, CommandReadDTO>();
        CreateMap<CommandCreateDTO, Command>();
        CreateMap<CommandUpdateDTO, Command>();
    }
}
