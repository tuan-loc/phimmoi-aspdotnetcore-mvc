using PhimMoi.Models.Category;
using PhimMoi.Models.Country;
using PhimMoi.Models.User;

namespace PhimMoi.Models
{
    public class HeaderNavBarViewModel
    {
        public UserViewModel? User { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public List<CountryViewModel> Countries { get; set; }
        public List<int> Years { get; set; }
    }
}
