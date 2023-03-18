using Server.Models;
using System.Net.NetworkInformation;

namespace Server.Data
{
	public class ClientRepo : IClientRepo
	{
		private readonly AppDbContext _dbContext;

		public ClientRepo(AppDbContext dbContext) => _dbContext = dbContext;

		public bool Add(Client client)
		{
			if (Exists(client))
				return false;

			_dbContext.Clients.Add(client);

			return true;
		}


		public bool Exists(string hostName) => _dbContext.Clients.Where(e => e.Hostname == hostName).Any();

		public bool Exists(Client client) => Exists(client.Hostname);

		public Client Get(int id) => _dbContext.Clients.FirstOrDefault(e => e.Id == id);

		public Client Get(string hostName) => _dbContext.Clients.FirstOrDefault(e => e.Hostname == hostName);

		public IEnumerable<Client> GetAll() => _dbContext.Clients.ToList();

		public void Remove(int id)
		{
			var clientToDelete = new Client() { Id = id };
			var local = _dbContext.Set<Client>().Local.FirstOrDefault(e => e.Id.Equals(id));

			RemovePrivate(clientToDelete, local);
		}

		public void Remove(string hostName)
		{
			if (string.IsNullOrEmpty(hostName))
				throw new ArgumentNullException(nameof(hostName));

			var clientToDelete = new Client() { Hostname = hostName };
			var local = _dbContext.Set<Client>().Local.FirstOrDefault(e => e.Hostname.Equals(hostName));

			RemovePrivate(clientToDelete, local);
		}

		private void RemovePrivate(Client clientToDelete, Client local)
		{
			if (local != null)
				_dbContext.Clients.Remove(local);
			else
			{
				_dbContext.Clients.Attach(clientToDelete);
				_dbContext.Clients.Remove(clientToDelete);
			}
		}

		public bool SaveChanges() => _dbContext.SaveChanges() >= 0;

		public bool Exists(int id) => _dbContext.Clients.Any(e => e.Id == id);
	}
}
