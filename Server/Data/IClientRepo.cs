using Server.Models;
using System.Net.NetworkInformation;

namespace Server.Data
{
	public interface IClientRepo
	{
		bool SaveChanges();

		IEnumerable<Client> GetAll();
		bool Add(Client client);

		void Remove(int id);
		void Remove(string hostName);

		Client Get(int id);
		Client Get(string hostName);

		bool Exists(Client client);
		bool Exists(string hostName);
		bool Exists(int id);
	}
}
