using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CommandService.AsyncDataServices;
using CommandService.Data;
using CommandService.EventProcessing;
using CommandService.Profiles;
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

namespace CommandService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMem"));
            services.AddScoped<ICommandRepo, CommandRepo>();
            services.AddControllers();
            services.AddHostedService<MessageBusSubscriber>();

            // services.AddScoped<IEventProcessor, EventProcessor>();
            services.AddSingleton<IEventProcessor, EventProcessor>();
            // services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // register AutoMapper as Singleton
            // https://stackoverflow.com/a/40275196
            // var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new CommandProfiles()));
            // IMapper mapper = mappingConfig.CreateMapper();
            // services.AddSingleton(mapper);

            // register AutoMapper as Singleton 2
            // https://ryuichi111std.hatenablog.com/entry/2016/11/18/005238
            // AutoMapperをプロファイルを元に初期化してサービスに登録
            services.AddAutoMapper(cfg => cfg.AddProfile<CommandProfiles>());
            // IMapper型に対してMapperオブジェクトをsingletonでDIする様に定義を追加
            services.AddSingleton<IMapper, Mapper>();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CommandService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CommandService v1"));
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
        }
    }
}
