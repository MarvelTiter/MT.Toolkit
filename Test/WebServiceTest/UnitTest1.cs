using Microsoft.Extensions.DependencyInjection;
using SimpleSOAPClient.Exceptions;
using SimpleSOAPClient.Handlers;
using SimpleSOAPClient.Models;
using SimpleSOAPClient;
using SimpleSOAPClient.Helpers;
using SimpleSOAPClient.Models.Headers;
using SimpleSOAPClient.Models.Headers.Oasis.Security;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Data;
using System.Runtime.InteropServices;
using System.Xml.XPath;
using MT.Toolkit.XmlHelper;

namespace WebServiceTest
{
    public class UnitTest1
    {
      
        [Fact]
        public async void ServiceCollectionTest()
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

        [Fact]
        public void ToDataTable()
        {
            string xml = """
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

        }

        [Fact]
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
            using var stringReader = new StringReader(responseXml);
            var reader = XmlReader.Create(stringReader);
            XmlNameTable nameTable = reader.NameTable;
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
            namespaceManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            namespaceManager.AddNamespace("r", "http://risservice.hg-banner.com.cn/");
            var doc = XElement.Load(reader);
            var retVal = doc.XPathSelectElement("//soap:Body/r:GetDetailOFUserResponse", namespaceManager)?.ToString();
            var response = new SoapResponse("",responseXml, retVal, namespaceManager, "GetDetailOFUser");
            var ret = response.ReadReturnValue();
            var o = response.ReadParameterReturnValue();

            Assert.True(o?.diffgram?.DocumentElement?.ds?.USR_ID == "admin ");
            Assert.True(o?.diffgram?.DocumentElement?.ds?.USR_NAME == "管理员 ");
            Assert.True(o?.diffgram?.DocumentElement?.ds?.USR_IDENTITY == "000000000000000000");
            Assert.True(o?.diffgram?.DocumentElement?.ds?.STN_ID == "4401000081");
            Assert.True(o?.diffgram?.DocumentElement?.ds?.STN_NAME == "检测服务有限公司");
            Assert.True(o?.diffgram?.DocumentElement?.ds?.GRP_ID == "G001");
            Assert.True(o?.diffgram?.DocumentElement?.ds?.USR_ENABLE == "1");
            Assert.True(o?.diffgram?.DocumentElement?.ds?.USR_DEADLINE == "2101-03-31T00:00:00+08:00");
            Assert.True(o?.diffgram?.DocumentElement?.ds?.PSW_DEADLINE == "2025-12-31T00:00:00+08:00");

        }
    }
}