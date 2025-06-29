namespace Lavender_Veil.Models
{
    public class WishlistItem
    {
        public int WishlistItemId { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public string? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }

}
