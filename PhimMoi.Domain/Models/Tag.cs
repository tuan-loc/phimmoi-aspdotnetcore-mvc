using System.ComponentModel.DataAnnotations;

namespace PhimMoi.Domain.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Movie Movie { get; set; }
        public string TagName { get; set; }
    }
}