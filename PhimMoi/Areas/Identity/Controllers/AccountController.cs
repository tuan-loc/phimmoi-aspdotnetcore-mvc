using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using PhimMoi.Application.Interfaces;
using PhimMoi.Areas.Identity.Models;
using PhimMoi.Domain.Models;
using PhimMoi.Models.Movie;
using PhimMoi.Models.User;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace PhimMoi.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailSender _emailSender;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AccountController(IWebHostEnvironment environment, IEmailSender emailSender, IUserService userService, IMapper mapper)
        {
            _environment = environment;
            _emailSender = emailSender;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("/account")]
        public async Task<IActionResult> Index()
        {
            User? user = await _userService.GetByClaims(User);

            if(user == null)
            {
                return NotFound("Không tìm thấy user :()");
            }
            return View(_mapper.Map<UserViewModel>(user));
        }

        [HttpGet("/account/liked-movies")]
        public async Task<IActionResult> LikedMovies()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User? user = await _userService.GetUserWithLikedMovies(userId);

            if(user == null)
            {
                return NotFound("Không tìm thấy user :((");
            }

            user.LikedMovies ??= new List<Movie>();
            return View(_mapper.Map<List<MovieViewModel>>(user.LikedMovies));
        }

        [HttpGet("/account/change-password")]
        public IActionResult ChangePassword() => View();

        [HttpPost("/account/change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            User? user = await _userService.GetByClaims(User);
            if(user == null)
            {
                return NotFound("Unable to load user.");
            }

            var result = await _userService.ChangePasswordAsync(user.Id, model.OldPassword, model.NewPassword);
            if (result.Success)
            {
                TempData["success"] = "Thay đổi mật khẩu thành công!";
                return RedirectToAction("Index");
            }

            if (result.Errors == null) return View();

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            TempData["error"] = "Thay đổi mật khẩu thất bại!";
            return View();
        }

        [HttpGet]
        [Route("/account/email")]
        public async Task<IActionResult> Email()
        {
            var user = await _userService.GetByClaims(User);
            if(user == null)
            {
                return NotFound($"Unable to load user.");
            }

            return View(new ManageEmailModel
            {
                IsEmailConfirmed = user.EmailConfirmed,
                Email = user.Email
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ManageEmailModel model)
        {
            var user = await _userService.GetByClaims(User);
            if(user == null){
                return NotFound($"Unable to load user.");
            }

            var result = await _userService.ChangeEmailAsync(user.Id, model.NewEmail);

            if (!result.Success)
            {
                TempData["status"] = $"Lỗi, {result.Errors?[0]}";
                return RedirectToAction("Email");
            }

            if (result.Success)
            {
                string token = await _userService.GenerateEmailConfirmationTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                string? callbackUrl = Url.Action("ConfirmEmail", "Authentication", new
                {
                    area = "Identity",
                    token = token,
                    userId = user.Id
                },
                protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Xác thực Email", $"Hello người mới!, click vào <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>đây</a> để xác thực Email của bạn nhé :)");

                TempData["success"] = "Đã thay đổi Email";
                TempData["status"] = "Đã thay đổi Email, hãy kiểm tra hòm thư Email để xác thực.";
            }

            return RedirectToAction("Email");
        }

        [HttpPost]
        public async Task<JsonResult> SendEmailVerify()
        {
            var user = await _userService.GetByClaims(User);

            if(user == null)
            {
                return Json(new { success = false });
            }

            string token = await _userService.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            string callbackUrl = Url.Action("ConfirmEmail", "Authentication", new
            {
                area = "Identity",
                token = token,
                userId = user.Id
            },
            protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Xác thực Email", $"Hello!, click vào <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>đây</a> để xác thực Email của bạn nhé :)");
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> EditInformation(EditUserViewModel? model)
        {
            var user = await _userService.GetByClaims(User);

            if(model == null || user == null)
            {
                return Json(new { success = false });
            }

            User userModel = _mapper.Map<User>(model);
            if(model.AvatarFile != null)
            {
                if(!model.AvatarFile.FileName.EndsWith(".png") && !model.AvatarFile.FileName.EndsWith(".jpg"))
                {
                    return Json(new { success = false, error = "Định dạng ảnh phải là .png, .jpg" });
                }

                var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/UserAvatars", user.Id + ".jpg");
                using(var fileStream = new FileStream(file, FileMode.Create))
                {
                    await model.AvatarFile.CopyToAsync(fileStream);
                }

                userModel.Avatar = "/src/img/UserAvatars/" + user.Id + ".jpg";
            }

            var result = await _userService.UpdateAsync(user.Id, userModel);

            if (!result.Success)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                displayname = userModel.DisplayName,
                phone = userModel.PhoneNumber,
                favoritemovie = userModel.FavoriteMovie,
                hobby = userModel.Hobby,
                avatar = userModel.Avatar ?? ""
            });
        }
    }
}
