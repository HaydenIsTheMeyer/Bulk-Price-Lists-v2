using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System;
using System.IO;
using System.Xml;

namespace Bulk_Price_Lists_v2.Controllers
{
    public class RequestCustomer
    {


        [XmlRoot("request")]
        public class Request
        {
            [XmlElement("control")]
            public Control Control { get; set; }

            [XmlElement("operation")]
            public Operation Operation { get; set; }
        }

        public class Control
        {
            [XmlElement("senderid")]
            public string SenderId { get; set; }

            [XmlElement("password")]
            public string Password { get; set; }

            [XmlElement("controlid")]
            public string ControlId { get; set; }

            [XmlElement("uniqueid")]
            public bool UniqueId { get; set; }

            [XmlElement("dtdversion")]
            public string DtdVersion { get; set; }

            [XmlElement("includewhitespace")]
            public bool IncludeWhitespace { get; set; }
        }

        public class Operation
        {
            [XmlElement("authentication")]
            public Authentication Authentication { get; set; }

            [XmlElement("content")]
            public Content Content { get; set; }
        }

        public class Authentication
        {
            [XmlElement("sessionid")]
            public string SessionId { get; set; }
        }

        public class Content
        {
            [XmlElement("function")]
            public Function Function { get; set; }
        }

        public class Function
        {
            [XmlAttribute("controlid")]
            public string ControlId { get; set; }

            [XmlElement("query")]
            public Query Query { get; set; }
        }

        public class Query
        {
            [XmlElement("object")]
            public string Object { get; set; }

            [XmlElement("select")]
            public Select Select { get; set; }
        }

        public class Select
        {
            [XmlElement("field")]
            public string[] Fields { get; set; }
        }

       public  class Customers
        {
            public static async Task GetCustomers()
            {
                var request = new Request
                {
                    Control = new Control
                    {
                        SenderId = "agroserve",
                        Password = "@Agr0s3rv3!@#",
                        ControlId = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                        UniqueId = false,
                        DtdVersion = "3.0",
                        IncludeWhitespace = false
                    },
                    Operation = new Operation
                    {
                        Authentication = new Authentication
                        {
                            SessionId = "u-8_wu9GCxvGUc3kVp0lpK6mG8eVLLv-ExQK8cVRxlHN5FadJYLr41fG"
                        },
                        Content = new Content
                        {
                            Function = new Function
                            {
                                ControlId = Guid.NewGuid().ToString(),
                                Query = new Query
                                {
                                    Object = "CUSTOMER",
                                    Select = new Select
                                    {
                                        Fields = new[] { "RECORDNO", "CUSTOMERID", "NAME", "GLGROUP", "PRICELIST" }
                                    }
                                }
                            }
                        }
                    }
                };

                string xmlRequest = SerializeToXml(request);
                string apiUrl = "https://api.intacct.com/ia/xml/xmlgw.phtml";

                string response = await SendXmlRequest(apiUrl, xmlRequest);

              HomeController.customer = ResponseCustomer.XmlHelper.DeserializeFromXml(response);

                Console.WriteLine("API Response:");
                Console.WriteLine(response);
            }

            static string SerializeToXml(Request request)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Request));
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", ""); // Removes unwanted xmlns:xsi and xmlns:xsd

                XmlWriterSettings settings = new XmlWriterSettings
                {
                    Encoding = new UTF8Encoding(false), // Ensures UTF-8 without BOM
                    Indent = true,
                    OmitXmlDeclaration = false // Keeps the XML declaration (optional)
                };

                using (StringWriter utf8StringWriter = new Utf8StringWriter())
                using (XmlWriter writer = XmlWriter.Create(utf8StringWriter, settings))
                {
                    serializer.Serialize(writer, request, namespaces);
                    return utf8StringWriter.ToString();
                }
            }

            // Custom StringWriter to enforce UTF-8 encoding
            public class Utf8StringWriter : StringWriter
            {
                public override Encoding Encoding => new UTF8Encoding(false); // UTF-8 without BOM
            }



            static async Task<string> SendXmlRequest(string url, string xmlData)
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(xmlData, Encoding.UTF8, "application/xml");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }



    }
}
