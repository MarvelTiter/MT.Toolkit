using MT.Toolkit.Machine.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MT.Toolkit.Machine.HelperImpl
{
	internal class CpuImplForWindow : ICpu
	{
		public async Task<double> CpuProcessUsageAsync()
		{
			var startTime = DateTime.UtcNow;
			var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

			await Task.Delay(500);

			var endTime = DateTime.UtcNow;
			var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

			var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
			var totalMsPassed = (endTime - startTime).TotalMilliseconds;

			var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

			return cpuUsageTotal * 100;
		}

		public double CpuTotalUsage()
		{
			throw new NotImplementedException();
		}

		public int ProcessorCount()
		{
			return Environment.ProcessorCount;
		}
	}
}
