using System;
using AHAM.Services.Commission.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Commission.Domain.AggregatesModel.InvestorAggregate;
using AHAM.Services.Commission.Dtos.Grpc;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Inv = AHAM.Services.Commission.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace AHAM.Services.Commission.API.Infrastructure.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<FeeRebate, FeeRebateDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Agent, opt =>
                {
                    opt.MapFrom(src => src.Agent);
                    opt.NullSubstitute(string.Empty);
                })
                .ForMember(d => d.Amc, opt => opt.MapFrom(src => src.Amc))
                .ForMember(d => d.Channel, opt =>
                {
                    opt.MapFrom(src => src.Channel);
                    opt.NullSubstitute(string.Empty);
                })
                .ForMember(d => d.Coa, opt => opt.MapFrom(src => src.Coa))
                .ForMember(d => d.Currency, opt => opt.MapFrom(src => src.GetCurrency()))
                .ForMember(d => d.Drcr, opt => opt.MapFrom(src => src.DrCr))
                .ForMember(d => d.InvestorId, opt => opt.MapFrom(src => src.GetInvestor().id))
                .ForMember(d => d.InvestorName, opt => opt.MapFrom(src => src.GetInvestor().name))
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
                .ForMember(d => d.SetupDate, opt =>
                {
                    opt.MapFrom(src => Timestamp.FromDateTime(src.SetupDate.ToUniversalTime()));
                    opt.NullSubstitute(Timestamp.FromDateTime(DateTime.UtcNow));
                })
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

            CreateMap<FeeRebateDTO, FeeRebate>()
                .ForMember(d => d.DomainEvents, opt => opt.Ignore())
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ConvertUsing(src => new FeeRebate(src.Amc, src.Agent, src.Channel, src.Coa, src.Drcr, src.Plan, src.SetupBy, src.SetupType, src.SetupDate.ToDateTime(), src.Type, src.Currency, src.InvestorId));

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