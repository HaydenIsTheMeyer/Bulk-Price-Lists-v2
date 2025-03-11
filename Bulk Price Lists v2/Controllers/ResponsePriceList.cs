using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bulk_Price_Lists_v2.Controllers
{
    public class ResponsePriceList
    {
        [XmlRoot("response")]
        public class Response
        {
            [XmlElement("control")]
            public Control Control { get; set; }

            [XmlElement("operation")]
            public Operation Operation { get; set; }
        }

        public class Control
        {
            [XmlElement("status")]
            public string Status { get; set; }

            [XmlElement("senderid")]
            public string SenderId { get; set; }

            [XmlElement("controlid")]
            public string ControlId { get; set; }

            [XmlElement("uniqueid")]
            public bool UniqueId { get; set; }

            [XmlElement("dtdversion")]
            public string DtdVersion { get; set; }
        }

        public class Operation
        {
            [XmlElement("authentication")]
            public Authentication Authentication { get; set; }

            [XmlElement("result")]
            public Result Result { get; set; }
        }

        public class Authentication
        {
            [XmlElement("status")]
            public string Status { get; set; }

            [XmlElement("userid")]
            public string UserId { get; set; }

            [XmlElement("companyid")]
            public string CompanyId { get; set; }

            [XmlElement("locationid")]
            public string LocationId { get; set; }

            [XmlElement("sessiontimestamp")]
            public DateTime SessionTimestamp { get; set; }

            [XmlElement("sessiontimeout")]
            public DateTime SessionTimeout { get; set; }
        }

        public class Result
        {
            [XmlElement("status")]
            public string Status { get; set; }

            [XmlElement("function")]
            public string Function { get; set; }

            [XmlElement("controlid")]
            public string ControlId { get; set; }

            [XmlElement("data")]
            public Data Data { get; set; }
        }

        public class Data
        {
            [XmlAttribute("listtype")]
            public string ListType { get; set; }

            [XmlAttribute("totalcount")]
            public int TotalCount { get; set; }

            [XmlAttribute("offset")]
            public int Offset { get; set; }

            [XmlAttribute("count")]
            public int Count { get; set; }

            [XmlAttribute("numremaining")]
            public int NumRemaining { get; set; }

            [XmlElement("SOPRICELIST")]
            public List<Sopricelist> Sopricelist { get; set; }
        }

        public class Sopricelist
        {
            [XmlElement("RECORDNO")]
            public int RecordNo { get; set; }

            [XmlElement("NAME")]
            public string Name { get; set; }
        }

        public class PriceLists
        {
            public static void SetPriceLists(string xmlResponse)
            {
                

                // Deserialize XML response
                var serializer = new XmlSerializer(typeof(Response));
                using (var stringReader = new System.IO.StringReader(xmlResponse))
                {
                    HomeController.priceLists = (Response)serializer.Deserialize(stringReader);


                }
            }
        }
    }
}
