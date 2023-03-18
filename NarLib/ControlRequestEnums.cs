using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarLib
{
	public enum ControlRequestType
	{
		Cmd = 0,
		InternalFunction = 1,
		Unknown
	}

	public enum ControlRequestStatus
	{
		Awaiting = 0,
		TimedOut,
		Failed,
		Succeeded
	}
}
