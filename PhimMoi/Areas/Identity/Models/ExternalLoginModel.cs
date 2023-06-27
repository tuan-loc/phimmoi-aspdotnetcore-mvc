using System.ComponentModel.DataAnnotations;

namespace PhimMoi.Areas.Identity.Models
{
    public class ExternalLoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string ProviderDisplayName { get; set; }
    }
}