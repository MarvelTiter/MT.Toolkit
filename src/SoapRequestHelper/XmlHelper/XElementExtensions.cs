using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
namespace SoapRequestHelper.XmlHelper;

#if NET462
public static class IDictionaryExtensions
{
    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value)
    {
        if (dic.ContainsKey(key))
        {
            return false;
        }
        dic.Add(key, value);
        return true;
    }
}
#endif
internal static class XElementExtensions
{
    public static dynamic AsDynamic(this XElement root)
    {
        object? value = InternalConvert(root);
        var node = new ExpandoObject();
        node.TryAdd(root.Name.LocalName, value);
        return node;
    }

    private static dynamic InternalConvert(XElement root)
    {
        var node = new ExpandoObject();
        IDictionary<string, object?> nodeValues = new ExpandoObject();
        //XElement[] elements = [.. root.Elements()];
        foreach (var p in root.Elements())
        {
            var key = p.Name.LocalName;
            object? val;
            if (p.Elements().Any())
            {
                val = InternalConvert(p);
            }
            else
            {
                val = p.Value;
            }
            if (!nodeValues.TryGetValue(key, out var list))
            {
                list = new List<object>();
                nodeValues[key] = list;
            }
            ((List<object>)list!).Add(val);
        }

        foreach (var item in nodeValues)
        {
            var val = (List<object>)item.Value!;
            var v = val.Count > 1 ? val : val[0];
            node.TryAdd(item.Key, v);
        }
        return node;
    }

    public static string? GetValue(this XElement? element, string path, XmlNamespaceManager? nsManager = null)
    {
        return element?.XPathSelectElement(path, nsManager)?.Value;
    }
    public static T? GetValue<T>(this XElement? element, string path, XmlNamespaceManager? nsManager = null) where T : struct
    {
        var str = element.GetValue(path, nsManager);
        if (string.IsNullOrEmpty(str)) return default;
        var ret = Convert.ChangeType(str, typeof(T));
        if (ret == null) return default;
        return (T?)ret;
    }

    public static XElement? GetElement(this XElement? element, string path, XmlNamespaceManager? nsManager = null)
    {
        return element?.XPathSelectElement(path, nsManager);
    }

    public static IEnumerable<XElement> GetElementsAfterSelf(this XElement? element, string path, XmlNamespaceManager? nsManager = null)
    {
        return element?.XPathSelectElement(path, nsManager)?.ElementsAfterSelf() ?? [];
    }

    public static IEnumerable<XElement> GetChildElements(this XElement? element, string path, XmlNamespaceManager? nsManager = null)
    {
        return element?.XPathSelectElement(path, nsManager)?.Elements() ?? [];
    }
}