using AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AHAM.Services.Investor.Infrastructure.EntityConfigurations
{
    public class CurrencyEntityTypeConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currencies", InvestorContext.DefaultSchema);
            builder.HasKey(c => c.Code);

            builder.Property(c => c.Code).IsRequired();

            builder.Property(o => o.Name)
                .ValueGeneratedNever()
                .IsRequired();
        }
    }
}