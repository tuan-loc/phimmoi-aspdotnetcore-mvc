

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimMoi.Application.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Models.Director;
using PhimMoi.Models.Movie;

namespace PhimMoi.Controllers
{
    [Route("[controller]")]
    public class DirectorController : Controller
    {
        private const int MOVIES_PER_PAGE = 25;
        private readonly IMapper _mapper;
        private readonly IDirectorService _directorService;
        private readonly IMovieService _movieService;

        public DirectorController(IMapper mapper, IDirectorService directorService, IMovieService movieService)
        {
            _mapper = mapper;
            _directorService = directorService;
            _movieService = movieService;
        }

        [HttpGet("{value}")]
        public async Task<IActionResult> Index(string? value, int page)
        {
            Director? director = await _directorService.GetByNameAsync(value ?? "");
            if(director == null)
            {
                return View("/Views/Shared/404.cshtml");
            }

            PagedList<Movie> movies = await _movieService.FindByDirectorIdAsync(director.Id, new PagingParameter(page, MOVIES_PER_PAGE));
            ViewData["RouteValue"] = value;
            DirectorViewModel model = _mapper.Map<DirectorViewModel>(director);
            model.Movies = _mapper.Map<PagedList<MovieViewModel>>(movies);
            return View(model);
        }
    }
}