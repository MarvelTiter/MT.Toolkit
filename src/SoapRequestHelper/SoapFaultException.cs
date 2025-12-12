using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoapRequestHelper;

/// <summary>
/// Soap请求异常
/// </summary>
/// <param name="fCode"></param>
/// <param name="fString"></param>
/// <param name="fDetail"></param>
public class SoapFaultException(string? fCode, string? fString, string? fDetail) : Exception($"{fCode}-{fString}-{fDetail}")
{
    /// <summary>
    /// 失败代码
    /// </summary>
    public string? FaultCode { get; } = fCode;
    /// <summary>
    /// 失败信息
    /// </summary>
    public string? FaultString { get; } = fString;
    /// <summary>
    /// 失败详细信息
    /// </summary>
    public string? FaultDetail { get; } = fDetail;
}
