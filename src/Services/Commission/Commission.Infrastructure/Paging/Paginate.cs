using System;
using System.Collections.Generic;
using System.Linq;

namespace AHAM.Services.Commission.Infrastructure.Paging
{
    public class Paginate<T> : IPaginate<T>
    {
        public Paginate(IList<T> items, int index, int size, int from)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (from > index) throw new ArgumentException($"indexFrom: {from} > pageIndex: {index}, must indexFrom <= pageIndex");

            Index = index;
            Size = size;
            From = from;
            Count = items.Count();
            Pages = (int) Math.Ceiling(Count / (double) Size);
            Items = items.Skip((Index - From) * Size).Take(Size).ToList();
        }

        internal Paginate()
        {
            Items = new T[0];
        }

        public int From { get; set; }
        public int Index { get; set; }
        public int Size { get; set; }
        public int Count { get; set; }
        public int Pages { get; set; }
        public IList<T> Items { get; set; }
        public bool HasPrevious => Index - From > 0;
        public bool HasNext => Index - From + 1 < Pages;
    }

    internal class Paginate<TSource, TResult> : IPaginate<TResult>
    {
        public Paginate(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter,
            int index, int size, int from)
        {
            var enumerable = source as TSource[] ?? source.ToArray();

            if (from > index) throw new ArgumentException($"From: {from} > Index: {index}, must From <= Index");

            if (source is IQueryable<TSource> query)
            {
                Index = index;
                Size = size;
                From = from;
                Count = query.Count();
                Pages = (int) Math.Ceiling(Count / (double) Size);

                var items = query.Skip((Index - From) * Size).Take(Size).ToArray();
                Items = new List<TResult>(converter(items));
            }
            else
            {
                Index = index;
                Size = size;
                From = from;
                Count = enumerable.Count();
                Pages = (int) Math.Ceiling(Count / (double) Size);

                var items = enumerable.Skip((Index - From) * Size).Take(Size).ToArray();
                Items = new List<TResult>(converter(items));
            }
        }

        public Paginate(IPaginate<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            Index = source.Index;
            Size = source.Size;
            From = source.From;
            Count = source.Count;
            Pages = source.Pages;

            Items = new List<TResult>(converter(source.Items));
        }

        public Paginate()
        {
            Items = new List<TResult>();
        }

        public int Index { get; set; }
        public int Size { get; set; }
        public int Count { get; set; }
        public int Pages { get; set; }
        public int From { get; set; }
        public IList<TResult> Items { get; set; }
        public bool HasPrevious => Index - From > 0;
        public bool HasNext => Index - From + 1 < Pages;
    }

    public static class Paginate
    {
        public static IPaginate<T> Empty<T>()
        {
            return new Paginate<T>();
        }

        public static IPaginate<TResult> From<TResult, TSource>(IPaginate<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
        {
            return new Paginate<TSource, TResult>(source, converter);
        }
    }
}