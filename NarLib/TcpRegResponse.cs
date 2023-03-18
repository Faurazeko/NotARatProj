using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarLib
{
	public class TcpRegResponse
	{
		public TcpRegStatus Status { get; set; }
	}

	public enum TcpRegStatus
	{
		Ok = 0,
		WrongCredentails,
		ErrorRegEnd,
		UncategorizedError
	}
}
