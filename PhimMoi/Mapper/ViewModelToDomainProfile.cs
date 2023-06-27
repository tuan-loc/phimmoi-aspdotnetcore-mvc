using AutoMapper;
using PhimMoi.Areas.Admin.Models.Cast;
using PhimMoi.Areas.Admin.Models.Category;
using PhimMoi.Areas.Admin.Models.Country;
using PhimMoi.Areas.Admin.Models.Director;
using PhimMoi.Areas.Admin.Models.Movie;
using PhimMoi.Areas.Identity.Models;
using PhimMoi.Domain.Models;
using PhimMoi.Models.Comment;
using PhimMoi.Models.Movie;
using PhimMoi.Models.Tag;
using PhimMoi.Models.User;
using PhimMoi.Models.Video;

namespace PhimMoi.Mapper
{
    public class ViewModelToDomainProfile : Profile
    {
        public ViewModelToDomainProfile()
        {
            CreateMap<MovieViewModel, Movie>();
            CreateMap<CreateMovieViewModel, Movie>()
                .ForMember(des => des.Casts, options => options.Ignore())
                .ForMember(des => des.Categories, options => options.Ignore())
                .ForMember(des => des.Directors, options => options.Ignore())
                .ForMember(des => des.Country, options => options.Ignore())
                .ForMember(des => des.Tags, options => options.Ignore())
                .ForMember(des => des.Videos, options => options.Ignore());

            CreateMap<EditMovieViewModel, Movie>()
                .ForMember(des => des.Casts, options => options.Ignore())
                .ForMember(des => des.Categories, options => options.Ignore())
                .ForMember(des => des.Directors, options => options.Ignore())
                .ForMember(des => des.Country, options => options.Ignore())
                .ForMember(des => des.Tags, options => options.Ignore())
                .ForMember(des => des.Videos, options => options.Ignore());

            CreateMap<CreateCastViewModel, Cast>();
            CreateMap<EditCastViewModel, Cast>();

            CreateMap<EditDirectorViewModel, Director>();
            CreateMap<CreateDirectorViewModel, Director>();

            CreateMap<EditCategoryViewModel, Category>();
            CreateMap<CreateCategoryViewModel, Category>();

            CreateMap<EditCountryViewModel, Country>();
            CreateMap<CreateCategoryViewModel, Country>();

            CreateMap<CommentViewModel, Comment>();

            CreateMap<EditUserViewModel, User>();
            CreateMap<UserViewModel, User>();

            CreateMap<VideoViewModel, Video>()
                .ForMember(des => des.Movie, options => options.Ignore());

            CreateMap<TagViewModel, Tag>()
                .ForMember(des => des.Movie, options => options.Ignore());
        }
    }
}