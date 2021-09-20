using System.Collections.Generic;
using PlatformService.Models;

namespace PlatformService.Data
{
	public interface IPlatformRepository
	{
		bool SaveChanged();
		IEnumerable<Platform> GetAllPlatforms();
		Platform GetPlatformById(int id);
		void CreatePlatform(Platform platform);
	}
}