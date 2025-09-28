using AutoMapper;
using Clinic.API.API.Dtos.AuthDtos.RefreshTokenDtos;
using Clinic.API.API.Dtos.DoctorDtos;
using Clinic.API.Domain.Entities;
using System.Security.Cryptography;

namespace Clinic.API.API.Mappings
{
    public class RefreshTokenMappingConfig
    {
        public void Configure(Profile profile)
        {
            profile.CreateMap<RefreshToken, RefreshTokenDto>();
            profile.CreateMap<CreateNewRefreshTokenDto, RefreshToken>()
                .ForMember(dest => dest.Token, opt => opt.MapFrom(_ => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))))
                .ForMember(dest => dest.Expires, opt => opt.MapFrom(_ => DateTime.UtcNow.AddDays(7)));
            profile.CreateMap<RevokeRefreshTokenDto, RefreshToken>()
                .ForMember(dest => dest.Revoked, opt => opt.MapFrom(_ => DateTime.UtcNow));

        }
    }
}
