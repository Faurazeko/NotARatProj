using System.ComponentModel.DataAnnotations;

namespace Server.Models
{
	public class UploadedFile
	{
		public int Id { get; set; }
		public int ClientId { get; set; }
		public string Filename { get; set; }
		public string OriginalFilename { get; set; }
		[DataType("datetime2")]
		public DateTime UploadedDateTimeUtc { get; set; } = DateTime.UtcNow;

	}
}
