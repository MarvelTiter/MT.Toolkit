using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET40_OR_GREATER
#else
using System.Runtime.InteropServices;
#endif
namespace MT.Toolkit.Machine
{
	public static class SysHelper
	{
		public static string OsPlatform()
		{
#if NET40_OR_GREATER
            return "Windows";
#else
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return "Windows";
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				return "Linux";
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				return "OS X";
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
			{
				return "FreeBSD";
			}
			else
				throw new Exception("Unknow OsPlatform");
#endif
		}
		public static string OSArchitecture()
		{
#if NET40_OR_GREATER
            return Environment.Is64BitOperatingSystem ? "X64" : "X86";

#else
			return RuntimeInformation.OSArchitecture.ToString();
#endif
		}
#if NET5_0_OR_GREATER
		public static string OSDescription()
		{
			return RuntimeInformation.OSDescription;
		}
		public static string ProcessArchitecture()
		{
			return RuntimeInformation.ProcessArchitecture.ToString();
		}
#endif
		public static string SystemBit()
		{
			return Environment.Is64BitOperatingSystem ? "x64" : "x86";
		}

		public static string OSVersion()
		{
			return $"{Environment.OSVersion.Platform}:{Environment.Version}";
		}

		public static string HostName()
		{
			return Environment.MachineName;
		}

		public static TimeSpan RunningTime()
		{
#if NET40_OR_GREATER
            var tick = Environment.TickCount;
#else
			var tick = Environment.TickCount64;
#endif
			return new TimeSpan(tick);
		}
	}
}
