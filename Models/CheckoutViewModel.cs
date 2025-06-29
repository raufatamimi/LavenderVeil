using System.ComponentModel.DataAnnotations;

namespace Lavender_Veil.Models
{
    public class CheckoutViewModel
    {
        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public string ShippingCity { get; set; }

        [Required]
        [Phone]
        public string ShippingPhone { get; set; }

        public string Notes { get; set; }

        public string? PaymentMethod { get; set; }
    }

}
