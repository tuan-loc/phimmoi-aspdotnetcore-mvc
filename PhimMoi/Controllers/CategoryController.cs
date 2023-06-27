using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimMoi.Application.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Models.Category;
using PhimMoi.Models.Movie;

namespace PhimMoi.Controllers
{
    [Route("[controller]")]
    public class CategoryController : Controller
    {
        private const int MOVIES_PER_PAGE = 25;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        private readonly IMovieService _movieService;

        public CategoryController(IMapper mapper, ICategoryService categoryService, IMovieService movieService)
        {
            _mapper = mapper;
            _categoryService = categoryService;
            _movieService = movieService;
        }

        [HttpGet("{value}")]
        public async Task<IActionResult> Index(string? value, int page)
        {
            Category? category = await _categoryService.GetByNameAsync(value ?? "");
            if(category == null)
            {
                return View("/Views/Shared/404.cshtml");
            }

            PagedList<Movie> movies = await _movieService.FindByCategoryIdAsync(category.Id, new PagingParameter(page, MOVIES_PER_PAGE));
            ViewData["RoureValue"] = value;
            CategoryViewModel model = _mapper.Map<CategoryViewModel>(category);
            model.Movies = _mapper.Map<PagedList<MovieViewModel>>(movies);
            return View(model);
        }
    }
}