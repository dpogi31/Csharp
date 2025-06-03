namespace Grocery.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int orderId { get; set; }

        public int cartId { get; set; }

        public decimal quantity { get; set; }

        public decimal price { get; set; }

        public decimal totalPrice {  get; set; }

        public decimal discount { get; set; } = 0;
    }
}
