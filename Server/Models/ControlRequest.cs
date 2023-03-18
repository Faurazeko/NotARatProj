using Server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NarLib
{
	public class ControlRequest
	{
		public int Id { get; set; }
		public int ClientId { get; set; }
		public Client Client { get; set; }
		public ControlRequestType Type { get; set; }
		public string Request { get; set; }
		public string FilesSerialized { get; set; } = "";
		[NotMapped]
		public IEnumerable<string> Files 
		{ 
			get => FilesSerialized.Split(';').Where(e => !String.IsNullOrWhiteSpace(e)).ToList();

			set
			{
				var files = "";

				foreach (var item in value)
					files += $"{item};";

				FilesSerialized = files;
			}
		}
		public ControlRequestStatus Status { get; set; } = ControlRequestStatus.Awaiting;
		public string? Result { get; set;}
		[DataType("datetime2")]
		public DateTime CreatedUtcDateTime { get; set; } = DateTime.UtcNow;
	}
}
