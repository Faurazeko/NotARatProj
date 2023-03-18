using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWF.Models
{
	public class DirDataResponse<T>
	{
		public bool IsOk { get; set; } = true;
		public string ErrMsg { get; set; } = string.Empty;
		public DirDataType DirDataType { get; set; } = DirDataType.DirContent;
		public T Response { get; set; }

	}

	public enum DirDataType
	{
		DirContent = 0,
		Drives = 1
	}
}
