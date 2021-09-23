using System;
using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandService.EventProcessing
{
	public class EventProcessor : IEventProcessor
	{
		private readonly IServiceScopeFactory _scopeFactory;
		// private readonly ICommandRepo _repo;
		private readonly IMapper _mapper;

		public EventProcessor(
            IServiceScopeFactory scopeFactory,
            // ICommandRepo repo,
            IMapper mapper)
		{
			_scopeFactory = scopeFactory;
            //  Cannot consume scoped service 'CommandService.Data.ICommandRepo' from singleton 'CommandService.EventProcessing.IEventProcessor'
            // _repo = repo;
			_mapper = mapper;
		}

		public void ProcessEvent(string message)
		{
			var eventType = DetermineEvent(message);

			switch (eventType)
			{
				case EventType.PlatformPublished:
					addPlatform(message);
					break;
				default:
					break;
			}
		}

		private EventType DetermineEvent(string notificationMessage)
		{
			Console.WriteLine("determining event");

			var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

			switch (eventType.Event)
			{
				case "Platform_Publisished":
					Console.WriteLine("Platform published event detected");
					return EventType.PlatformPublished;
				default:
					Console.WriteLine("Could not determine the event type");
					return EventType.Undetermined;
			}
		}

		private void addPlatform(string platformPublishedMessage)
		{
			using (var scope = _scopeFactory.CreateScope())
			{
				var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

				var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

				try
				{
					var plat = _mapper.Map<Platform>(platformPublishedDto);
					if (!repo.ExternalPlatformExists(plat.ExternalId))
					{
						repo.CreatePlatform(plat);
						repo.SaveChanges();
						Console.WriteLine("--> Platform added!");
					}
					else
					{
						Console.WriteLine("platform already exisits");
					}
				}
				catch (Exception e)
				{
					Console.WriteLine($"could not add platform to DB {e.Message}");
				}
			}
		}
	}

	enum EventType
	{
		PlatformPublished,
		Undetermined
	}
}