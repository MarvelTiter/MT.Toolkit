using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
namespace MT.Toolkit.XmlHelper
{
    public static class XElementExtensions
    {
        public static dynamic AsDynamic(this XElement root)
        {
            var ret = new ExpandoObject();
            var values = new Dictionary<string, List<object>>();
            foreach (var p in root.Elements())
            {
                var key = p.Name.LocalName;
                object? val;
                if (p.Elements().Any())
                {
                    val = p.AsDynamic();
                }
                else
                {
                    val = p.Value;
                }
                if (!values.TryGetValue(key, out var list))
                {
                    list = new List<object>();
                    values[key] = list;
                }
                list.Add(val);
            }

            foreach (var item in values)
            {
                var v = item.Value.Count > 1 ? item.Value : item.Value[0];
                ret.TryAdd(item.Key, v);
            }
            return ret;
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
    }
}