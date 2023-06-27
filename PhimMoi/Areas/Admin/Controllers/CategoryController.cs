using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhimMoi.Application.Interfaces;
using PhimMoi.Areas.Admin.Models.Category;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Models.Category;
using PhimMoi.SharedLibrary.Constants;

namespace PhimMoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private const int CATE_PER_PAGE = 15;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page, string? value = null)
        {
            PagedList<Category> categories = await _categoryService.SearchAsync(value, new PagingParameter(page, CATE_PER_PAGE));
            if(value != null)
            {
                ViewData["value"] = value;
            }
            return View(_mapper.Map<PagedList<CategoryViewModel>>(categories));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCategoryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể thêm.");
                return View();
            }

            Category category = _mapper.Map<Category>(model);
            try
            {
                await _categoryService.CreateAsync(category);
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return View(model);
            }

            TempData["success"] = $"Đã thêm thể loại {model.Name}.";
            return RedirectToAction("Index", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string cateId)
        {
            var category = await _categoryService.GetByIdAsync(cateId);
            if(category == null)
            {
                return View("/Views/Shared/404.cshtml");
            }

            return View(_mapper.Map<EditCategoryViewModel>(category));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string cateId, EditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Lỗi không thể sửa.");
                return View(model);
            }

            Category category = _mapper.Map<Category>(model);
            try
            {
                await _categoryService.UpdateAsync(cateId, category);
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi " + e.Message;
                return View(model);
            }

            TempData["success"] = "Chỉnh sửa thành công.";
            return RedirectToAction("Index", new { area = "Admin" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string cateId)
        {
            try
            {
                await _categoryService.DeleteAsync(cateId);
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return RedirectToAction("Edit", new { area = "Admin", cateId = cateId });
            }

            TempData["success"] = "Xóa thành công.";
            return RedirectToAction("Index", new { area = "Admin" });
        }
    }
}
