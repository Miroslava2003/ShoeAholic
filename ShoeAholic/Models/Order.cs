using ShoeAholic.Models.Enums;

namespace ShoeAholic.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public decimal SubTotal { get; set; }
        public decimal DeliveryPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;

        public string City { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Address { get; set; } = null!;

        public DeliveryType DeliveryType { get; set; }
        public PaymentType PaymentType { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? AdminNotes { get; set; }

        public bool IsPaid { get; set; } = false;
        public string? PaymentTransactionId { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
