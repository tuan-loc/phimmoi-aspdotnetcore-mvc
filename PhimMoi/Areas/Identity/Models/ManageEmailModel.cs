using System.ComponentModel.DataAnnotations;

namespace PhimMoi.Areas.Identity.Models
{
    public class ManageEmailModel
    {
        public bool IsEmailConfirmed { get; set; }
        public string Email { get; set; }

        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }
    }
}