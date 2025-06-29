using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lavender_Veil.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public string? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}