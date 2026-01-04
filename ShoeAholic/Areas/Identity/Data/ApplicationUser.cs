using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ShoeAholic.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string Име { get; set; } = null!;

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string Фамилия { get; set; } = null!;

        [PersonalData]
        [Column(TypeName = "nvarchar(20)")]
        public string Телефон { get; set; } = null!;
    }
}