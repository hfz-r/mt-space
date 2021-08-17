using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AHAM.Services.Investor.Infrastructure.Paging
{
    public static class PaginateExtensions
    {
        public static IPaginate<T> ToPaginate<T>(this IEnumerable<T> source, int index, int size, int from = 0)
        {
            return new Paginate<T>(source, index, size, from);
        }

        public static IPaginate<TResult> ToPaginate<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> converter,
            int index,
            int size,
            int from = 0)
        {
            return new Paginate<TSource, TResult>(source, converter, index, size, from);
        }

        public static async Task<IPaginate<T>> ToPaginateAsync<T>(
            this IQueryable<T> source,
            int index,
            int size,
            int from = 0,
            CancellationToken cancellationToken = default)
        {
            if (from > index) throw new ArgumentException($"From: {from} > Index: {index}, must From <= Index");

            var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await source.Skip((index - from) * size)
                .Take(size)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var list = new Paginate<T>
            {
                Index = index,
                Size = size,
                From = from,
                Count = count,
                Items = items,
                Pages = (int) Math.Ceiling(count / (double) size)
            };

            return list;
        }

        public static async Task<IPaginate<TResult>> ToPaginateAsync<TSource, TResult>(
            this IQueryable<TSource> source,
            Func<IEnumerable<TSource>, IEnumerable<TResult>> converter,
            int index,
            int size,
            int from = 0,
            CancellationToken cancellationToken = default)
        {
            if (from > index) throw new ArgumentException($"From: {from} > Index: {index}, must From <= Index");

            var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            var items = await source.Skip((index - from) * size)
                .Take(size)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var list = new Paginate<TSource, TResult>
            {
                Index = index,
                Size = size,
                From = from,
                Count = count,
                Items = new List<TResult>(converter(items)),
                Pages = (int) Math.Ceiling(count / (double) size),
            };

            return list;
        }
    }
}