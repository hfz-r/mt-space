using System;
using AHAM.Services.Commission.API.Infrastructure.AutoMapper;
using AutoMapper;

namespace Commission.UnitTests
{
    public class AutoMapperFixture : IDisposable
    {
        public IMapper Mapper { get; private set; }

        public AutoMapperFixture()
        {
            ConfigureAutoMapper();
        }

        public void Dispose()
        {
        }

        private void ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(c => { c.AddProfile(typeof(AutoMapperProfile)); });
            Mapper = config.CreateMapper();
            config.AssertConfigurationIsValid();
        }
    }
}
