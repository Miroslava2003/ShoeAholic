namespace ShoeAholic.Models
{
    public class WishlistItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ShoeId { get; set; }
        public DateTime AddedOn { get; set; } = DateTime.Now;
        public Shoe Shoe { get; set; } = null!;
    }
}