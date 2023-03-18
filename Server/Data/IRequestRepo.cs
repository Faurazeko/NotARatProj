using NarLib;
using Server.Models;

namespace Server.Data
{
	public interface IRequestRepo
	{
		bool SaveChanges();

		IEnumerable<ControlRequest> GetAll();
		bool Add(ControlRequest request);

		void Remove(int id);

		ControlRequest Get(int id);
		ICollection<ControlRequest> GetForClient(int clientId);

		bool Exists(ControlRequest request);
		bool Exists(int id);
	}
}
