using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET40_OR_GREATER
#else
using System.Runtime.InteropServices;
#endif
namespace MT.Toolkit.Machine;

/// <summary>
/// 系统信息帮助类
/// </summary>
public static class SysHelper
{
    /// <summary>
    /// 获取操作系统平台
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
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

    /// <summary>
    /// 获取操作系统架构
    /// </summary>
    /// <returns></returns>
    public static string OSArchitecture()
    {
#if NET40_OR_GREATER
        return Environment.Is64BitOperatingSystem ? "X64" : "X86";

#else
        return RuntimeInformation.OSArchitecture.ToString();
#endif
    }
#if NET5_0_OR_GREATER
    /// <summary>
    /// 获取操作系统描述
    /// </summary>
    /// <returns></returns>
    public static string OSDescription()
    {
        return RuntimeInformation.OSDescription;
    }
    /// <summary>
    /// 获取当前进程架构
    /// </summary>
    /// <returns></returns>
    public static string ProcessArchitecture()
    {
        return RuntimeInformation.ProcessArchitecture.ToString();
    }
#endif
    /// <summary>
    /// 获取系统位数
    /// </summary>
    /// <returns></returns>
    public static string SystemBit()
    {
        return Environment.Is64BitOperatingSystem ? "x64" : "x86";
    }

    /// <summary>
    /// 获取系统版本
    /// </summary>
    /// <returns></returns>
    public static string OSVersion()
    {
        return $"{Environment.OSVersion.Platform}:{Environment.Version}";
    }

    /// <summary>
    /// 获取机器名称
    /// </summary>
    /// <returns></returns>
    public static string HostName()
    {
        return Environment.MachineName;
    }

    /// <summary>
    /// 获取运行时间
    /// </summary>
    /// <returns></returns>
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
