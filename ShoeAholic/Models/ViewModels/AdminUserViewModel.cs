namespace ShoeAholic.Models.ViewModels
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsAdmin { get; set; }
        public bool IsSelf { get; set; }
    }
}
