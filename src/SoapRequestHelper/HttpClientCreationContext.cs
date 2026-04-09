namespace SoapRequestHelper;

/// <summary>
/// 
/// </summary>
/// <param name="ConfigurationName"></param>
/// <param name="MethodName"></param>
/// <param name="Parameters"></param>
public readonly record struct HttpClientCreationContext(string ConfigurationName, string MethodName, Dictionary<string, object>? Parameters);



