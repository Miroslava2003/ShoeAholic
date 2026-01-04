namespace ShoeAholic.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ShoeId { get; set; }
        public Shoe Shoe { get; set; }

        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
