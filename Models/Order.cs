namespace Grocery.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string userId { get; set; }

        public DateTime orderDate { get; set; }

        public string status { get; set; } = "Pending";

        public decimal totalAmount { get; set; }
    }
}
