using Microsoft.EntityFrameworkCore;
using NarLib;
using Server.Models;

namespace Server.Data
{
	public static class PrepDb
	{
		public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
		{
			using (var serviceScope = app.ApplicationServices.CreateScope())
			{
				SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
			}
		}

		private static void SeedData(AppDbContext context, bool isProduction)
		{

			if (isProduction)
			{
				Console.WriteLine("--> Attempting to apply migrations...");
				try
				{
					context.Database.Migrate();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"--> Could not run migration: {ex.Message}");
				}
			}

			if (!context.Clients.Any())
			{
				Console.WriteLine("--> Seeding CLIENT data...");

				context.Clients.AddRange(new[]
				{
					new Client()
					{
						Hostname= "MyFantasticName", LastUpdateUtcTime= DateTime.UtcNow, LocalIp = new System.Net.IPAddress(new byte[] {192, 168, 69, 10}),
						Os = "Windows 10", ProgramVersion = "0.0.69", PublicIp = new System.Net.IPAddress(new byte[] {13, 37, 13, 37})
					},
				});
				context.SaveChanges();
			}
			else
			{
				Console.WriteLine("--> We already have CLIENT data");
			}

			if (!context.Files.Any())
			{
				Console.WriteLine("--> Seeding REQUEST data...");

				context.Requests.AddRange(new[]
				{
					new ControlRequest()
					{ 
						ClientId = 1, Files = new[] { "test.png" }, Request= "test", Result="what to write here...", 
						Status = ControlRequestStatus.Succeeded, Type = ControlRequestType.InternalFunction
					},
				});
				context.SaveChanges();
			}
			else
			{
				Console.WriteLine("--> We already have REQUEST data");
			}
		}
	}
}
