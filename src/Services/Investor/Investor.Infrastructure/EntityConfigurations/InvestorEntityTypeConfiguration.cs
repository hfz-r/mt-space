using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Inv = AHAM.Services.Investor.Domain.AggregatesModel.InvestorAggregate.Investor;

namespace AHAM.Services.Investor.Infrastructure.EntityConfigurations
{
    public class InvestorEntityTypeConfiguration : IEntityTypeConfiguration<Inv>
    {
        public void Configure(EntityTypeBuilder<Inv> builder)
        {
            builder.ToTable("ISS_INVESTOR", InvestorContext.DefaultSchema);
            builder.Ignore(i => i.Id);
            builder.Ignore(i => i.DomainEvents);

            builder.OwnsOne(i => i.Address, a =>
            {
                a.WithOwner();
                a.Property(d => d.Street).HasColumnName("ADDRESS_FAPP");
                a.Property(d => d.City).HasColumnName("CITY_FAPP");
                a.Property(d => d.ZipCode).HasColumnName("PINCODE_FAPP");
                a.Property(d => d.State).HasColumnName("STATE_FAPP");
                a.Property(d => d.Country).HasColumnName("COUNTRY_FAPP");
            });

            #region Properties

            builder.Property(i => i.InvestorId)
                .IsRequired()
                .HasColumnName("INVESTOR_ID");
            builder.Property(i => i.InvestorName)
                .IsRequired()
                .HasColumnName("NAME_FAPP");

            #endregion
        }
    }
}