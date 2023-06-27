using System.ComponentModel.DataAnnotations;

namespace PhimMoi.Areas.Admin.Models.Category
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Chưa nhập tên.")]
        public string Name { get; set; }
        public string? Description { get; set; }
    }    
}