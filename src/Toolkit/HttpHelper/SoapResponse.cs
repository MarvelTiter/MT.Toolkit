using MT.Toolkit.XmlHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
namespace MT.Toolkit.HttpHelper
{
    public sealed class SoapResponse
    {
        /// <summary>
        /// 请求是否发送成功
        /// </summary>
        public bool Success { get; set; } = true;
        /// <summary>
        /// 请求发生异常或解析返回数据时发生异常的异常信息
        /// </summary>
        public string? Message { get; set; }
        private readonly XmlString? xmlString;
        private readonly string? responseContent;
        private XmlNamespaceManager? nsManager;
        private readonly string? methodName;

        internal SoapResponse(string? responseContent, string? rawValue, XmlNamespaceManager manager, string? methodName)
        {
            this.responseContent = responseContent;
            xmlString = rawValue;
            nsManager = manager;
            this.methodName = methodName;
        }

        internal SoapResponse(Exception ex)
        {
            Success = false;
            Message = ex.Message;
        }
        public string? RawContent => responseContent;
        public string? RawValue => xmlString?.Value;

        public T? ReadReturnValue<T>()
        {
            var type = typeof(T);
            if (type != typeof(object))
            {
                var str = xmlString?.GetValue($"//r:{methodName}Result", nsManager);
                if (string.IsNullOrEmpty(str)) return default;
                var ret = Convert.ChangeType(str, typeof(T));
                if (ret == null) return default;
                return (T?)ret;
            }
            // 序列化返回的结果
            return GetNode($"//r:{methodName}Result")?.AsDynamic();
        }

        public dynamic? ReadReturnValue() => ReadReturnValue<object>();

        public T? ReadParameterReturnValue<T>(string? name = null)
        {
            var outParam = ReadParameterReturnValueAsXml(name);
            if (outParam == null) return default;
            var type = typeof(T);
            if (type != typeof(object))
            {
                var str = outParam.Value;
                if (string.IsNullOrEmpty(str)) return default;
                var ret = Convert.ChangeType(str, typeof(T));
                if (ret == null) return default;
                return (T?)ret;
            }
            return outParam.AsDynamic();
        }

        public XElement? ReadParameterReturnValueAsXml(string? name = null)
        {
            var outParams = xmlString?.GetElementsAfterSelf($"r:{methodName}Result", nsManager);
            XElement? outParam = outParams?.FirstOrDefault(x =>
            {
                if (string.IsNullOrEmpty(name)) return true;
                return x.Name == name;
            });
            if (outParam == null) return default;
            return outParam;
        }

        public dynamic? ReadParameterReturnValue(string? name = null) => ReadParameterReturnValue<object>(name);

        private XElement? retXml;

        /// <summary>
        /// 如果返回的结果可以解释为XML
        /// </summary>
        /// <returns></returns>
        public XElement? ReadReturnValueAsXml()
        {
            if (retXml == null)
            {
                var raw = GetValue($"//r:{methodName}Result");
                if (raw != null)
                {
                    using var reader = new StringReader(raw);
                    using var xmlReader = XmlReader.Create(reader);
                    retXml = XElement.Load(xmlReader);
                }
            }
            return retXml;
        }

        /// <summary>
        /// 从接口响应的Xml中获取数据，需要加上前缀 <b>r</b>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string? GetValue(string expression) => xmlString?.GetValue(expression, nsManager);

        /// <summary>
        /// 从接口响应的Xml中获取数据，需要加上前缀 <b>r</b>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private XElement? GetNode(string expression) => xmlString?.GetElement(expression, nsManager);

        /// <summary>
        /// 从接口响应的Xml中获取数据，需要加上前缀 <b>r</b>
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public T? GetValue<T>(string expression) where T : struct
        {
            //var str = xml?.GetValue(expression, nsManager);
            //if (string.IsNullOrEmpty(str)) return default;
            //var ret = Convert.ChangeType(str, typeof(T));
            //if (ret == null) return default;
            //return (T?)ret;
            return xmlString?.GetValue<T>(expression, nsManager);
        }
    }
}