using System.ComponentModel.DataAnnotations;

namespace Grocery.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public int ProductId { get; set; }

        public decimal Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        //Navigation property
        public virtual Products Product {
            get; set;
        }
    }
}
