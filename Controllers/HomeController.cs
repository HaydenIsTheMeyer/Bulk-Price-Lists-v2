using System.Diagnostics;
using Bulk_Price_Lists_v2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Bulk_Price_Lists_v2.Models.Display;

namespace Bulk_Price_Lists_v2.Controllers
{
    public class HomeController : Controller
    {

        public static ResponseCustomer.Response customer { get; set; }

        public static ResponsePriceList.Response priceLists { get; set; }

        private readonly ILogger<HomeController> _logger;

        public static string SessionId { get; private set; } = "2D2Cdhf8qKiOJjPwyx7eWF8FqI8notg9gitKhRy1jiYz8Mse3ktRC6iO";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Call the async methods and await their completion
            await RequestCustomer.Customers.GetCustomers();
            await RequestPriceList.Pricelists.GetPriceLists();

            // Now that the data is available, execute the LINQ expression
            var uniqueGLGroups = customer.Operation.Result.Data.Customers
                .Select(c => c.GlGroup)
                .Where(glg => !string.IsNullOrEmpty(glg))
                .Distinct()
                .ToList();



            // Populate CustomerGroups
            List<SelectListItem> CustomerGroups = new List<SelectListItem>();

            SelectListItem Allitem = new SelectListItem { Value = "All", Text = "All" };
            CustomerGroups.Add(Allitem);

            foreach (var group in uniqueGLGroups)
            {
                SelectListItem item = new SelectListItem { Value = group, Text = group };
                CustomerGroups.Add(item);
            }

            // Populate PriceLists
            List<SelectListItem> PriceLists = new List<SelectListItem>();
            foreach (var sopricelist in priceLists.Operation.Result.Data.Sopricelist)
            {
                SelectListItem item = new SelectListItem { Value = sopricelist.Name, Text = sopricelist.Name };
                PriceLists.Add(item);
            }

            List<Customer> customers = new List<Customer>();
            foreach (var customer in customer.Operation.Result.Data.Customers)
            {
                Customer customer1 = new Customer();
                customer1.Name = customer.Name;
                customer1.PriceList = customer.PriceList;
                customer1.GlGroup = customer.GlGroup;

                customers.Add(customer1);
            }
            // Create and populate the model
            var model = new Display
            {
                CustomerGroups = CustomerGroups,
                PriceLists = PriceLists,
                Customers = customers
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult GetFilteredCustomers(string[] group) // Change to string[]
        {
            var filteredCustomers = customer.Operation.Result.Data.Customers
                                    .Select(c => new Customer
                                    {
                                        Name = c.Name,
                                        GlGroup = c.GlGroup,
                                        PriceList = c.PriceList
                                    })
                                    .ToList();

            if (!group.Contains("All"))
            {
                 filteredCustomers = customer.Operation.Result.Data.Customers
                .Where(c => group.Contains(c.GlGroup)) // Filter by multiple groups
                .Select(c => new Customer
                {
                    Name = c.Name,
                    GlGroup = c.GlGroup,
                    PriceList = c.PriceList
                })
                .ToList();
            }


            return PartialView("_CustomerGrid", filteredCustomers);
        }

        [HttpPost]
        public IActionResult SetPriceLists(string[] customerGroupIds, string priceListId, List<string> selectedCustomers)
        {
            // Filter customers based on selectedCustomers
            var updatedCustomers = customer.Operation.Result.Data.Customers
                .Where(c => selectedCustomers.Contains(c.Name))
                .Select(c => new Customer { Name = c.Name })
                .ToList();

            var customerstoUpdate = customer.Operation.Result.Data.Customers
                .Where(c => selectedCustomers.Contains(c.Name))
                .Select(c => new ResponseCustomer.Customer
                {
                    RecordNo = c.RecordNo,
                    CustomerId = c.CustomerId,
                    Name = c.Name,
                    GlGroup = c.GlGroup,
                    PriceList = c.PriceList
                })
                .ToList();








            foreach (ResponseCustomer.Customer cust in customerstoUpdate)
            {
                cust.PriceList = priceListId;

            }

            ResponseCustomer.Customer[] customers = customerstoUpdate.ToArray();

            RequestUpdateCustomer.UpdateCustomer.UpdateCustomers(customers);

            // Create the view model
            UpdateResultViewModel viewModel = new UpdateResultViewModel
            {
                Success = true, // Set to false if update fails
                UpdatedCustomers = updatedCustomers
            };

            // Redirect to the new result page
            return RedirectToAction("UpdateResult", viewModel);
        }

        [HttpGet]
        public IActionResult UpdateResult(UpdateResultViewModel model)
        {
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
