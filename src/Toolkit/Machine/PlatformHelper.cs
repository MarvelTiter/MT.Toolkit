using MT.Toolkit.Machine.HelperImpl;
using MT.Toolkit.Machine.Interfaces;
using System;
using System.Runtime.InteropServices;

namespace MT.Toolkit.Machine
{
	internal class PlatformHelper
	{
		internal static ICpu GetCpuHelper()
		{
#if NET40_OR_GREATER
            return new CpuImplForWindow();
#else
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return new CpuImplForWindow();
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				throw new PlatformNotSupportedException();
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				throw new PlatformNotSupportedException();
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
			{
				throw new PlatformNotSupportedException();
			}
			else
				throw new Exception("Unknow OsPlatform");
#endif
		}


	}
}
