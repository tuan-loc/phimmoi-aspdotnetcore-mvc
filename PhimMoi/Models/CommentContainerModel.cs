using PhimMoi.Models.Comment;

namespace PhimMoi.Models
{
    public class CommentContainerModel
    {
        public int CommentCount { get; set; }
        public bool RenderCommentOnly { get; set; }
        public string? UserAvatar { get; set; }
        public bool UserLogin { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsEnd { get; set; }
        public string MovieId { get; set; }
        public List<CommentViewModel>? Comments { get; set; }
    }
}