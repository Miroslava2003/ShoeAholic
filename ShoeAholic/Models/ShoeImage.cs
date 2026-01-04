using System.ComponentModel.DataAnnotations;

namespace ShoeAholic.Models
{
    public class ShoeImage
    {
        public int Id { get; set; }

        [Required]
        public int ShoeId { get; set; }

        [Required]
        [Display(Name = "Снимка")]
        public string ImageUrl { get; set; } = null!;

        public Shoe Shoe { get; set; } = null!;
    }
}

