using AHAM.Services.Commission.Domain.AggregatesModel.FeeRebateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AHAM.Services.Commission.Infrastructure.EntityConfigurations
{
    public class CurrencyEntityTypeConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            builder.ToTable("Currencies", CommissionContext.DefaultSchema);
            builder.HasKey(c => c.Code);

            builder.Property(c => c.Code).IsRequired();

            builder.Property(o => o.Name)
                .ValueGeneratedNever()
                .IsRequired();
        }
    }
}