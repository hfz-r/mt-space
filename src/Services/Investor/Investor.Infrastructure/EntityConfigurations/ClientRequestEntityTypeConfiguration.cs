using AHAM.Services.Investor.Infrastructure.Idempotent;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AHAM.Services.Investor.Infrastructure.EntityConfigurations
{
    public class ClientRequestEntityTypeConfiguration : IEntityTypeConfiguration<ClientRequest>
    {
        public void Configure(EntityTypeBuilder<ClientRequest> builder)
        {
            builder.ToTable("Requests", InvestorContext.DefaultSchema);
            builder.HasKey(cr => cr.Id);
            builder.Property(cr => cr.Name).IsRequired();
            builder.Property(cr => cr.Time).IsRequired();
        }
    }
}