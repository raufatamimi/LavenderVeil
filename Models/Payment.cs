using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Lavender_Veil.Models
{
    public class Payment
    {
        [Key]
        public string? PaymentId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        [StringLength(20)]
        public string? Method { get; set; } // credit_card, paypal, bank_transfer

        [StringLength(100)]
        public string? TransactionId { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        public string? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
