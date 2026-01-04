using System.ComponentModel.DataAnnotations;

namespace ShoeAholic.Models
{
    public class Shoe
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Марката е задължителна")]
        [StringLength(50, ErrorMessage = "Максимум 50 символа")]
        [Display(Name = "Марка")]
        public string Brand { get; set; } = null!;

        [Required(ErrorMessage = "Моделът е задължителен")]
        [StringLength(50, ErrorMessage = "Максимум 50 символа")]
        [Display(Name = "Модел")]
        public string Model { get; set; } = null!;

        [Required(ErrorMessage = "Категорията е задължителна")]
        [Display(Name = "Категория")]
        public string Category { get; set; } = null!;

        [Required(ErrorMessage = "Цветът е задължителен")]
        [StringLength(30)]
        [Display(Name = "Цвят")]
        public string Color { get; set; } = null!;

        [Required(ErrorMessage = "Цената е задължителна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цената трябва да е по-голяма от 0 лв.")]
        [Display(Name = "Цена (лв.)")]
        public decimal Price { get; set; }

        [StringLength(1000, ErrorMessage = "Максимум 1000 символа")]
        [Display(Name = "Описание")]
        public string? Description { get; set; }
        public ICollection<ShoeSize> Sizes { get; set; } = new List<ShoeSize>();
        public ICollection<ShoeImage> Images { get; set; } = new List<ShoeImage>();
    }
}
