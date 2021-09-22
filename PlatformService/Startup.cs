using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using PlatformService.SyncDataService.Http;

namespace PlatformService
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		private readonly IWebHostEnvironment _env;

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
			_env = env;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			if(_env.IsProduction())
			{
				Console.WriteLine("Using MSSQL DB");
				services.AddDbContext<AppDbContext>(opt =>
					opt.UseSqlServer(Configuration.GetConnectionString("PlatformsConnection")));
			}
			else
			{
				Console.WriteLine("Using InMemory DB");
				services.AddDbContext<AppDbContext>(opt =>
					opt.UseInMemoryDatabase("InMem"));
			}

            services.AddScoped<IPlatformRepository, PlatformRepository>();

			services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

			services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
			});

			Console.WriteLine($"CommandService endpoint {Configuration["CommandService"]}");
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
			}

            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

            PrepareDb.PreparePopulation(app, env.IsProduction());
		}
	}
}
