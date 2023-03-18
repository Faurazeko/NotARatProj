using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
	public class FileRepo : IFileRepo
	{
		private readonly AppDbContext _dbContext;

		public FileRepo(AppDbContext dbContext) => _dbContext = dbContext;

		public bool Add(UploadedFile file)
		{
			if (Exists(file))
				return false;

			_dbContext.Files.Add(file);

			return true;
		}

		public bool Exists(UploadedFile file) => Exists(file.Filename);

		public bool Exists(string filename) => _dbContext.Files.Any( e => e.Filename == filename );

		public bool Exists(int id) => _dbContext.Files.Any(e => e.Id == id);

		public UploadedFile Get(int id) => _dbContext.Files.FirstOrDefault(e => e.Id == id);

		public IEnumerable<UploadedFile> GetAll() => _dbContext.Files.ToList();

		public IEnumerable<UploadedFile> GetForClient(int clientId)  => _dbContext.Files.Where(e => e.ClientId == clientId).ToList();

		public void Remove(int id)
		{
			var fileToDelete = new UploadedFile() { Id = id };
			var local = _dbContext.Set<UploadedFile>().Local.FirstOrDefault(e => e.Id.Equals(id));

			RemovePrivate(fileToDelete, local);
		}

		public void Remove(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				throw new ArgumentNullException(nameof(filename));

			var fileToDelete = new UploadedFile() { Filename = filename };
			var local = _dbContext.Set<UploadedFile>().Local.FirstOrDefault(e => e.Filename.Equals(filename));

			RemovePrivate(fileToDelete, local);
		}

		private void RemovePrivate(UploadedFile fileToDelete, UploadedFile local)
		{
			if (local != null)
				_dbContext.Files.Remove(local);
			else
			{
				_dbContext.Files.Attach(fileToDelete);
				_dbContext.Files.Remove(fileToDelete);
			}
		}

		public bool SaveChanges() => _dbContext.SaveChanges() >= 0;
	}
}
