using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.HttpHelper;

public class SoapFaultException(string? fCode, string? fString, string? fDetail) : Exception($"{fCode}-{fString}-{fDetail}")
{
    public string? FaultCode { get; } = fCode;
    public string? FaultString { get; } = fString;
    public string? FaultDetail { get; } = fDetail;
}
