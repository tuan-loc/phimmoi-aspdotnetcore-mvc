using AutoMapper;
using PhimMoi.Areas.Admin.Models.Cast;
using PhimMoi.Areas.Admin.Models.Category;
using PhimMoi.Areas.Admin.Models.Country;
using PhimMoi.Areas.Admin.Models.Director;
using PhimMoi.Domain.Models;
using PhimMoi.Domain.PagingModel;
using PhimMoi.Models.Cast;
using PhimMoi.Models.Category;
using PhimMoi.Models.Comment;
using PhimMoi.Models.Country;
using PhimMoi.Models.Director;
using PhimMoi.Models.Movie;
using PhimMoi.Models.Tag;
using PhimMoi.Models.User;
using PhimMoi.Models.Video;

namespace PhimMoi.Mapper
{
    public class DomainToViewModelProfile : Profile
    {
        public DomainToViewModelProfile()
        {
            CreateMap<Movie, MovieViewModel>();

            CreateMap<Cast, EditCastViewModel>();
            CreateMap<Cast, CastViewModel>();

            CreateMap<Director, EditDirectorViewModel>();
            CreateMap<Director, DirectorViewModel>();

            CreateMap<Category, EditCategoryViewModel>();
            CreateMap<Category, CategoryViewModel>();

            CreateMap<Country, EditCountryViewModel>();
            CreateMap<Country, CountryViewModel>();

            CreateMap<Comment, CommentViewModel>();
            CreateMap<User, UserViewModel>();

            CreateMap<Video, VideoViewModel>()
                .ForMember(des => des.MovieId, options => options.MapFrom(src => src.Movie.Id));

            CreateMap<Tag, TagViewModel>()
                .ForMember(des => des.MovieId, options => options.MapFrom(src => src.Movie.Id));

            CreateMap(typeof(PagedList<>), typeof(PagedList<>)).ConvertUsing(typeof(Converter<,>));
        }
    }

    public class Converter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
    {
        public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination> destination, ResolutionContext context) => new PagedList<TDestination>(context.Mapper.Map<List<TSource>, List<TDestination>>(source), source.CurrentPage, source.PageSize, source.TotalItems);
    }
}