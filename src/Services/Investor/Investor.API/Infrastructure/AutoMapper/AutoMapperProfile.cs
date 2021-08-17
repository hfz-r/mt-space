using AHAM.Services.Dtos.Grpc;
using AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Investor.Domain.AggregatesModel.InvestorAggregate;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Inv = AHAM.Services.Investor.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace AHAM.Services.Investor.API.Infrastructure.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<FeeRebate, FeeRebateDTO>()
                .ForMember(d => d.Agent, opt =>
                {
                    opt.MapFrom(src => src.Agent);
                    opt.NullSubstitute(string.Empty);
                })
                .ForMember(d => d.Amc, opt => opt.MapFrom(src => src.AMC))
                .ForMember(d => d.Channel, opt =>
                {
                    opt.MapFrom(src => src.Channel);
                    opt.NullSubstitute(string.Empty);
                })
                .ForMember(d => d.Coa, opt => opt.MapFrom(src => src.COA))
                .ForMember(d => d.Currency, opt => opt.MapFrom(src => src.GetCurrency()))
                .ForMember(d => d.Drcr, opt => opt.MapFrom(src => src.DrCr))
                .ForMember(d => d.Investor, opt => opt.MapFrom(src => src.Investor))
                .ForMember(d => d.Plan, opt =>
                {
                    opt.MapFrom(src => src.Plan);
                    opt.NullSubstitute(string.Empty);
                })
                .ForMember(d => d.SetupBy, opt =>
                {
                    opt.MapFrom(src => src.SetupBy);
                    opt.NullSubstitute(string.Empty);
                })
                .ForMember(d => d.SetupDate, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.SetupDate.ToUniversalTime())))
                .ForMember(d => d.SetupType, opt =>
                {
                    opt.MapFrom(src => src.SetupType);
                    opt.NullSubstitute(string.Empty);
                })
                .ForMember(d => d.Type, opt =>
                {
                    opt.MapFrom(src => src.Type);
                    opt.NullSubstitute(string.Empty);
                });

            CreateMap<Inv, InvestorDTO>()
                .ForMember(d => d.InvestorId, opt => opt.MapFrom(src => src.InvestorId))
                .ForMember(d => d.InvestorName, opt => opt.MapFrom(src => src.InvestorName))
                .ForMember(d => d.Address, opt => opt.MapFrom(src => src.Address))
                .ReverseMap();

            CreateMap<Address, AddressDTO>()
                .ForMember(d => d.Street, opt => opt.MapFrom(src => src.Street ?? ""))
                .ForMember(d => d.City, opt => opt.MapFrom(src => src.City ?? ""))
                .ForMember(d => d.State, opt => opt.MapFrom(src => src.Street ?? ""))
                .ForMember(d => d.ZipCode, opt => opt.MapFrom(src => src.ZipCode ?? ""))
                .ForMember(d => d.Country, opt => opt.MapFrom(src => src.Country ?? ""))
                .ReverseMap();
        }
    }
}