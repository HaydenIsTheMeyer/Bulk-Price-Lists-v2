using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bulk_Price_Lists_v2.Models
{
    public class Display
    {
        public List<SelectListItem> CustomerGroups { get; set; }
        public List<SelectListItem> PriceLists { get; set; }
        public List<Customer> Customers { get; set; }


    }
    public class Customer
    {
        public string Name { get; set; }

        public string PriceList { get; set; }

        public string GlGroup { get; set; }
    }
}
