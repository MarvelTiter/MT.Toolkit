using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MT.Toolkit.XmlHelper
{
    public class XmlString
    {
        internal string? Value { get; }

        public XmlString(string? str)
        {
            Value = str;
            Xml = new Lazy<XElement?>(() =>
            {
                if (Value == null)
                    return null;
                using var reader = new StringReader(Value);
                using var xmlReader = XmlReader.Create(reader);
                return XElement.Load(xmlReader);
            });
        }

        internal Lazy<XElement?> Xml { get; }

        public static implicit operator XmlString(string? str) => new(str);

    }
}
