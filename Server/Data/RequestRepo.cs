using Microsoft.EntityFrameworkCore;
using NarLib;
using Server.Models;
using System.Diagnostics.CodeAnalysis;

namespace Server.Data
{
	public class RequestRepo : IRequestRepo
	{
		private readonly AppDbContext _dbContext;

		public RequestRepo(AppDbContext dbContext) => _dbContext = dbContext;

		public bool Add(ControlRequest request)
		{
			if(Exists(request))
				return false;

			_dbContext.Requests.Add(request);

			return true;
		}

		public bool Exists(ControlRequest request) => _dbContext.Requests.Any(e => (e.Id == request.Id && e.ClientId == e.Id));

		public bool Exists(int id) => _dbContext.Requests.Any(e => e.Id == id);

		public ControlRequest Get(int id) => _dbContext.Requests.Include(e => e.Client).FirstOrDefault(e => e.Id == id);

		public IEnumerable<ControlRequest> GetAll() => _dbContext.Requests.Include(e => e.Client).ToList();

		public ICollection<ControlRequest> GetForClient(int clientId) => _dbContext.Requests.Include(e => e.Client).Where(e => e.ClientId == clientId).ToList();

		public void Remove(int id)
		{

			var clientToDelete = new ControlRequest() { Id = id };
			var local = _dbContext.Set<ControlRequest>().Local.FirstOrDefault(e => e.Id == id);

			RemovePrivate(clientToDelete, local);
		}

		private void RemovePrivate(ControlRequest clientToDelete, ControlRequest local)
		{
			if (local != null)
				_dbContext.Requests.Remove(local);
			else
			{
				_dbContext.Requests.Attach(clientToDelete);
				_dbContext.Requests.Remove(clientToDelete);
			}
		}

		public bool SaveChanges() => _dbContext.SaveChanges() >= 0;
	}
}
