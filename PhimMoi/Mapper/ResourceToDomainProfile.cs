using AutoMapper;
using PhimMoi.Domain.Parameters;
using PhimMoi.Resources.Movie;

namespace PhimMoi.Mapper
{
    public class ResourceToDomainProfile : Profile
    {
        public ResourceToDomainProfile()
        {
            CreateMap<MovieParameterResource, MovieParameter>();
        }
    }
}