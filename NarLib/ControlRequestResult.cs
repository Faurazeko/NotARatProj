using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarLib
{
	public class ControlRequestResult
	{
		public int RequestId { get; set; }
		public ControlRequestStatus Status { get; set; }
		public string? Result { get; set; }
		public string[] Files { get; set; }
		public int ClientId { get; set; }
		public string ClientSecret { get; set; }
	}
}
