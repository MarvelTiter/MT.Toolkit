using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.Machine
{
	public static class MemoryHelper
	{
		/// <summary>
		/// 机器总内存
		/// </summary>
		/// <returns></returns>
		public static double TotalMemory()
		{
			return 0;
		}

		public static long ProcessMemoryUsage()
		{
			//return Process.GetCurrentProcess().WorkingSet64;
			return Environment.WorkingSet;
		}

		public static long TotalUsage()
		{
			var process = Process.GetProcesses();
			long totalB = 0;
			foreach (var p in process)
			{
				totalB += p.WorkingSet64;
			}
			return totalB;
		}
	}
}
