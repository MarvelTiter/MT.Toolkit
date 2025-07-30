using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.XPath;
using SoapRequestHelper.XmlHelper;
using SoapRequestHelper;

namespace SoapReponseTest;

[TestClass]
public class Test1
{

    [TestMethod]
    public async Task ServiceCollectionTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddHttpClient();
        services.AddSoapServiceHelper(manager =>
        {
            manager.AddSoapService("Main", config =>
            {
                config.Url = "";
                config.RequestNamespace = "";
                config.ResponseNamespace = "";
            }).AddSoapService("Sec", config =>
            {
                config.Url = "";
                config.RequestNamespace = "";
                config.ResponseNamespace = "";
            })
            .SetDefault("Sec");
        });

        var provider = services.BuildServiceProvider();
        var fac = provider.GetService<ISoapServiceFactory>();
        var main = fac!.GetSoapService("Main");

        var ris = fac!.GetSoapService("Sec");

        var login = await ris.SendAsync("Login", new()
        {
            ["szUser"] = "admin",
            ["szPass"] = "hgbanner"
        });

        var response = await main.SendAsync("UpdateLst", new()
        {
            ["lTick"] = 0,
            ["typeid"] = 1,
            ["MyGuid"] = login.ReadReturnValueAsXml()?.GetValue("//Guid")!
        });
        var d = response.ReadReturnValueAsXml()?.AsDynamic();



    }

    [TestMethod]
    public void ToDataTable()
    {
        string responseXml = """
            <?xml version="1.0" encoding="utf-8"?>
            <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"
                xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                xmlns:xsd="http://www.w3.org/2001/XMLSchema">
                <soap:Body>
                    <GetMutimeDiaByPlateResponse xmlns="http://risservice.hg-banner.com.cn/">
                        <GetMutimeDiaByPlateResult>
                            <xs:schema id="NewDataSet" xmlns="" xmlns:xs="http://www.w3.org/2001/XMLSchema"
                                xmlns:msdata="urn:schemas-microsoft-com:xml-msdata"
                                xmlns:msprop="urn:schemas-microsoft-com:xml-msprop">
                                <xs:element name="NewDataSet" msdata:IsDataSet="true"
                                    msdata:MainDataTable="MutimeDia" msdata:UseCurrentLocale="true">
                                    <xs:complexType>
                                        <xs:choice minOccurs="0" maxOccurs="unbounded">
                                            <xs:element name="MutimeDia" msprop:BaseTable.0="JOBS"
                                                msprop:BaseTable.1="BASIC_STATION">
                                                <xs:complexType>
                                                    <xs:sequence>
                                                        <xs:element name="VIDEOEXIST" msprop:OraDbType="104"
                                                            type="xs:string" minOccurs="0" />
                                                        <xs:element name="VIDEOSIZE" msprop:OraDbType="104"
                                                            type="xs:string" minOccurs="0" />
                                                        <xs:element name="JOB_DATE" msprop:OraDbType="106"
                                                            msprop:BaseColumn="JOB_DATE" type="xs:dateTime"
                                                            minOccurs="0" />
                                                        <xs:element name="JYLSH" msprop:OraDbType="126"
                                                            type="xs:string" minOccurs="0" />
                                                        <xs:element name="JYCS" msprop:OraDbType="111"
                                                            type="xs:short" minOccurs="0" />
                                                        <xs:element name="JYXM" msprop:OraDbType="126"
                                                            msprop:BaseColumn="LXIP" type="xs:string"
                                                            minOccurs="0" />
                                                        <xs:element name="SPLX" msprop:OraDbType="111"
                                                            msprop:BaseColumn="LXPORT" type="xs:short"
                                                            minOccurs="0" />
                                                        <xs:element name="SPTD" msprop:OraDbType="111"
                                                            msprop:BaseColumn="STATIONIP" type="xs:short"
                                                            minOccurs="0" />
                                                        <xs:element name="KSSJ" msprop:OraDbType="106"
                                                            msprop:BaseColumn="STATIONPORT" type="xs:dateTime"
                                                            minOccurs="0" />
                                                        <xs:element name="JSSJ" msprop:OraDbType="106"
                                                            type="xs:dateTime" minOccurs="0" />
                                                        <xs:element name="ZDXZ" msprop:OraDbType="111"
                                                            type="xs:short" minOccurs="0" />
                                                        <xs:element name="JCZBH" msprop:OraDbType="104"
                                                            type="xs:string" minOccurs="0" />
                                                        <xs:element name="XZCS" msprop:OraDbType="111"
                                                            type="xs:short" minOccurs="0" />
                                                        <xs:element name="SPSC" msprop:OraDbType="111"
                                                            type="xs:short" minOccurs="0" />
                                                        <xs:element name="JYXMNAME" msprop:OraDbType="126"
                                                            type="xs:string" minOccurs="0" />
                                                        <xs:element name="LXIP" msprop:OraDbType="104"
                                                            type="xs:string" minOccurs="0" />
                                                        <xs:element name="LXPORT" msprop:OraDbType="104"
                                                            type="xs:string" minOccurs="0" />
                                                        <xs:element name="STATIONIP" msprop:OraDbType="126"
                                                            type="xs:string" minOccurs="0" />
                                                        <xs:element name="STATIONPORT" msprop:OraDbType="126"
                                                            type="xs:string" minOccurs="0" />
                                                    </xs:sequence>
                                                </xs:complexType>
                                            </xs:element>
                                        </xs:choice>
                                    </xs:complexType>
                                </xs:element>
                            </xs:schema>
                            <diffgr:diffgram xmlns:msdata="urn:schemas-microsoft-com:xml-msdata"
                                xmlns:diffgr="urn:schemas-microsoft-com:xml-diffgram-v1">
                                <DocumentElement xmlns="">
                                    <MutimeDia diffgr:id="MutimeDia1" msdata:rowOrder="0"
                                        diffgr:hasChanges="modified">
                                        <VIDEOEXIST>1</VIDEOEXIST>
                                        <VIDEOSIZE>14261782</VIDEOSIZE>
                                        <JOB_DATE>2024-06-06T10:39:24+08:00</JOB_DATE>
                                        <JYLSH>240606370004N0264985</JYLSH>
                                        <JYCS>1</JYCS>
                                        <JYXM>C1</JYXM>
                                        <SPLX>1</SPLX>
                                        <SPTD>13</SPTD>
                                        <KSSJ>2024-06-06T10:57:38+08:00</KSSJ>
                                        <JSSJ>2024-06-06T10:59:19+08:00</JSSJ>
                                        <ZDXZ>0</ZDXZ>
                                        <JCZBH>3701000098</JCZBH>
                                        <JYXMNAME>底盘检验</JYXMNAME>
                                        <LXIP>172.37.90.2 </LXIP>
                                        <LXPORT>8092 </LXPORT>
                                        <STATIONIP>172.37.90.2</STATIONIP>
                                        <STATIONPORT>7091</STATIONPORT>
                                    </MutimeDia>
                                    <MutimeDia diffgr:id="MutimeDia2" msdata:rowOrder="1"
                                        diffgr:hasChanges="modified">
                                        <VIDEOEXIST>1</VIDEOEXIST>
                                        <VIDEOSIZE>5174101</VIDEOSIZE>
                                        <JOB_DATE>2024-06-06T10:39:24+08:00</JOB_DATE>
                                        <JYLSH>240606370004N0264985</JYLSH>
                                        <JYCS>1</JYCS>
                                        <JYXM>NQ</JYXM>
                                        <SPLX>1</SPLX>
                                        <SPTD>9</SPTD>
                                        <KSSJ>2024-06-06T10:56:52+08:00</KSSJ>
                                        <JSSJ>2024-06-06T10:57:04+08:00</JSSJ>
                                        <ZDXZ>0</ZDXZ>
                                        <JCZBH>3701000098</JCZBH>
                                        <JYXMNAME>联网查询检验</JYXMNAME>
                                        <LXIP>172.37.90.2 </LXIP>
                                        <LXPORT>8092 </LXPORT>
                                        <STATIONIP>172.37.90.2</STATIONIP>
                                        <STATIONPORT>7091</STATIONPORT>
                                    </MutimeDia>
                                    <MutimeDia diffgr:id="MutimeDia3" msdata:rowOrder="2"
                                        diffgr:hasChanges="modified">
                                        <VIDEOEXIST>1</VIDEOEXIST>
                                        <VIDEOSIZE>5039495</VIDEOSIZE>
                                        <JOB_DATE>2024-06-06T10:39:24+08:00</JOB_DATE>
                                        <JYLSH>240606370004N0264985</JYLSH>
                                        <JYCS>1</JYCS>
                                        <JYXM>NQ</JYXM>
                                        <SPLX>2</SPLX>
                                        <SPTD>10</SPTD>
                                        <KSSJ>2024-06-06T10:56:52+08:00</KSSJ>
                                        <JSSJ>2024-06-06T10:57:04+08:00</JSSJ>
                                        <ZDXZ>0</ZDXZ>
                                        <JCZBH>3701000098</JCZBH>
                                        <JYXMNAME>联网查询检验</JYXMNAME>
                                        <LXIP>172.37.90.2 </LXIP>
                                        <LXPORT>8092 </LXPORT>
                                        <STATIONIP>172.37.90.2</STATIONIP>
                                        <STATIONPORT>7091</STATIONPORT>
                                    </MutimeDia>
                                    <MutimeDia diffgr:id="MutimeDia4" msdata:rowOrder="3"
                                        diffgr:hasChanges="modified">
                                        <VIDEOEXIST>1</VIDEOEXIST>
                                        <VIDEOSIZE>6315202</VIDEOSIZE>
                                        <JOB_DATE>2024-06-06T10:39:24+08:00</JOB_DATE>
                                        <JYLSH>240606370004N0264985</JYLSH>
                                        <JYCS>1</JYCS>
                                        <JYXM>UC</JYXM>
                                        <SPLX>1</SPLX>
                                        <SPTD>9</SPTD>
                                        <KSSJ>2024-06-06T10:57:05+08:00</KSSJ>
                                        <JSSJ>2024-06-06T10:57:26+08:00</JSSJ>
                                        <ZDXZ>0</ZDXZ>
                                        <JCZBH>3701000098</JCZBH>
                                        <JYXMNAME>唯一性检验</JYXMNAME>
                                        <LXIP>172.37.90.2 </LXIP>
                                        <LXPORT>8092 </LXPORT>
                                        <STATIONIP>172.37.90.2</STATIONIP>
                                        <STATIONPORT>7091</STATIONPORT>
                                    </MutimeDia>
                                    <MutimeDia diffgr:id="MutimeDia5" msdata:rowOrder="4"
                                        diffgr:hasChanges="modified">
                                        <VIDEOEXIST>1</VIDEOEXIST>
                                        <VIDEOSIZE>6464899</VIDEOSIZE>
                                        <JOB_DATE>2024-06-06T10:39:24+08:00</JOB_DATE>
                                        <JYLSH>240606370004N0264985</JYLSH>
                                        <JYCS>1</JYCS>
                                        <JYXM>UC</JYXM>
                                        <SPLX>2</SPLX>
                                        <SPTD>10</SPTD>
                                        <KSSJ>2024-06-06T10:57:05+08:00</KSSJ>
                                        <JSSJ>2024-06-06T10:57:26+08:00</JSSJ>
                                        <ZDXZ>0</ZDXZ>
                                        <JCZBH>3701000098</JCZBH>
                                        <JYXMNAME>唯一性检验</JYXMNAME>
                                        <LXIP>172.37.90.2 </LXIP>
                                        <LXPORT>8092 </LXPORT>
                                        <STATIONIP>172.37.90.2</STATIONIP>
                                        <STATIONPORT>7091</STATIONPORT>
                                    </MutimeDia>
                                </DocumentElement>
                                <diffgr:before>
                                    <MutimeDia diffgr:id="MutimeDia1" msdata:rowOrder="0" xmlns="">
                                        <VIDEOEXIST>0</VIDEOEXIST>
                                        <VIDEOSIZE>0</VIDEOSIZE>
                                        <JOB_DATE>2024-06-06T10:39:24+08:00</JOB_DATE>
                                        <JYLSH>240606370004N0264985</JYLSH>
                                        <JYCS>1</JYCS>
                                        <JYXM>C1</JYXM>
                                        <SPLX>1</SPLX>
                                        <SPTD>13</SPTD>
                                        <KSSJ>2024-06-06T10:57:38+08:00</KSSJ>
                                        <JSSJ>2024-06-06T10:59:19+08:00</JSSJ>
                                        <ZDXZ>0</ZDXZ>
                                        <JCZBH>3701000098</JCZBH>
                                        <JYXMNAME>底盘检验</JYXMNAME>
                                        <LXIP>172.37.90.2 </LXIP>
                                        <LXPORT>8092 </LXPORT>
                                        <STATIONIP>172.37.90.2</STATIONIP>
                                        <STATIONPORT>7091</STATIONPORT>
                                    </MutimeDia>
                                    <MutimeDia diffgr:id="MutimeDia2" msdata:rowOrder="1" xmlns="">
                                        <VIDEOEXIST>0</VIDEOEXIST>
                                        <VIDEOSIZE>0</VIDEOSIZE>
                                        <JOB_DATE>2024-06-06T10:39:24+08:00</JOB_DATE>
                                        <JYLSH>240606370004N0264985</JYLSH>
                                        <JYCS>1</JYCS>
                                        <JYXM>NQ</JYXM>
                                        <SPLX>1</SPLX>
                                        <SPTD>9</SPTD>
                                        <KSSJ>2024-06-06T10:56:52+08:00</KSSJ>
                                        <JSSJ>2024-06-06T10:57:04+08:00</JSSJ>
                                        <ZDXZ>0</ZDXZ>
                                        <JCZBH>3701000098</JCZBH>
                                        <JYXMNAME>联网查询检验</JYXMNAME>
                                        <LXIP>172.37.90.2 </LXIP>
                                        <LXPORT>8092 </LXPORT>
                                        <STATIONIP>172.37.90.2</STATIONIP>
                                        <STATIONPORT>7091</STATIONPORT>
                                    </MutimeDia>
                                    <MutimeDia diffgr:id="MutimeDia3" msdata:rowOrder="2" xmlns="">
                                        <VIDEOEXIST>0</VIDEOEXIST>
                                        <VIDEOSIZE>0</VIDEOSIZE>
                                        <JOB_DATE>2024-06-06T10:39:24+08:00</JOB_DATE>
                                        <JYLSH>240606370004N0264985</JYLSH>
                                        <JYCS>1</JYCS>
                                        <JYXM>NQ</JYXM>
                                        <SPLX>2</SPLX>
                                        <SPTD>10</SPTD>
                                        <KSSJ>2024-06-06T10:56:52+08:00</KSSJ>
                                        <JSSJ>2024-06-06T10:57:04+08:00</JSSJ>
                                        <ZDXZ>0</ZDXZ>
                                        <JCZBH>3701000098</JCZBH>
                                        <JYXMNAME>联网查询检验</JYXMNAME>
                                        <LXIP>172.37.90.2 </LXIP>
                                        <LXPORT>8092 </LXPORT>
                                        <STATIONIP>172.37.90.2</STATIONIP>
                                        <STATIONPORT>7091</STATIONPORT>
                                    </MutimeDia>
                                    <MutimeDia diffgr:id="MutimeDia4" msdata:rowOrder="3" xmlns="">
                                        <VIDEOEXIST>0</VIDEOEXIST>
                                        <VIDEOSIZE>0</VIDEOSIZE>
                                        <JOB_DATE>2024-06-06T10:39:24+08:00</JOB_DATE>
                                        <JYLSH>240606370004N0264985</JYLSH>
                                        <JYCS>1</JYCS>
                                        <JYXM>UC</JYXM>
                                        <SPLX>1</SPLX>
                                        <SPTD>9</SPTD>
                                        <KSSJ>2024-06-06T10:57:05+08:00</KSSJ>
                                        <JSSJ>2024-06-06T10:57:26+08:00</JSSJ>
                                        <ZDXZ>0</ZDXZ>
                                        <JCZBH>3701000098</JCZBH>
                                        <JYXMNAME>唯一性检验</JYXMNAME>
                                        <LXIP>172.37.90.2 </LXIP>
                                        <LXPORT>8092 </LXPORT>
                                        <STATIONIP>172.37.90.2</STATIONIP>
                                        <STATIONPORT>7091</STATIONPORT>
                                    </MutimeDia>
                                    <MutimeDia diffgr:id="MutimeDia5" msdata:rowOrder="4" xmlns="">
                                        <VIDEOEXIST>0</VIDEOEXIST>
                                        <VIDEOSIZE>0</VIDEOSIZE>
                                        <JOB_DATE>2024-06-06T10:39:24+08:00</JOB_DATE>
                                        <JYLSH>240606370004N0264985</JYLSH>
                                        <JYCS>1</JYCS>
                                        <JYXM>UC</JYXM>
                                        <SPLX>2</SPLX>
                                        <SPTD>10</SPTD>
                                        <KSSJ>2024-06-06T10:57:05+08:00</KSSJ>
                                        <JSSJ>2024-06-06T10:57:26+08:00</JSSJ>
                                        <ZDXZ>0</ZDXZ>
                                        <JCZBH>3701000098</JCZBH>
                                        <JYXMNAME>唯一性检验</JYXMNAME>
                                        <LXIP>172.37.90.2 </LXIP>
                                        <LXPORT>8092 </LXPORT>
                                        <STATIONIP>172.37.90.2</STATIONIP>
                                        <STATIONPORT>7091</STATIONPORT>
                                    </MutimeDia>
                                </diffgr:before>
                            </diffgr:diffgram>
                        </GetMutimeDiaByPlateResult>
                    </GetMutimeDiaByPlateResponse>
                </soap:Body>
            </soap:Envelope>
            """;
        //using var stringReader = new StringReader(responseXml);
        //var reader = XmlReader.Create(stringReader);
        //XmlNameTable nameTable = reader.NameTable;
        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
        namespaceManager.AddNamespace("soap2", "http://schemas.xmlsoap.org/soap/envelope/");
        namespaceManager.AddNamespace("r", "http://risservice.hg-banner.com.cn/");
        var doc = XDocument.Parse(responseXml);
        var retVal = doc.XPathSelectElement("//soap2:Body/r:GetMutimeDiaByPlateResponse", namespaceManager)?.ToString();
        var response = new SoapResponse("", responseXml, retVal, namespaceManager, "GetMutimeDiaByPlate");
        var dt = response.ReadReturnValueAsDataTable();
        var d = response.ReadReturnValue();
    }

    [TestMethod]
    public void Result()
    {
        string responseXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""
    xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
    xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
    <soap:Body>
        <GetDetailOFUserResponse xmlns=""http://risservice.hg-banner.com.cn/"">
            <GetDetailOFUserResult>
                <XMLStr>XMLStr</XMLStr>
                <TableName>TableName</TableName>
            </GetDetailOFUserResult>
            <SystemDt>
                <xs:schema id=""NewDataSet"" xmlns="""" xmlns:xs=""http://www.w3.org/2001/XMLSchema""
                    xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata""
                    xmlns:msprop=""urn:schemas-microsoft-com:xml-msprop"">
                    <xs:element name=""NewDataSet"" msdata:IsDataSet=""true"" msdata:MainDataTable=""ds""
                        msdata:UseCurrentLocale=""true"">
                        <xs:complexType>
                            <xs:choice minOccurs=""0"" maxOccurs=""unbounded"">
                                <xs:element name=""ds"" msprop:BaseTable.0=""USERS""
                                    msprop:BaseTable.1=""BASIC_STATION"">
                                    <xs:complexType>
                                        <xs:sequence>
                                            <xs:element name=""USR_ID"" msprop:OraDbType=""117""
                                                msprop:BaseColumn=""USR_ID"" type=""xs:string""
                                                minOccurs=""0"" />
                                            <xs:element name=""USR_NAME"" msprop:OraDbType=""117""
                                                msprop:BaseColumn=""USR_NAME"" type=""xs:string""
                                                minOccurs=""0"" />
                                            <xs:element name=""USR_IDENTITY"" msprop:OraDbType=""117""
                                                msprop:BaseColumn=""USR_IDENTITY"" type=""xs:string""
                                                minOccurs=""0"" />
                                            <xs:element name=""DPT_ID"" msprop:OraDbType=""117""
                                                msprop:BaseColumn=""DPT_ID"" type=""xs:string""
                                                minOccurs=""0"" />
                                            <xs:element name=""STN_ID"" msprop:OraDbType=""104""
                                                msprop:BaseColumn=""STN_ID"" type=""xs:string""
                                                minOccurs=""0"" />
                                            <xs:element name=""STN_NAME"" msprop:OraDbType=""126""
                                                msprop:BaseColumn=""JCZMC"" type=""xs:string""
                                                minOccurs=""0"" />
                                            <xs:element name=""GRP_ID"" msprop:OraDbType=""117""
                                                msprop:BaseColumn=""GRP_ID"" type=""xs:string""
                                                minOccurs=""0"" />
                                            <xs:element name=""USR_ENABLE"" msprop:OraDbType=""111""
                                                msprop:BaseColumn=""USR_ENABLE"" type=""xs:short""
                                                minOccurs=""0"" />
                                            <xs:element name=""USR_DEADLINE"" msprop:OraDbType=""106""
                                                msprop:BaseColumn=""USR_DEADLINE"" type=""xs:dateTime""
                                                minOccurs=""0"" />
                                            <xs:element name=""PSW_DEADLINE"" msprop:OraDbType=""106""
                                                msprop:BaseColumn=""PSW_DEADLINE"" type=""xs:dateTime""
                                                minOccurs=""0"" />
                                        </xs:sequence>
                                    </xs:complexType>
                                </xs:element>
                            </xs:choice>
                        </xs:complexType>
                    </xs:element>
                </xs:schema>
                <diffgr:diffgram xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata""
                    xmlns:diffgr=""urn:schemas-microsoft-com:xml-diffgram-v1"">
                    <DocumentElement xmlns="""">
                        <ds diffgr:id=""ds1"" msdata:rowOrder=""0"">
                            <USR_ID>admin </USR_ID>
                            <USR_NAME>管理员 </USR_NAME>
                            <USR_IDENTITY>000000000000000000</USR_IDENTITY>
                            <STN_ID>4401000081</STN_ID>
                            <STN_NAME>检测服务有限公司</STN_NAME>
                            <GRP_ID>G001</GRP_ID>
                            <USR_ENABLE>1</USR_ENABLE>
                            <USR_DEADLINE>2101-03-31T00:00:00+08:00</USR_DEADLINE>
                            <PSW_DEADLINE>2025-12-31T00:00:00+08:00</PSW_DEADLINE>
                        </ds>
                    </DocumentElement>
                </diffgr:diffgram>
            </SystemDt>
        </GetDetailOFUserResponse>
    </soap:Body>
</soap:Envelope>
";
        //using var stringReader = new StringReader(responseXml);
        //var reader = XmlReader.Create(stringReader);
        //XmlNameTable nameTable = reader.NameTable;
        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
        namespaceManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
        namespaceManager.AddNamespace("r", "http://risservice.hg-banner.com.cn/");
        var doc = XDocument.Parse(responseXml);
        var retVal = doc.XPathSelectElement("//soap:Body/r:GetDetailOFUserResponse", namespaceManager)?.ToString();
        var response = new SoapResponse("", responseXml, retVal, namespaceManager, "GetDetailOFUser");
        var ret = response.ReadReturnValue();
        var o = response.ReadParameterReturnValue();
        var dt = response.ReadParameterReturnValueAsDataTable();
        Assert.IsTrue(o?.SystemDt?.diffgram?.DocumentElement?.ds?.USR_ID == "admin ");
        Assert.IsTrue(o?.SystemDt?.diffgram?.DocumentElement?.ds?.USR_NAME == "管理员 ");
        Assert.IsTrue(o?.SystemDt?.diffgram?.DocumentElement?.ds?.USR_IDENTITY == "000000000000000000");
        Assert.IsTrue(o?.SystemDt?.diffgram?.DocumentElement?.ds?.STN_ID == "4401000081");
        Assert.IsTrue(o?.SystemDt?.diffgram?.DocumentElement?.ds?.STN_NAME == "检测服务有限公司");
        Assert.IsTrue(o?.SystemDt?.diffgram?.DocumentElement?.ds?.GRP_ID == "G001");
        Assert.IsTrue(o?.SystemDt?.diffgram?.DocumentElement?.ds?.USR_ENABLE == "1");
        Assert.IsTrue(o?.SystemDt?.diffgram?.DocumentElement?.ds?.USR_DEADLINE == "2101-03-31T00:00:00+08:00");
        Assert.IsTrue(o?.SystemDt?.diffgram?.DocumentElement?.ds?.PSW_DEADLINE == "2025-12-31T00:00:00+08:00");

    }

    [TestMethod]
    public void TestFault()
    {
        var response = """
            <?xml version="1.0" encoding="utf-8"?><soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><soap:Body><soap:Fault><faultcode>soap:Server</faultcode><faultstring>Server was unable to process request. ---&gt; There was an error generating the XML document. ---&gt; Cannot serialize the DataTable. DataTable name is not set.</faultstring><detail /></soap:Fault></soap:Body></soap:Envelope>
            """;

        var ns = "http://schemas.xmlsoap.org/soap/envelope/";
        var alias = "soap";
        var fault = SoapService.IsSoapFault(response, ns);
        Assert.IsTrue(fault == true);
        var tr = new StringReader(response);
        using var xmlReader = XmlReader.Create(tr);
        var doc = XDocument.Load(xmlReader);
        XmlNameTable nameTable = xmlReader.NameTable;
        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
        namespaceManager.AddNamespace(alias, ns);
        var ex = SoapService.ParseSoapFault(doc, ns, SoapVersion.Soap11, namespaceManager);
        Assert.IsTrue(ex.FaultCode == "soap:Server");
        Assert.IsTrue(ex.FaultString == "Server was unable to process request. ---> There was an error generating the XML document. ---> Cannot serialize the DataTable. DataTable name is not set.");
    }

    [TestMethod]
    public void XmlTest()
    {
        XmlString content = """
            <root><head><returncode>100</returncode> <reason>获取检测信息成功！</reason></head><Vehicle><Row_Detect><JYLSH>23013101902000537</JYLSH><JYCS>2</JYCS><DLSJ>2023/1/31 10:02:09</DLSJ><DLY>黄敏华</DLY><JYXM>F1</JYXM><UNDOJYXM></UNDOJYXM><KSSJ>2023/1/31 10:02:10</KSSJ><JSSJ>2023/1/31 10:08:14</JSSJ><PROCESS>&lt;Row_Process&gt;&lt;JYLSH&gt;23013101902000537&lt;/JYLSH&gt;&lt;JYCS&gt;2&lt;/JYCS&gt;&lt;JYXM&gt;F1&lt;/JYXM&gt;&lt;JYXMNAME&gt;车辆外观检验&lt;/JYXMNAME&gt;&lt;KSSJ&gt;2023/1/31 10:03:37&lt;/KSSJ&gt;&lt;JSSJ&gt;2023/1/31 10:07:44&lt;/JSSJ&gt;&lt;/Row_Process&gt;</PROCESS></Row_Detect><Row_Detect><JYLSH>23013101902000537</JYLSH><JYCS>1</JYCS><DLSJ>2023/1/31 9:01:06</DLSJ><DLY>权正梁</DLY><JYXM>F1,NQ,UC,C1,DC,B1,B2,B0,H1,H4</JYXM><UNDOJYXM></UNDOJYXM><KSSJ>2023/1/31 9:01:06</KSSJ><JSSJ>2023/1/31 9:31:53</JSSJ><PROCESS>&lt;Row_Process&gt;&lt;JYLSH&gt;23013101902000537&lt;/JYLSH&gt;&lt;JYCS&gt;1&lt;/JYCS&gt;&lt;JYXM&gt;B0&lt;/JYXM&gt;&lt;JYXMNAME&gt;驻车制动&lt;/JYXMNAME&gt;&lt;KSSJ&gt;2023/1/31 9:30:54&lt;/KSSJ&gt;&lt;JSSJ&gt;2023/1/31 9:31:17&lt;/JSSJ&gt;&lt;/Row_Process&gt;&lt;Row_Process&gt;&lt;JYLSH&gt;23013101902000537&lt;/JYLSH&gt;&lt;JYCS&gt;1&lt;/JYCS&gt;&lt;JYXM&gt;B2&lt;/JYXM&gt;&lt;JYXMNAME&gt;二轴制动&lt;/JYXMNAME&gt;&lt;KSSJ&gt;2023/1/31 9:30:22&lt;/KSSJ&gt;&lt;JSSJ&gt;2023/1/31 9:30:50&lt;/JSSJ&gt;&lt;/Row_Process&gt;&lt;Row_Process&gt;&lt;JYLSH&gt;23013101902000537&lt;/JYLSH&gt;&lt;JYCS&gt;1&lt;/JYCS&gt;&lt;JYXM&gt;B1&lt;/JYXM&gt;&lt;JYXMNAME&gt;一轴制动&lt;/JYXMNAME&gt;&lt;KSSJ&gt;2023/1/31 9:29:20&lt;/KSSJ&gt;&lt;JSSJ&gt;2023/1/31 9:29:47&lt;/JSSJ&gt;&lt;/Row_Process&gt;&lt;Row_Process&gt;&lt;JYLSH&gt;23013101902000537&lt;/JYLSH&gt;&lt;JYCS&gt;1&lt;/JYCS&gt;&lt;JYXM&gt;H4&lt;/JYXM&gt;&lt;JYXMNAME&gt;右外灯或二三轮机动车的右灯&lt;/JYXMNAME&gt;&lt;KSSJ&gt;2023/1/31 9:26:28&lt;/KSSJ&gt;&lt;JSSJ&gt;2023/1/31 9:27:01&lt;/JSSJ&gt;&lt;/Row_Process&gt;&lt;Row_Process&gt;&lt;JYLSH&gt;23013101902000537&lt;/JYLSH&gt;&lt;JYCS&gt;1&lt;/JYCS&gt;&lt;JYXM&gt;H1&lt;/JYXM&gt;&lt;JYXMNAME&gt;左外灯或二三轮机动车的左灯&lt;/JYXMNAME&gt;&lt;KSSJ&gt;2023/1/31 9:26:27&lt;/KSSJ&gt;&lt;JSSJ&gt;2023/1/31 9:27:01&lt;/JSSJ&gt;&lt;/Row_Process&gt;&lt;Row_Process&gt;&lt;JYLSH&gt;23013101902000537&lt;/JYLSH&gt;&lt;JYCS&gt;1&lt;/JYCS&gt;&lt;JYXM&gt;DC&lt;/JYXM&gt;&lt;JYXMNAME&gt;底盘动态检验&lt;/JYXMNAME&gt;&lt;KSSJ&gt;2023/1/31 9:08:56&lt;/KSSJ&gt;&lt;JSSJ&gt;2023/1/31 9:10:00&lt;/JSSJ&gt;&lt;/Row_Process&gt;&lt;Row_Process&gt;&lt;JYLSH&gt;23013101902000537&lt;/JYLSH&gt;&lt;JYCS&gt;1&lt;/JYCS&gt;&lt;JYXM&gt;C1&lt;/JYXM&gt;&lt;JYXMNAME&gt;底盘检验&lt;/JYXMNAME&gt;&lt;KSSJ&gt;2023/1/31 9:06:07&lt;/KSSJ&gt;&lt;JSSJ&gt;2023/1/31 9:06:51&lt;/JSSJ&gt;&lt;/Row_Process&gt;&lt;Row_Process&gt;&lt;JYLSH&gt;23013101902000537&lt;/JYLSH&gt;&lt;JYCS&gt;1&lt;/JYCS&gt;&lt;JYXM&gt;F1&lt;/JYXM&gt;&lt;JYXMNAME&gt;车辆外观检验&lt;/JYXMNAME&gt;&lt;KSSJ&gt;2023/1/31 9:01:59&lt;/KSSJ&gt;&lt;JSSJ&gt;2023/1/31 9:05:43&lt;/JSSJ&gt;&lt;/Row_Process&gt;&lt;Row_Process&gt;&lt;JYLSH&gt;23013101902000537&lt;/JYLSH&gt;&lt;JYCS&gt;1&lt;/JYCS&gt;&lt;JYXM&gt;UC&lt;/JYXM&gt;&lt;JYXMNAME&gt;唯一性检验&lt;/JYXMNAME&gt;&lt;KSSJ&gt;2023/1/31 9:01:27&lt;/KSSJ&gt;&lt;JSSJ&gt;2023/1/31 9:01:50&lt;/JSSJ&gt;&lt;/Row_Process&gt;&lt;Row_Process&gt;&lt;JYLSH&gt;23013101902000537&lt;/JYLSH&gt;&lt;JYCS&gt;1&lt;/JYCS&gt;&lt;JYXM&gt;NQ&lt;/JYXM&gt;&lt;JYXMNAME&gt;联网查询检验&lt;/JYXMNAME&gt;&lt;KSSJ&gt;2023/1/31 9:01:09&lt;/KSSJ&gt;&lt;JSSJ&gt;2023/1/31 9:01:24&lt;/JSSJ&gt;&lt;/Row_Process&gt;</PROCESS></Row_Detect></Vehicle></root>
            """;
        var d = content.AsDynamic();
        if (d?.root.Vehicle.Row_Detect is IEnumerable<dynamic> dataArray)
        {
            foreach (var item in dataArray)
            {
                XmlString p = item.PROCESS;
                var dd = p.AsDynamic();
                var rp = dd?.root.Row_Process;
                if (rp is IEnumerable<dynamic> ps)
                {
                    Debug.WriteLine("===== List Start =====");
                    foreach (var pr in ps)
                    {
                        Debug.WriteLine($"{pr.JYLSH}");
                        Debug.WriteLine($"{pr.JYCS}");
                        Debug.WriteLine($"{pr.JYXMNAME}");
                        Debug.WriteLine($"{pr.JSSJ}");
                    }
                    Debug.WriteLine("===== List End =====");
                }
                else
                {
                    Debug.WriteLine($"{rp?.JYLSH}");
                    Debug.WriteLine($"{rp?.JYCS}");
                    Debug.WriteLine($"{rp?.JYXMNAME}");
                    Debug.WriteLine($"{rp?.JSSJ}");
                }
            }
        }
    }
}
