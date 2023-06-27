using System.ComponentModel.DataAnnotations;

namespace PhimMoi.Areas.Admin.Models.Country
{
    public class CreateCountryViewModel
    {
        [Required]
        public string Name { get; set; }
        public string? About { get; set; }
    }
}