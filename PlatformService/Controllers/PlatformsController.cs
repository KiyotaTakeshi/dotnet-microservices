using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PlatformsController : ControllerBase
	{
		private readonly IPlatformRepository _repository;
		private readonly IMapper _mapper;
		private readonly ICommandDataClient _commandDataClient;
		private readonly IMessageBusClient _messageBusClient;

		public PlatformsController(
			IPlatformRepository repository,
			IMapper mapper,
			ICommandDataClient commandDataClient,
			IMessageBusClient messageBusClient
			)
		{
			_repository = repository;
			_mapper = mapper;
			_commandDataClient = commandDataClient;
			_messageBusClient = messageBusClient;
		}

		[HttpGet]
		public ActionResult<IEnumerable<PlatformReadDto>> GetPlatForms()
		{
			Console.WriteLine("Getting platforms");
			var platfomrItems = _repository.GetAllPlatforms();

			return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platfomrItems));
		}

		[HttpGet("{id}", Name = "GetPlatformById")]
		public ActionResult<PlatformReadDto> GetPlatformById(int id)
		{
			var platformItem = _repository.GetPlatformById(id);

			if (platformItem != null)
				return Ok(_mapper.Map<PlatformReadDto>(platformItem));

			return NotFound();
		}

		[HttpPost]
		public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
		{
			var platformModel = _mapper.Map<Platform>(platformCreateDto);
			_repository.CreatePlatform(platformModel);
			_repository.SaveChanged();

			var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

			// Send Sync Message
			try
			{
				await _commandDataClient.SendPlatformToCommand(platformReadDto).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Could not send synchronously: {ex.Message}");
			}

			// Send Async Message
			try
			{
				var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
				platformPublishedDto.Event = "Platform_Publisished";
				_messageBusClient.PublishNewPlatform(platformPublishedDto);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Could not send asynchronously: {ex.Message}");
			}

			return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
		}

	}
}