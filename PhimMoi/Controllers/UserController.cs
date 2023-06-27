using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimMoi.Application.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Models;
using PhimMoi.Models.User;

namespace PhimMoi.Controllers
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IEmailSender emailSender, IMapper mapper)
        {
            _userService = userService;
            _emailSender = emailSender;
            _mapper = mapper;
        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> Index(string id)
        {
            User? user = await _userService.GetUserWithLikedMovies(id);

            if(user == null)
            {
                return View("/Views/Shared/404.cshtml");
            }

            return View(_mapper.Map<UserViewModel>(user));
        }

        [HttpPost("report")]
        public async Task<JsonResult> Report(ReportModel model)
        {
            _emailSender.SendEmailAsync("thuyto@gmail.com", "Báo lỗi", $"Người dùng có Email {model.Email} báo lỗi : {model.Content}");

            return Json(new
            {
                success = true
            });
        }
    }
}