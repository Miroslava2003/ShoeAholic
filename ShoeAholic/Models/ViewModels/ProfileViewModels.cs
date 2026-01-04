using System.ComponentModel.DataAnnotations;

namespace ShoeAholic.Models.ViewModels
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int OrdersCount { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Моля въведете име")]
        [Display(Name = "Име")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете фамилия")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете имейл")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете телефон")]
        [RegularExpression(@"^(?=(?:.*\d){10,})[0-9+\-\s()]+$",
            ErrorMessage = "Телефонният номер трябва да съдържа поне 10 цифри")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = null!;
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Моля въведете текущата парола")]
        [DataType(DataType.Password)]
        [Display(Name = "Текуща парола")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете нова парола")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Паролата трябва да е поне 8 символа")]
        [DataType(DataType.Password)]
        [Display(Name = "Нова парола")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Моля потвърдете новата парола")]
        [DataType(DataType.Password)]
        [Display(Name = "Потвърди парола")]
        [Compare("NewPassword", ErrorMessage = "Паролите не съвпадат")]
        public string ConfirmPassword { get; set; } = null!;
    }
}











