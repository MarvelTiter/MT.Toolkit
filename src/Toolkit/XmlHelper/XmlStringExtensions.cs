using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace MT.Toolkit.XmlHelper;

/// <summary>
/// <see cref="XmlString"/>扩展方法
/// </summary>
public static class XmlStringExtensions
{
    /// <summary>
    /// 根据XML path获取值
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="path"></param>
    /// <param name="ns"></param>
    /// <returns></returns>
    public static string? GetValue(this XmlString? xml, string path, XmlNamespaceManager? ns = null)
    {
        return xml?.Xml.Value?.GetValue(path, ns);
    }

    /// <summary>
    /// 根据XML path获取值，并转换为指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xml"></param>
    /// <param name="path"></param>
    /// <param name="ns"></param>
    /// <returns></returns>
    public static T? GetValue<T>(this XmlString? xml, string path, XmlNamespaceManager? ns = null) where T : struct
    {
        return xml?.Xml.Value?.GetValue<T>(path, ns);
    }

    /// <summary>
    /// 转成Dynamic对象
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static dynamic? AsDynamic(this XmlString? xml)
    {
        return xml?.Xml.Value?.AsDynamic();
    }

    /// <summary>
    /// <see cref="XNode.ElementsAfterSelf()"/>
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="path"></param>
    /// <param name="ns"></param>
    /// <returns></returns>
    public static IEnumerable<XElement> GetElementsAfterSelf(this XmlString? xml, string path, XmlNamespaceManager? ns = null)
    {
        return xml?.Xml.Value?.GetElementsAfterSelf(path, ns) ?? [];
    }

    /// <summary>
    /// 根据XML path获取<see cref="XElement"/>
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="path"></param>
    /// <param name="nsManager"></param>
    /// <returns></returns>
    public static XElement? GetElement(this XmlString? xml, string path, XmlNamespaceManager? nsManager = null)
    {
        return xml?.Xml.Value?.GetElement(path, nsManager);
    }
}
