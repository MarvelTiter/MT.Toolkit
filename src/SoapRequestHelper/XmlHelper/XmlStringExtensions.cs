using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SoapRequestHelper.XmlHelper;

internal static class XmlStringExtensions
{
    public static string? GetValue(this XmlString? xml, string path, XmlNamespaceManager? ns = null)
    {
        return xml?.Xml.Value?.GetValue(path, ns);
    }

    public static T? GetValue<T>(this XmlString? xml, string path, XmlNamespaceManager? ns = null) where T : struct
    {
        return xml?.Xml.Value?.GetValue<T>(path, ns);
    }

    public static dynamic? AsDynamic(this XmlString? xml)
    {
        return xml?.Xml.Value?.AsDynamic();
    }

    public static IEnumerable<XElement> GetElementsAfterSelf(this XmlString? xml, string path, XmlNamespaceManager? ns = null)
    {
        return xml?.Xml.Value?.GetElementsAfterSelf(path, ns) ?? [];
    }

    public static XElement? GetElement(this XmlString? xml, string path, XmlNamespaceManager? nsManager = null)
    {
        return xml?.Xml.Value?.GetElement(path, nsManager);
    }
}
