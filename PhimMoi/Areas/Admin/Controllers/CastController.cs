using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimMoi.Application.Interfaces;
using PhimMoi.Areas.Admin.Models.Cast;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Models.Cast;
using PhimMoi.SharedLibrary.Constants;

namespace PhimMoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class CastController : Controller
    {
        private readonly ICastService _castService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private const int CASTS_PER_PAGE = 15;

        public CastController(ICastService castService, IMapper mapper, IWebHostEnvironment environment)
        {
            _castService = castService;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page, string? value = null)
        {
            PagedList<Cast> casts = await _castService.SearchAsync(value, new PagingParameter(page, CASTS_PER_PAGE));

            if(value != null)
            {
                ViewData["value"] = value;
            }

            return View(_mapper.Map<PagedList<CastViewModel>>(casts));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCastViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCastViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể thêm diễn viên.");
                return View(model);
            }

            Cast cast = _mapper.Map<Cast>(model);

            try
            {
                await _castService.CreateAsync(cast);
                if(model.AvatarFile != null)
                {
                    cast.Avatar = "/src/img/CastAvatars/" + cast.Id + ".jpg";
                    await _castService.UpdateAsync(cast.Id, cast);
                    var file = Path.Combine(_environment.WebRootPath, "src/img/CastAvatars", cast.Id + ".jpg");
                    using var fileStream = new FileStream(file, FileMode.Create);
                    await model.AvatarFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(model);
            }

            TempData["success"] = $"Đã thêm diễn viên {model.Name}.";
            return RedirectToAction("Index", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string castId)
        {
            var cast = await _castService.GetByIdAsync(castId);

            if(cast == null)
            {
                return View("/Views/Shared/404.cshtml");
            }

            return View(_mapper.Map<EditCastViewModel>(cast));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string castId, EditCastViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể sửa.");
                return View(model);
            }

            Cast cast = _mapper.Map<Cast>(model);

            if(model.AvatarFile != null)
            {
                cast.Avatar = "/src/img/CastAvatars/" + cast.Id + ".jpg";
            }

            try
            {
                await _castService.UpdateAsync(castId, cast);
                if(model.AvatarFile != null)
                {
                    var file = Path.Combine(_environment.WebRootPath, "src/img/CastAvatars", castId + ".jpg");
                    using var fileStream = new FileStream(file, FileMode.Create);
                    await model.AvatarFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(model);
            }

            TempData["success"] = "Chỉnh sửa thành công.";
            return RedirectToAction("Index", new { area = "Admin" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string castId)
        {
            try
            {
                await _castService.DeleteAsync(castId);
                var file = Path.Combine(_environment.WebRootPath, "src\\img\\CastAvatars", castId + ".jpg");
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return RedirectToAction("Edit", new { area = "Admin", castId = castId });
            }

            TempData["success"] = "Xóa thành công.";
            return RedirectToAction("Index", new { area = "Admin" });
        }
    }
}
