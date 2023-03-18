using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWF.Models
{
	public class DriveInfoDto
	{
		public long AvailableFreeSpace { get; set; }
		public string DriveFormat { get; set; }
		public string Name { get; set; }
		public string Directory { get; set; }
		public long TotalFreeSpace { get; set; }
		public long TotalSize { get; set; }
		public string VolumeLabel { get; set; }
	}
}
