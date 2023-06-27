using AutoMapper;
using PhimMoi.Domain.Models;
using PhimMoi.Resources.Cast;
using PhimMoi.Resources.Category;
using PhimMoi.Resources.Country;
using PhimMoi.Resources.Director;
using PhimMoi.Resources.Movie;

namespace PhimMoi.Mapper
{
    public class DomainToResourceProfile : Profile
    {
        public DomainToResourceProfile()
        {
            CreateMap<Movie, MovieResource>();
            CreateMap<Movie, MovieDetailResource>()
                .ForMember(des => des.Tags, options => options.MapFrom(src => src.Tags != null ? src.Tags.Select(t => t.TagName) : null));
            CreateMap<Cast, CastResource>();
            CreateMap<Category, CategoryResource>();
            CreateMap<Country, CountryResource>();
            CreateMap<Director, DirectorResource>();
            CreateMap<Video, VideoResource>();
        }
    }
}