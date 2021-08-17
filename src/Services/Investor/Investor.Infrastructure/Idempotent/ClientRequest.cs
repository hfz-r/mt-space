using System;

namespace AHAM.Services.Investor.Infrastructure.Idempotent
{
    public class ClientRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
    }
}