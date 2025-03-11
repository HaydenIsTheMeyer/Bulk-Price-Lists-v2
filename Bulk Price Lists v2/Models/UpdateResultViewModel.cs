namespace Bulk_Price_Lists_v2.Models
{
    public class UpdateResultViewModel
    {
        public bool Success { get; set; } = true;
        public string ErrorMessage { get; set; }
        public List<Customer> UpdatedCustomers { get; set; } = new List<Customer>();
    }
}
