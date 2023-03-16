using MT.Toolkit.Machine.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.Machine
{
	public static class CpuHelper
	{
		private static ICpu cpuHelper = PlatformHelper.GetCpuHelper();
		/// <summary>
		/// CPU核心数
		/// </summary>
		/// <returns></returns>
		public static int ProcessorCount() => cpuHelper.ProcessorCount();

		/// <summary>
		/// 服务器CPU使用情况
		/// </summary>
		/// <returns></returns>
		public static double CpuTotalUsage() => cpuHelper.CpuTotalUsage();

		/// <summary>
		/// 当前进程CPU使用情况
		/// </summary>
		/// <returns></returns>
		public static Task<double> CpuProcessUsageAsync() => cpuHelper.CpuProcessUsageAsync();
	}
}
