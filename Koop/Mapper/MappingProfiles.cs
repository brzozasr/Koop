using AutoMapper;
using Koop.Models;
using Koop.Models.Auth;
using Koop.Models.RepositoryModels;

namespace Koop.Mapper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<UserSignUp, User>();
            CreateMap<OrderView, CoopOrder>();
        }
    }
}