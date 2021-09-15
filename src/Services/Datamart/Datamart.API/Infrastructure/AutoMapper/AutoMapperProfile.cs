using AHAM.Services.Datamart.API.Model;
using AHAM.Services.Datamart.API.Model.Envelopes;
using AHAM.Services.Datamart.Dtos.Grpc;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace AHAM.Services.Datamart.API.Infrastructure.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<FeeStructure, CimbSplitDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.EffectiveFrom, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.EffectiveFrom.ToUniversalTime())))
                .ForMember(d => d.ProductCode, opt => opt.MapFrom(src => src.Product.Code))
                .ForMember(d => d.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(d => d.AgentId, opt => opt.MapFrom(src => src.Agent.Id))
                .ForMember(d => d.AgentName, opt => opt.MapFrom(src => src.Agent.Name))
                .ForMember(d => d.InvestorId, opt => opt.MapFrom(src => src.Account.Id))
                .ForMember(d => d.InvestorName, opt => opt.MapFrom(src => src.Account.Name))
                .ForMember(d => d.Plan, opt => opt.MapFrom(src => src.Plan))
                .ForMember(d => d.FeeType, opt => opt.MapFrom(src => src.FeeType))
                .ForMember(d => d.Basis, opt => opt.MapFrom(src => src.Basis))
                .ForMember(d => d.AmountFrom, opt => opt.MapFrom(src => src.AmountFrom))
                .ForMember(d => d.AmountTo, opt => opt.MapFrom(src => src.AmountTo))
                .ForMember(d => d.Rate, opt => opt.MapFrom(src => src.Rate));

            CreateMap<CimbSplitDTO, CimbCommand>()
                .ConvertUsing(src => new CimbCommand(src.Id, src.EffectiveFrom.ToDateTime().ToString("u"), src.ProductCode, src.AgentId, src.InvestorId, src.Plan));
        }
    }
}