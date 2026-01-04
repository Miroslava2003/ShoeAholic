using System.ComponentModel.DataAnnotations;

namespace ShoeAholic.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Моля въведете име")]
        [StringLength(100, ErrorMessage = "Името не може да е повече от 100 символа")]
        [Display(Name = "Име")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете имейл")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес")]
        [Display(Name = "Имейл")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете телефон")]
        [Phone(ErrorMessage = "Невалиден телефонен номер")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Моля въведете съобщение")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Съобщението трябва да е между 10 и 2000 символа")]
        [Display(Name = "Съобщение")]
        public string Message { get; set; } = null!;

        [Display(Name = "Дата")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Display(Name = "Прочетено")]
        public bool IsRead { get; set; } = false;
    }
}