using AutoMapper;
using BZGames.Application.DTOs.Auth;
using BZGames.Domain.Entities;

namespace BZGames.Application.Common.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserInfo>().ReverseMap();
            CreateMap<UserRegistration, User>();
        }
    }
}
