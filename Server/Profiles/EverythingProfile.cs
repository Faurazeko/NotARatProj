using AutoMapper;
using Server.Dtos;
using Server.Models;
using System.Net;
using NarLib;

namespace Server.Profiles
{
	public class EverythingProfile : Profile
	{
		public EverythingProfile()
		{
			// source => target

			CreateMap<ClientRegisterDto, Client>()
				.ForMember(dest => dest.LocalIp, opt => opt.MapFrom(src => IPAddress.Parse(src.LocalIp)))
				.ForMember(dest => dest.PublicIp, opt => opt.MapFrom(src => IPAddress.Parse(src.PublicIp)));

			CreateMap<Client, ClientMngDto>()
				.ForMember(dest => dest.LocalIp, opt => opt.MapFrom(src => src.LocalIp.ToString()))
				.ForMember(dest => dest.PublicIp, opt => opt.MapFrom(src => src.PublicIp.ToString()));

			CreateMap<ControlRequest, ControlRequestDto>();
		}
	}
}
