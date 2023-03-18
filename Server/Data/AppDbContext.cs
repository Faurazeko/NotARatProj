using Microsoft.EntityFrameworkCore;
using NarLib;
using Server.Models;

namespace Server.Data
{
	public class AppDbContext : DbContext
	{
		public DbSet<Client> Clients { get; set; }
		public DbSet<ControlRequest> Requests { get; set; }
		public DbSet<UploadedFile> Files { get; set; }
		public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }
	}
}
