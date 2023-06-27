using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimMoi.Application.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Resources.Category;
using PhimMoi.Resources.Movie;

namespace PhimMoi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoriesController(IMovieService movieService, ICategoryService categoryService, IMapper mapper)
        {
            _movieService = movieService;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            IEnumerable<Category> categories = await _categoryService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CategoryResource>>(categories));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            Category? category = await _categoryService.GetByIdAsync(id);
            if(category == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CategoryResource>(category));
        }

        [HttpGet("{id}/movies")]
        public async Task<IActionResult> GetMovies(string id, [FromQuery] PagingParameter pagingParameter)
        {
            PagedList<Movie> movies = await _movieService.FindByCategoryIdAsync(id, pagingParameter);
            return Ok(_mapper.Map<PagedList<MovieResource>>(movies).GetMetaData());
        }
    }
}