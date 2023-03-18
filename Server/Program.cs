using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Hubs;


namespace Server
{
	public class Program
	{
		public static void Main()
		{
			var builder = WebApplication.CreateBuilder();

			builder.Services.AddControllersWithViews();
			builder.Services.AddScoped<IClientRepo, ClientRepo>();
			builder.Services.AddScoped<IRequestRepo, RequestRepo>();
			builder.Services.AddScoped<IFileRepo, FileRepo>();
			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			builder.Services.AddSignalR();

			builder.Services.AddHostedService<Communicator>();
			builder.Services.AddHostedService<ClientChecker>();

			builder.Services.AddCors(opt =>
			{
				opt.AddPolicy("ILoveSignalr", policty =>
				{
					policty.AllowAnyHeader()
					.AllowAnyMethod()
					.WithOrigins("/")
					.AllowCredentials();
				});
			});


			Console.WriteLine("--> using InMem Db");
			builder.Services.AddDbContext<AppDbContext>(opt =>
			{
				opt.UseInMemoryDatabase("InMem");
			}, ServiceLifetime.Scoped);


			var app = builder.Build();

			app.UseCors("ILoveSignalr");


			if (!app.Environment.IsDevelopment())
				app.UseHsts();

			app.UseStaticFiles(new StaticFileOptions
			{
				ServeUnknownFileTypes = true,
			});

			//app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();


			app.MapControllerRoute(
				name: "default",
				pattern: "{controller}/{action=Index}/{id?}");

			app.MapFallbackToFile("index.html");
			app.MapHub<GeneralHub>("/api/generalHub");

			PrepDb.PrepPopulation(app, !app.Environment.IsDevelopment());

			app.Run();
		}
	}
}
