using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Lavender_Veil.Models
{
        public class Customer : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string? Name { get; set; }  

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
