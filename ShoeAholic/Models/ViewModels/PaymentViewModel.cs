using System.ComponentModel.DataAnnotations;

namespace ShoeAholic.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Моля въведете номер на карта")]
        [Display(Name = "Номер на карта")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Номерът на картата трябва да е 16 цифри")]
        public string CardNumber { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете име на картодържателя")]
        [Display(Name = "Име на картодържателя")]
        public string CardHolderName { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете срок на валидност")]
        [Display(Name = "Срок на валидност (ММ/ГГ)")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Формат: ММ/ГГ")]
        public string ExpiryDate { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете CVV")]
        [Display(Name = "CVV")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV трябва да е 3 цифри")]
        public string CVV { get; set; } = null!;
    }
}









