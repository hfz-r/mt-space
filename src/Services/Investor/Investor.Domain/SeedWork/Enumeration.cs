using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AHAM.Services.Investor.Domain.SeedWork
{
    public abstract class Enumeration : IComparable
    {
        public string Code { get; }

        public string Name { get; }

        protected Enumeration(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Enumeration otherValue))
                return false;

            var typeMatches = GetType() == obj.GetType();
            var valueMatches = Code.Equals(otherValue.Code);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Code.GetHashCode();

        public int CompareTo(object other) => String.Compare(Code, ((Enumeration) other).Code, StringComparison.Ordinal);

        private static T Parse<T, TK>(TK value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }
    }
}