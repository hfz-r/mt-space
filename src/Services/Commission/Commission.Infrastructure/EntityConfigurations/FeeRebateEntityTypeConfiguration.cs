using AHAM.Services.Commission.Domain.AggregatesModel.FeeRebateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AHAM.Services.Commission.Infrastructure.EntityConfigurations
{
    public class FeeRebateEntityTypeConfiguration : IEntityTypeConfiguration<FeeRebate>
    {
        public void Configure(EntityTypeBuilder<FeeRebate> builder)
        {
            builder.ToTable("COARebateFeeMgmt", CommissionContext.DefaultSchema);
            builder.HasKey(f => f.Id);
            builder.Ignore(f => f.DomainEvents);

            #region Properties

            builder.Property(b => b.Amc).IsRequired();
            builder.Property(b => b.Coa).IsRequired();
            builder.Property(b => b.DrCr).IsRequired();
            builder.Property(b => b.Type).IsRequired();
            builder.Property(b => b.SetupDate).IsRequired();
            builder.Property(b => b.Channel).IsRequired(false);
            builder.Property(b => b.Agent).IsRequired(false);
            builder.Property(b => b.Plan).IsRequired(false);
            builder.Property(b => b.SetupType).IsRequired(false);
            builder.Property(b => b.SetupBy).IsRequired(false);

            builder
                .Property<string>("_investorId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("InvestorID")
                .IsRequired();

            builder
                .Property<string>("_currencyId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("Currency")
                .IsRequired();

            #endregion

            #region Entity relationship

            //var navigation = builder.Metadata.FindNavigation(nameof(SomeCollectionProperty));
            //navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne("_investor")
                .WithMany()
                .HasForeignKey("_investorId");

            builder.HasOne<Currency>()
                .WithMany()
                .HasForeignKey("_currencyId");

            #endregion
        }
    }
}