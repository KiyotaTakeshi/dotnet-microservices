using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
	public static class PrepareDb
	{
		public static void PreparePopulation(IApplicationBuilder app, bool isProd)
		{
			using (var serviceScope = app.ApplicationServices.CreateScope())
			{
				SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
			}
		}

		private static void SeedData(AppDbContext context, bool isProd)
		{
			if (isProd)
			{
				Console.WriteLine("attempting to apply migrations...");
				try
				{
					context.Database.Migrate();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"could not run migrations: {ex.Message}");
				}
			}
			if (!context.Platforms.Any())
			{
				Console.WriteLine("Seeding data");
				context.Platforms.AddRange(
					new Platform() { Name = ".NET Core", Publisher = "Microsoft", Cost = "Free" },
					new Platform() { Name = "SQL Server", Publisher = "Microsoft", Cost = "Free" },
					new Platform() { Name = "Kubernetes", Publisher = "CNCF", Cost = "Free" }
				);
				context.SaveChanges();
			}
			else
			{
				Console.WriteLine("We already have data");
			}
		}
	}
}