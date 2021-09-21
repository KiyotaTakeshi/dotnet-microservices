using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;

namespace PlatformService.SyncDataService.Http
{
	public class HttpCommandDataClient : ICommandDataClient
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;

		public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
            _configuration = configuration;
		}

		public async Task SendPlatformToCommand(PlatformReadDto platform)
		{
			var httpContent = new StringContent(
				JsonSerializer.Serialize(platform),
				Encoding.UTF8,
				"application/json");

            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent).ConfigureAwait(false);

            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine("sync post to Commandservice is OK");
            }
            else
            {
                Console.WriteLine("sync post to CommandService is failure");
            }
		}
	}
}