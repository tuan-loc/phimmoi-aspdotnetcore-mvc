using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhimMoi.Application.Interfaces;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Domain.Parameters;
using PhimMoi.Models;
using PhimMoi.Resources.Country;
using PhimMoi.Resources.Movie;

namespace PhimMoi.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesControlller : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly ICountryService _countryService;
        private readonly IMapper _mapper;

        public CountriesControlller(IMovieService movieService, ICountryService countryService, IMapper mapper)
        {
            _movieService = movieService;
            _countryService = countryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            IEnumerable<Country> countries = await _countryService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<CountryResource>>(countries));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            Country? country = await _countryService.GetByIdAsync(id);
            if(country == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CountryResource>(country));
        }

        [HttpGet("{id}/movies")]
        public async Task<IActionResult> GetMovies(string id, [FromQuery] PagingParameter pagingParameter)
        {
            PagedList<Movie> movies = await _movieService.FindByCountryIdAsync(id, pagingParameter);
            return Ok(_mapper.Map<PagedList<MovieResource>>(movies).GetMetaData());
        }
    }
}