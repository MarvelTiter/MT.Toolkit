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

namespace WebServiceTest
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            var client = new HttpClient();
            var ws = new SoapService(client, "", "");
            var response = await ws.SendAsync("Login", new()
            {
                ["szUser"] = "admin",
                ["szPass"] = "admin123"
            });
        }

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
            var response = new SoapResponse(responseXml, retVal, namespaceManager, "GetDetailOFUser");
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