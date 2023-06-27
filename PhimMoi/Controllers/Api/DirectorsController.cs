using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimMoi.Application.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Resources.Director;
using PhimMoi.Resources.Movie;

namespace PhimMoi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorsController : ControllerBase
    {
        private readonly IMovieService _moviceService;
        private readonly IDirectorService _directorService;
        private readonly IMapper _mapper;

        public DirectorsController(IMovieService moviceService, IDirectorService directorService, IMapper mapper)
        {
            _moviceService = moviceService;
            _directorService = directorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery(Name = "q")] string? value, [FromQuery]PagingParameter pagingParameter)
        {
            PagedList<Director> directors = await _directorService.SearchAsync(value, pagingParameter);
            return Ok(_mapper.Map<PagedList<DirectorResource>>(directors).GetMetaData());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            Director? director = await _directorService.GetByIdAsync(id);
            if(director == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DirectorResource>(director));
        }

        [HttpGet("{id}/movies")]
        public async Task<IActionResult> GetMovies(string id, [FromQuery]PagingParameter pagingParameter)
        {
            PagedList<Movie> movies = await _moviceService.FindByDirectorIdAsync(id, pagingParameter);
            return Ok(_mapper.Map<PagedList<MovieResource>>(movies).GetMetaData());
        }
    }
}