using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.Profiles
{
	public class CommandProfiles : Profile
	{
		public CommandProfiles()
		{
			CreateMap<Platform, PlatformReadDto>();
			CreateMap<CommandCreateDto, Command>();
			CreateMap<Command, CommandReadDto>();
			CreateMap<PlatformPublishedDto, Platform>()
				.ForMember(d => d.ExternalId, opt => opt.MapFrom(s => s.Id));
		}
	}
}