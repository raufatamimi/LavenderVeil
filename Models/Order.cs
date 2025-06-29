using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Lavender_Veil.Models
{
    public class Order
    {
        [Key]
        public string? OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        [StringLength(20)]
        public string? Status { get; set; } 

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        public string? ShippingAddress { get; set; }
        public string? ShippingCity { get; set; }
        public string? ShippingPhone { get; set; }
        public string? Notes { get; set; }  

        public String? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        public ICollection<OrderItem>? OrderItems { get; set; }
        public Payment? Payment { get; set; }
    }
}
