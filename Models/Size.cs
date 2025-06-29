using System.ComponentModel.DataAnnotations;

namespace Lavender_Veil.Models
{
    public class Size
    {
        [Key]
        public int SizeId { get; set; }

        [Required, StringLength(10)]
        public string Name { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}
