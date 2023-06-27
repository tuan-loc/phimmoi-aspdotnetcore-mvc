using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimMoi.Application.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Models;
using PhimMoi.Models.Comment;
using PhimMoi.SharedLibrary.Constants;
using System.Text.Encodings.Web;

namespace PhimMoi.Controllers
{
    [Route("[controller]")]
    public class CommentController : Controller
    {
        public const int COMMENTS_PER_PAGE = 10;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IMovieService _movieService;
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;

        public CommentController(IMapper mapper, IEmailSender emailSender, IMovieService movieService, ICommentService commentService, IUserService userService)
        {
            _mapper = mapper;
            _emailSender = emailSender;
            _movieService = movieService;
            _commentService = commentService;
            _userService = userService;
        }

        [HttpGet("get-comments-partial")]
        public async Task<IActionResult> GetCommentsPartial(int page, string movieId)
        {
            Movie? movie = await _movieService.GetByIdAsync(movieId);
            if (movie == null)
            {
                return Json("null");
            }

            PagedList<Comment> comments = await _commentService.GetByMovieIdAsync(movieId, new PagingParameter(page, COMMENTS_PER_PAGE));
            User? user = await _userService.GetByClaims(User);
            CommentContainerModel model = new()
            {
                IsEnd = page >= comments.TotalPage,
                Comments = _mapper.Map<List<CommentViewModel>>(comments),
                UserLogin = _userService.IsSignIn(User),
                CommentCount = comments.TotalItems,
                RenderCommentOnly = false,
                UserAvatar = user?.Avatar,
                MovieId = movie.Id,
                IsAdmin = user != null && user.RoleName != null && (user.RoleName == RoleConstant.ADMIN || user.RoleName == RoleConstant.THUY_TO)
            };
            return this.PartialView("_CommentContainerPartial", model);
        }

        [HttpGet("load-more-comments")]
        public async Task<IActionResult> LoadMoreComments(int page, string movieId)
        {
            Movie? movie = await _movieService.GetByIdAsync(movieId);
            if (movie == null)
            {
                return Json("null");
            }

            User? user = await _userService.GetByClaims(User);
            PagedList<Comment> comments = await _commentService.GetByMovieIdAsync(movieId, new PagingParameter(page, COMMENTS_PER_PAGE));
            CommentContainerModel model = new()
            {
                IsEnd = page >= comments.TotalPage,
                Comments = _mapper.Map<List<CommentViewModel>>(comments),
                IsAdmin = user != null && user.RoleName != null && (user.RoleName == RoleConstant.ADMIN || user.RoleName == RoleConstant.THUY_TO)
            };
            return this.PartialView("_CommentContainerPartial", model);
        }

        [HttpPost("create")]
        public async Task<JsonResult> CreateComment(CreateCommentViewModel? model)
        {
            User? user = await _userService.GetByClaims(User);
            Movie? movie = await _movieService.GetByIdAsync(model.MovieId);

            if (user == null || movie == null)
            {
                return Json(new { success = false });
            }

            Comment? responseToComment = model.ResponseToId > 0 ? await _commentService.GetByIdAsync(model.ResponseToId) : null;
            Comment comment = new()
            {
                User = user,
                Movie = movie,
                Content = model.Content,
                CreatedAt = DateTime.Now,
                Like = 0,
                ResponseTo = responseToComment
            };

            try
            {
                await _commentService.CreateAsync(comment);
            }
            catch
            {
                return Json(new { success = false });
            }

            if (responseToComment != null)
            {
                var callbackUrl = Url.Action("Detail", "Movie", values: new { area = "", id = movie.Id }, protocol: Request.Scheme);
                _emailSender.SendEmailAsync(responseToComment.User.Email, "Thông báo", $"Ai đó vừa phản hồi Comment của bạn về bộ phim {movie.Name}. " + $"<a href='{HtmlEncoder.Default.Encode(callbackUrl ?? "")}'>Ấn vào đây</a> để đi đến trang Web PhimMoi.");
            }

            return Json(new
            {
                success = true,
                useravatar = user.Avatar ?? "/src/img/UserAvatars/default_avatar.png",
                username = user.DisplayName,
                cmtcontent = model.Content,
                userrole = user.RoleName ?? ""
            });
        }

        [HttpPost("like-comment")]
        public async Task<JsonResult> LikeComment(int commentId)
        {
            try
            {
                await _commentService.LikeComment(commentId);
            }
            catch
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }

        [HttpPost("delete")]
        [Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
        public async Task<JsonResult> DeleteComment(int commentId)
        {
            try
            {
                await _commentService.DeleteAsync(commentId);
            }
            catch
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }
    }
}