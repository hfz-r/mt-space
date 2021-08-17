using System;
using System.Collections.Generic;
using System.Linq;
using AHAM.Services.Investor.Domain.Exceptions;
using AHAM.Services.Investor.Domain.SeedWork;

namespace AHAM.Services.Investor.Domain.AggregatesModel.FeeRebateAggregate
{
    public class Currency : Enumeration
    {
        public static Currency AUD = new Currency(nameof(AUD), "AUSTRALIAN DOLLAR");
        public static Currency CNY = new Currency(nameof(CNY), "YUAN CHINESE RENMI");
        public static Currency EUR = new Currency(nameof(EUR), "EURO");
        public static Currency GBP = new Currency(nameof(GBP), "BRITISH POUND");
        public static Currency MYR = new Currency(nameof(MYR), "MALAYSIA RINGGIT");
        public static Currency SGD = new Currency(nameof(SGD), "SINGAPORE DOLLAR");
        public static Currency USD = new Currency(nameof(USD), "US DOLLAR");

        public Currency(string code, string name) : base(code, name)
        {
        }

        public static IEnumerable<Currency> List() => new[] {AUD, CNY, EUR, GBP, MYR, SGD, USD};

        public static Currency From(object val)
        {
            var state = List().SingleOrDefault(x => val switch
            {
                string v => (x.Code == v ? x.Code == v : x.Name == v),
                _ => false
            });

            return state ?? throw new DomainException($"Possible values for SystemType: {String.Join(",", List().Select(x => x.Name))}");
        }
    }
}