using System.IO;
using AHAM.Services.Commission.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AHAM.Services.Commission.API.Infrastructure.DbContextFactory
{
    //todo: (future use - db context)
    public class CommissionContextDesignTimeFactory : IDesignTimeDbContextFactory<CommissionContext>
    {
        public CommissionContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<CommissionContext>();

            optionsBuilder.UseSqlServer(config["ConnectionString"], o => o.MigrationsAssembly("Commission.API"));

            return new CommissionContext(optionsBuilder.Options);
        }
    }
}