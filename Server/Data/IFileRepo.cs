using Server.Models;
using System.Net.NetworkInformation;

namespace Server.Data
{
	public interface IFileRepo
	{
		bool SaveChanges();

		IEnumerable<UploadedFile> GetAll();
		bool Add(UploadedFile file);

		void Remove(int id);
		void Remove(string filename);

		UploadedFile Get(int id);
		IEnumerable<UploadedFile> GetForClient(int clientId);

		bool Exists(UploadedFile file);
		bool Exists(string filename);
		bool Exists(int id);
	}
}
