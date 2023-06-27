using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhimMoi.Application.Interfaces;
using PhimMoi.Areas.Admin.Models.Movie;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Models.Movie;
using PhimMoi.SharedLibrary.Constants;
using System.Linq.Expressions;

namespace PhimMoi.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{RoleConstant.ADMIN}, {RoleConstant.THUY_TO}")]
    public class MovieController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMovieService _movieService;
        private readonly ICountryService _countryService;
        private readonly IWebHostEnvironment _environment;
        private const int MOVIES_PER_PAGE = 15;

        public MovieController(IMapper mapper, IMovieService movieService, ICountryService countryService, IWebHostEnvironment environment)
        {
            _mapper = mapper;
            _movieService = movieService;
            _countryService = countryService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page, string? value = null)
        {
            PagedList<Movie> movies = await _movieService.SearchAsync(new MovieParameter(page, MOVIES_PER_PAGE)
            {
                Value = value,
                OrderBy = "CreatedDate_desc"
            });

            if(value != null)
            {
                ViewData["value"] = value;
            }

            return View(_mapper.Map<PagedList<MovieViewModel>>(movies));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var countries = await _countryService.GetAllAsync();
            ViewData["countries"] = new SelectList(countries, "Id", "Name");
            return View(new CreateMovieViewModel());
        }

        [HttpPost]
        public async Task<JsonResult> Create(CreateMovieViewModel model)
        {
            Movie movie = _mapper.Map<Movie>(model);
            try
            {
                await _movieService.CreateAsync(movie, model.Casts, model.Directors, model.Categories, model.Country, model.Tags, model.Videos);
                if (model.ImageFile != null)
                {
                    movie.Image = "/src/img/MovieImages/" + movie.Id + ".jpg";
                    await _movieService.UpdateAsync(movie.Id, movie);
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/MovieImages", movie.Id + ".jpg");
                    using FileStream fileStream = new(file, FileMode.Create);
                    await model.ImageFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
            }

            TempData["success"] = $"Đã thêm phim {model.Name}.";
            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string movieId)
        {
            var movie = await _movieService.GetByIdAsync(movieId, new Expression<Func<Movie, object?>>[]
            {
                m => m.Casts,
                m => m.Categories,
                m => m.Country,
                m => m.Directors,
                m => m.Tags,
                m => m.Videos
            });

            if(movie == null)
            {
                return View("/Views/Shared/404.cshtml");
            }

            var countries = await _countryService.GetAllAsync();
            ViewData["countries"] = new SelectList(countries, "Id", "Name");
            return View(EditMovieViewModel.FromMovie(movie));
        }

        [HttpPost]
        public async Task<JsonResult> Edit(string movieId, EditMovieViewModel model)
        {
            if(model == null)
            {
                return Json(new { success = false, error = "Lỗi, không tìm thấy model :(" });
            }

            Movie movie = _mapper.Map<Movie>(model);
            if(model.ImageFile != null)
            {
                movie.Image = "/src/img/MovieImages/" + movieId + ".jpg";
            }

            try
            {
                await _movieService.UpdateAsync(movieId, movie, model.Casts, model.Directors, model.Categories, model.Country, model.Tags, model.Videos);
                if(model.ImageFile != null)
                {
                    var file = Path.Combine(_environment.ContentRootPath, "wwwroot/src/img/MovieImages", movieId, ".jpg");
                    using var fileStream = new FileStream(file, FileMode.Create);
                    await model.ImageFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message });
            }

            TempData["success"] = "Chỉnh sửa thành công.";
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string movieId)
        {
            try
            {
                await _movieService.DeleteAsync(movieId);
                var file = Path.Combine(_environment.WebRootPath, "src\\img\\MovieImages", movieId + ".jpg");
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
            catch (Exception e)
            {
                TempData["status"] = "Lỗi, " + e.Message;
                return RedirectToAction("Edit", new { area = "Admin", movieId = movieId });
            }

            TempData["status"] = "Xóa thành công.";
            return RedirectToAction("Index", new { area = "Admin" });
        }
    }
}
