## for migration

- temporarily rewrite following config to [appsettings.Development.json](appsettings.Development.json)

```json
  "CommandService": "http://localhost:6000/api/c/platforms/",
  "ConnectionStrings":
  {
    "PlatformsConnection": "Server=localhost,1433;Initial Catalog=platformsdb;User ID=sa;Password=1qazxsw2;"
  }
```

- temporarily comment out [Startup.cs](Startup.cs) logic of judge whether to use MSSQL or not

```c#
		public void ConfigureServices(IServiceCollection services)
		{
			// if(_env.IsProduction())
			// {
				Console.WriteLine("Using MSSQL DB");
				services.AddDbContext<AppDbContext>(opt =>
					opt.UseSqlServer(Configuration.GetConnectionString("PlatformsConnection")));
			// }
			// else
			// {
			// 	Console.WriteLine("Using InMemory DB");
			// 	services.AddDbContext<AppDbContext>(opt =>
			// 		opt.UseInMemoryDatabase("InMem"));
			// }
```
