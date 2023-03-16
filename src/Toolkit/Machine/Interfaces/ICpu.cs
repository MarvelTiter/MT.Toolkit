using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.Machine.Interfaces
{
	internal interface ICpu
	{
		int ProcessorCount();
		double CpuTotalUsage();
		Task<double> CpuProcessUsageAsync();
	}

	internal interface ISystemInfo
	{
		string OsPlatform();
		string OSArchitecture();
		string OSDescription();
		string ProcessArchitecture();
		string SystemBit();
		string OSVersion();
		string HostName();
		TimeSpan RunningTime();
	}
}
