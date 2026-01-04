namespace ShoeAholic.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int ShoeId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }
        public Shoe? Shoe { get; set; }
        public ShoeSize? ShoeSize { get; set; }
    }
}








