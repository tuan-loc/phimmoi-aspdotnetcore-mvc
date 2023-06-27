namespace PhimMoi.Models
{
    public class PagingModel
    {
        public int NumberOfPages { get; set; }
        public int CurrentPage { get; set; }
        public Func<int, string> Callback { get; set; }
    }
}