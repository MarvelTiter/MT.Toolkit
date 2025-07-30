using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SoapRequestHelper.XmlHelper;

/// <summary>
/// 尝试将string转为XElement，如果根节点不是root，将使用root根节点包裹
/// </summary>
internal class XmlString
{
    internal string? Value { get; private set; }

    public XmlString(string? str)
    {
        Value = str;
        Xml = new Lazy<XElement?>(() =>
        {
            if (Value == null)
                return null;
            if (!CheckRoot(Value))
            {
                Value = $"<root>{Value}</root>";
            }

            //using var reader = new StringReader(Value);
            //using var xmlReader = XmlReader.Create(reader, new XmlReaderSettings()
            //{
            //    ConformanceLevel = ConformanceLevel.Fragment
            //});
            //XElement.Load(xmlReader);
            return XElement.Parse(Value);
        });
    }

    internal Lazy<XElement?> Xml { get; }

    public static implicit operator XmlString(string? str) => new(str);
    private static bool CheckRoot(string xml)
    {
        try
        {
            using var reader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(reader);

            xmlReader.MoveToContent();
            var root = xmlReader.Name;
            if (root != "root")
            {
                return false;
            }
            if (xmlReader.ReadToFollowing(root))
            {
                return false;
            }
            return true;
        }
        catch (XmlException ex) when (ex.Message.Contains("multiple root elements"))
        {
            return false;
        }

    }

    //private static bool HasRootElement(string xml)
    //{
    //    try
    //    {
    //        using var reader = new StringReader(xml);
    //        using var xmlReader = XmlReader.Create(reader);
    //        xmlReader.MoveToContent();
    //        return xmlReader.Name == "root";
    //    }
    //    catch (Exception)
    //    {
    //        throw;
    //    }

    //}
}
