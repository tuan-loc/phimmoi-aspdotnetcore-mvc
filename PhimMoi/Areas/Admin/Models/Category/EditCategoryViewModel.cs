using System.ComponentModel.DataAnnotations;

namespace PhimMoi.Areas.Admin.Models.Category
{
    public class EditCategoryViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Chưa nhập tên.")]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}