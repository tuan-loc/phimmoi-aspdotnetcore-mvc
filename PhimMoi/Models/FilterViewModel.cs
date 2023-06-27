using Microsoft.AspNetCore.Mvc.Rendering;

namespace PhimMoi.Models
{
    public class FilterViewModel
    {
        public List<SelectListItem> Categories { get; set; }
        public SelectList Countries { get; set; }
        public SelectList Years { get; set; }
        public SelectList OrderByOptions { get; set; }
        public SelectList Types { get; set; }
    }
}
