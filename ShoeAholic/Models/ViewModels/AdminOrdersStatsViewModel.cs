namespace ShoeAholic.Models.ViewModels
{
    public class AdminOrdersStatsViewModel
    {
        public int TodayOrders { get; set; }
        public decimal TodayRevenue { get; set; }

        public int WeekOrders { get; set; }
        public decimal WeekRevenue { get; set; }

        public int MonthOrders { get; set; }
        public decimal MonthRevenue { get; set; }

        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int ShippedOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }

        public decimal AverageOrderValue { get; set; }
    }
}
