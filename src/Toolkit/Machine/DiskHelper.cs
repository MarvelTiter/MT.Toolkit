using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.Machine;

/// <summary>
/// 硬盘相关工具类
/// </summary>
public static class DiskHelper
{
    /// <summary>
    /// 获取所有逻辑驱动器
    /// </summary>
    /// <returns></returns>
    public static string[] GetDrives()
    {
        return Environment.GetLogicalDrives();
    }
}
