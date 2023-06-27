using System.ComponentModel.DataAnnotations;

namespace PhimMoi.Areas.Identity.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}