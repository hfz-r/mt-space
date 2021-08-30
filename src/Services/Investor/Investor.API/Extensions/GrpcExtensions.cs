using System;
using System.Linq;
using Grpc.Core;

namespace AHAM.Services.Investor.API.Extensions
{
    public static class GrpcExtensions
    {
        public static Guid GetRequestIdHeader(this ServerCallContext context)
        {
            var header = context.RequestHeaders.FirstOrDefault(e => e.Key == "x-requestid");
            Guid.TryParse(header?.Value, out var guid);
            
            return guid != Guid.Empty ? guid : Guid.NewGuid();
        }
    }
}