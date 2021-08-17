using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace Investor.UnitTests.Helpers
{
    /// <summary>
    /// Test doubles for client-side call objects.
    /// </summary>
    public static class TestCalls
    {
        public static AsyncUnaryCall<TResponse> AsyncUnaryCall<TResponse>(
            Task<TResponse> responseAsync, 
            Task<Metadata> responseHeadersAsync, 
            Func<Status> getStatusFunc,
            Func<Metadata> getTrailersFunc, 
            Action disposeAction)
        {
            return new AsyncUnaryCall<TResponse>(responseAsync, responseHeadersAsync, getStatusFunc, getTrailersFunc, disposeAction);
        }
    }
}