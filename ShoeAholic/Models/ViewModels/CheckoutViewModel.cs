using ShoeAholic.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShoeAholic.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Моля въведете пълно име")]
        [Display(Name = "Пълно име")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете имейл")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете телефон")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Моля изберете населено място")]
        [Display(Name = "Населено място")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете пощенски код")]
        [Display(Name = "Пощенски код")]
        public string PostalCode { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете адрес или офис")]
        [Display(Name = "Адрес / Офис")]
        public string Address { get; set; } = null!;

        [Required]
        [Display(Name = "Начин на доставка")]
        public DeliveryType DeliveryType { get; set; }

        [Required]
        [Display(Name = "Начин на плащане")]
        public PaymentType PaymentType { get; set; }

        public decimal SubTotal { get; set; }
        public decimal DeliveryPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int ItemsCount { get; set; }
    }
}








