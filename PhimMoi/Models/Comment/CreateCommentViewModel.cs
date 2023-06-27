namespace PhimMoi.Models.Comment
{
    public class CreateCommentViewModel
    {
        public string MovieId { get; set; }
        public string Content { get; set; }
        public int ResponseToId { get; set; }
    }
}