namespace Grocery.Models
{
    public class Payment
    {
        public int Id { get; set; }

        public string userId { get; set; }

        public int orderId { get; set; }

        public string paymentMethod { get; set; } = "COD";

        public DateTime paymentDate { get; set; } = DateTime.Now;

        public decimal amount { get; set; }

    }
}
