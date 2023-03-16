using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.Machine
{
	public static class DiskHelper
	{
		public static void GetDisk()
		{
			Environment.GetLogicalDrives();
		}
	}
}
