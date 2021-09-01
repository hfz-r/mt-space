using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHAM.Services.Dtos.Grpc;
using AHAM.Services.Investor.API.Application.IntegrationEvents;
using AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate;
using AHAM.Services.Investor.Infrastructure.Caching;
using AHAM.Services.Investor.Infrastructure.Idempotent;
using AHAM.Services.Investor.Infrastructure.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AHAM.Services.Investor.API.Application.Commands
{
    public class CreateRebateCommandHandler : IRequestHandler<CreateRebateCommand, bool>
    {
        private readonly IUnitOfWork _worker;
        private readonly IMapper _mapper;
        private readonly IInvestorIntegrationEventService _integrationEvent;
        private readonly ILogger<CreateRebateCommandHandler> _logger;

        public CreateRebateCommandHandler(IUnitOfWork worker, IMapper mapper, IInvestorIntegrationEventService integrationEvent, ILogger<CreateRebateCommandHandler> logger)
        {
            _worker = worker;
            _mapper = mapper;
            _integrationEvent = integrationEvent;
            _logger = logger;
        }

        public async Task<bool> Handle(CreateRebateCommand command, CancellationToken cancellationToken)
        {
            var repository = _worker.GetRepositoryAsync<FeeRebate>();

            try
            {
                var o = await repository.GetListAsync(
                    q => q.Investor.InvestorId == command.InvestorId,
                    include: q => q.Include(r => r.Investor),
                    cacheKey: cache => cache.PrepareKeyForDefaultCache(Keys<FeeRebate>.ByIdCacheKey, command.InvestorId),
                    disableTracking: true,
                    cancellationToken: cancellationToken
                );

                if (o.Count > 0)
                {
                    var l = new List<FeeRebate>();
                    foreach (var dto in command.List)
                    {
                        var r = await repository.FindAsync(dto.Id);
                        if (dto.Type == "deleted")
                        {
                            repository.Delete(r);
                            continue;
                        }
                        if (r != null)
                        {
                            if (r.Coa != dto.Coa) r.SetCoa(dto.Coa);
                            if (r.SetupDate != dto.SetupDate.ToDateTime()) r.SetSetupDate(dto.SetupDate.ToDateTime());
                            if (r.DrCr != dto.Drcr) r.SetDrCr(dto.Drcr);
                            if (r.GetCurrency() != dto.Currency) r.SetCurrency(dto.Currency);
                        }
                        else r = _mapper.Map<FeeRebateDTO, FeeRebate>(dto);

                        l.Add(r);
                    }

                    _logger.LogInformation("----- Updating FeeRebate - FeeRebate: {@FeeRebate}", "");
                    repository.Update(l);
                }
                else
                {
                    //add
                    var lfe = command.List.Select(dto => _mapper.Map<FeeRebateDTO, FeeRebate>(dto));

                    _logger.LogInformation("----- Creating FeeRebate - FeeRebate: {@FeeRebate}", "");
                    await repository.AddAsync(lfe, cancellationToken);
                }

                await repository.RemoveByPrefixAsync(Keys<FeeRebate>.Prefix);

                //publish @domain-event
                return await _worker.SaveEntitiesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

    public class CreateOrderIdentifiedCommandHandler : IdentifiedCommandHandler<CreateRebateCommand, bool>
    {
        public CreateOrderIdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<CreateRebateCommand, bool>> logger) : base(mediator, requestManager,
            logger)
        {
        }

        protected override bool CreateResultForDuplicateRequest() => true;
    }
}