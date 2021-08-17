using System.IO;
using AHAM.Services.Investor.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AHAM.Services.Investor.API.Infrastructure.DbContextFactory
{
    //todo: (future use - db context)
    public class InvestorContextDesignTimeFactory : IDesignTimeDbContextFactory<InvestorContext>
    {
        public InvestorContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<InvestorContext>();

            optionsBuilder.UseSqlServer(config["ConnectionString"], o => o.MigrationsAssembly("Investor.API"));

            return new InvestorContext(optionsBuilder.Options);
        }
    }
}