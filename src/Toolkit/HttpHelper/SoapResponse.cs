using MT.Toolkit.TypeConvertHelper;
using MT.Toolkit.XmlHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
        public const string RN_ALIAS = "r";
        /// <summary>
        /// 请求是否发送成功
        /// </summary>
        public bool Success { get; set; } = true;
        /// <summary>
        /// 请求发生异常时的异常
        /// </summary>
        public Exception? Exception { get; set; }
        /// <summary>
        /// 请求发生异常或解析返回数据时发生异常的异常信息
        /// </summary>
        public string? Message { get; set; }
        private readonly XmlString? xmlString;
        private readonly string? responseContent;
        private XmlNamespaceManager? nsManager;
        private readonly string? methodName;

        internal SoapResponse(string requestContent, string? responseContent, string? rawValue, XmlNamespaceManager manager, string? methodName)
        {
            RequestContent = requestContent;
            this.responseContent = responseContent;
            xmlString = rawValue;
            nsManager = manager;
            this.methodName = methodName;
        }

        internal SoapResponse(string requestContent,string? responseContent, Exception ex)
        {
            Success = false;
            RequestContent = requestContent;
            this.responseContent = responseContent;
            Exception = ex;
            Message = ex.Message;
        }
        public string? RawContent => responseContent;
        public string? RawValue => xmlString?.Value;
        public string RequestContent { get; }
        private XElement? retXml;

        #region 解析返回值
        /// <summary>
        /// 接口签名中return的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? ReadReturnValue<T>()
        {
            var type = typeof(T);
            if (type != typeof(object))
            {
                var str = xmlString?.GetValue($"//{RN_ALIAS}:{methodName}Result", nsManager);
                if (string.IsNullOrEmpty(str)) return default;
                var ret = Convert.ChangeType(str, typeof(T));
                if (ret == null) return default;
                return (T?)ret;
            }
            // 序列化返回的结果
            return GetNode($"//{RN_ALIAS}:{methodName}Result")?.AsDynamic();
        }
        /// <summary>
        /// 接口签名中return的内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public dynamic? ReadReturnValue() => ReadReturnValue<object>();

        /// <summary>
        /// 如果返回的结果可以解释为XML
        /// </summary>
        /// <returns></returns>
        public XElement? ReadReturnValueAsXml()
        {
            if (retXml == null)
            {
                var raw = GetValue($"//{RN_ALIAS}:{methodName}Result");
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
        /// 如果返回的结果可以转为DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ReadReturnValueAsDataTable()
        {
            nsManager?.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            var xml = xmlString?.GetElement($"//{RN_ALIAS}:{methodName}Result", nsManager);
            var schema = xml.GetChildElements("//xs:sequence", nsManager);
            var datas = xml.GetChildElements("//DocumentElement", nsManager);
            return CreateDataTable(schema, datas, nsManager);
        }

        #endregion

        #region 额外的返回值，out / ref 等
        /// <summary>
        /// out参数或者ref参数的返回值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
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
        /// <summary>
        /// out参数或者ref参数的返回值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic? ReadParameterReturnValue(string? name = null) => ReadParameterReturnValue<object>(name);
        /// <summary>
        /// 如果out参数或者ref参数的返回值可以解析为XML
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XElement? ReadParameterReturnValueAsXml(string? name = null)
        {
            var outParams = xmlString?.GetElementsAfterSelf($"{RN_ALIAS}:{methodName}Result", nsManager);
            XElement? outParam = outParams?.FirstOrDefault(x =>
            {
                if (string.IsNullOrEmpty(name)) return true;
                return x.Name == name;
            });
            if (outParam == null) return default;
            return outParam;
        }
        /// <summary>
        /// 如果out参数或者ref参数的返回值可以解析为DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ReadParameterReturnValueAsDataTable()
        {
            nsManager?.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            var xml = ReadParameterReturnValueAsXml();
            var schema = xml.GetChildElements("//xs:sequence", nsManager);
            var datas = xml.GetChildElements("//DocumentElement", nsManager);
            return CreateDataTable(schema, datas, nsManager);
        }

        #endregion

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

        private static DataTable CreateDataTable(IEnumerable<XElement> schema, IEnumerable<XElement> datas, XmlNamespaceManager? nsManager)
        {
            var dt = new DataTable();
            foreach (var element in schema)
            {
                var name = element.Attribute("name")!.Value;
                var type = element.Attribute("type")!.Value;
                dt.Columns.Add(name, ConvertElementType(type));
            }

            foreach (var d in datas)
            {
                var row = dt.NewRow();
                foreach (DataColumn col in dt.Columns)
                {
                    var value = d.GetValue(col.ColumnName);
                    var v = value?.Parse(col.DataType) ?? DBNull.Value;
                    row[col.ColumnName] = v;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        private static Type ConvertElementType(string type)
        {
            return type switch
            {
                "xs:dateTime" => typeof(DateTime),
                "xs:boolean" => typeof(bool),
                "xs:integer" or "xs:int" => typeof(int),
                "xs:decimal" => typeof(decimal),
                "xs:long" => typeof(long),
                "xs:short" => typeof(short),
                _ => typeof(string),
            };
        }
    }
}