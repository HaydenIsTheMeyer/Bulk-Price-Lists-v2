using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Bulk_Price_Lists_v2.Controllers
{
    public class RequestUpdateCustomer
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

            [XmlElement("update")]
            public Update Update { get; set; }
        }

        public class Update
        {
            [XmlElement("CUSTOMER")]
            public ResponseCustomer.Customer[] Customers { get; set; }
        }



        public class UpdateCustomer
        {
            public static async Task UpdateCustomers(ResponseCustomer.Customer[] customers)
            {
                // Example of creating the request object
                var request = new Request
                {
                    Control = new Control
                    {
                        SenderId = "agroserve",
                        Password = "@Agr0s3rv3!@#",
                        ControlId = "{{$timestamp}}",
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
                                ControlId = "{{$guid}}",
                                Update = new Update
                                {
                                    Customers = customers
                                
                                }
                            }
                        }
                    }
                };

                // Serialize the request object to XML
                //var serializer = new XmlSerializer(typeof(Request));
                //using (var stringWriter = new StringWriter())
                //{
                //    serializer.Serialize(stringWriter, request);
                //    string xmlRequest = stringWriter.ToString();
                //    Console.WriteLine(xmlRequest);

                //    // Here you would send the XML request to your API or service
                //    // e.g., make an HTTP request with the xmlRequest as the body
                //}

                string xmlRequest = SerializeToXml(request);
                string apiUrl = "https://api.intacct.com/ia/xml/xmlgw.phtml";

                string response = await SendXmlRequest(apiUrl, xmlRequest);
            }
            public static string SerializeToXml(Request request)
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



            public static async Task<string> SendXmlRequest(string url, string xmlData)
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
