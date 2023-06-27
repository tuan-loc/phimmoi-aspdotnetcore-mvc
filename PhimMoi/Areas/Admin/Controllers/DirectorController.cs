using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimMoi.Application.Interfaces;
using PhimMoi.Areas.Admin.Models.Director;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Models.Director;
using PhimMoi.SharedLibrary.Constants;

namespace PhimMoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class DirectorController : Controller
    {
        private readonly IDirectorService _directorService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private const int DIRECTOR_PER_PAGE = 15;

        public DirectorController(IDirectorService directorService, IMapper mapper, IWebHostEnvironment environment)
        {
            _directorService = directorService;
            _mapper = mapper;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page, string? value = null)
        {
            PagedList<Director> directors = await _directorService.SearchAsync(value, new PagingParameter(page, DIRECTOR_PER_PAGE));
            if(value != null)
            {
                ViewData["value"] = value;
            }
            return View(_mapper.Map<PagedList<DirectorViewModel>>(directors));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateDirectorViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDirectorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể thêm đạo diễn.");
                return View(model);
            }

            Director director = _mapper.Map<Director>(model);
            try
            {
                await _directorService.CreateAsync(director);
                if(model.AvatarFile != null)
                {
                    director.Avatar = "src/img/DirectorAvatars/" + director.Id + ".jpg";
                    await _directorService.UpdateAsync(director.Id, director);
                    var file = Path.Combine(_environment.WebRootPath, "src/img/DirectorAvatars", director.Id + ".jpg");
                    using var fileStream = new FileStream(file, FileMode.Create);
                    await model.AvatarFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(model);
            }

            TempData["success"] = $"Đã thêm đạo diễn {model.Name}.";
            return RedirectToAction("Index", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string directorId)
        {
            var director = await _directorService.GetByIdAsync(directorId);
            if(director == null)
            {
                return View("/Views/Shared/404.cshtml");
            }
            return View(_mapper.Map<EditDirectorViewModel>(director));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string directorId, EditDirectorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể sửa.");
                return View(model);
            }

            Director director = _mapper.Map<Director>(model);
            if(model.AvatarFile != null)
            {
                director.Avatar = "/src/img/DirectorAvatars/" + director.Id + ".jpg";
            }

            try
            {
                await _directorService.UpdateAsync(directorId, director);
                if(model.AvatarFile != null)
                {
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/DirectorAvatars", directorId + ".jpg");
                    using var fileStream = new FileStream(file, FileMode.Create);
                    await model.AvatarFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(model);
            }

            TempData["success"] = "Chỉnh sửa thành công";
            return RedirectToAction("Index", new { area = "Admin" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string directorId)
        {
            try
            {
                await _directorService.DeleteAsync(directorId);
                var file = Path.Combine(_environment.WebRootPath, "src\\img\\DirectorAvatars", directorId + ".jpg");
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return RedirectToAction("Edit", new { area = "Admin", directorId = directorId });
            }

            TempData["success"] = "Xóa thành công.";
            return RedirectToAction("Index", new { area = "Admin" });
        }
    }
}
