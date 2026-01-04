using System.ComponentModel.DataAnnotations;

namespace ShoeAholic.Models
{
    public class ShoeSize
    {
        public int Id { get; set; }

        [Required]
        public int ShoeId { get; set; }

        [Required(ErrorMessage = "Размерът е задължителен")]
        [Range(36, 50, ErrorMessage = "Размерът трябва да е между 36 и 50")]
        [Display(Name = "Размер")]
        public decimal Size { get; set; }

        [Required(ErrorMessage = "Количеството е задължително")]
        [Range(0, 1000, ErrorMessage = "Количеството трябва да е положително число")]
        [Display(Name = "Количество")]
        public int Quantity { get; set; }

        public Shoe Shoe { get; set; } = null!;
    }
}
