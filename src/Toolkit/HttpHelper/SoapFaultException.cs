using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.HttpHelper;

/// <summary>
/// Soap接口调用异常类
/// </summary>
/// <param name="fCode"></param>
/// <param name="fString"></param>
/// <param name="fDetail"></param>
public class SoapFaultException(string? fCode, string? fString, string? fDetail) : Exception($"{fCode}-{fString}-{fDetail}")
{
    /// <summary>
    /// Gets the fault code associated with the current error, if available.
    /// </summary>
    public string? FaultCode { get; } = fCode;
    /// <summary>
    /// Gets the fault string associated with the current error, if available.
    /// </summary>
    public string? FaultString { get; } = fString;
    /// <summary>
    /// Gets the fault detail associated with the current error, if available.
    /// </summary>
    public string? FaultDetail { get; } = fDetail;
}
