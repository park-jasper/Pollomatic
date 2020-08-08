using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;

namespace Pollomatic.Domain.Extensions
{
    public static class EnumerableExtensions
    {
        public static IPartialEnumerable<TElement> TakePartialWhile<TElement>(this IEnumerable<TElement> source,
            Func<TElement, bool> predicate)
        {
            return new PartialEnumerable<TElement>(source, predicate);
        }

        public static void Invoke<TElement, TResult>(this IEnumerable<TElement> source, Func<IEnumerable<TElement>, TResult> func, out TResult result)
        {
            result = func(source);
        }

        public static IEnumerable<TElement> WhereNot<TElement>(this IEnumerable<TElement> source, Func<TElement, bool> predicate)
        {
            return source.Where(x => !predicate(x));
        }

        public static void ForEach(this IEnumerable source, Action<object> action)
        {
            foreach (var ele in source)
            {
                action(ele);
            }
        }
    }

    internal class PartialEnumerable<TElement> : IPartialEnumerable<TElement>
    {
        private IEnumerable<TElement> _source;
        private Func<Func<TElement, bool>> _predicate;
        public PartialEnumerable(IEnumerable<TElement> source, Func<TElement, bool> predicate)
        {
            _source = source;
            _predicate = () => predicate;
        }
        public PartialEnumerable(IEnumerable<TElement> source, int count)
        {
            _source = source;
            _predicate = () =>
            {
                int index = 0;
                return _ =>
                {
                    index += 1;
                    return index <= count;
                };
            };
        }
        public IEnumerator<TElement> GetEnumerator()
        {
            var pred = _predicate();
            return _source.TakeWhile(pred).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public IEnumerable<TElement> InvokePartial<TResult>(Func<IEnumerable<TElement>, TResult> func, out TResult result)
        {
            var part = new List<TElement>();
            var pred = _predicate();
            var enumerator = _source.GetEnumerator();
            bool currentValid = false;
            while (enumerator.MoveNext())
            {
                if (pred(enumerator.Current))
                {
                    part.Add(enumerator.Current);
                }
                else
                {
                    currentValid = true;
                    break;
                }
            }
            result = func(part);
            return MakeRest(currentValid, enumerator);
        }

        private static IEnumerable<TElement> MakeRest(bool currentValid, IEnumerator<TElement> enumerator)
        {
            if (currentValid)
            {
                yield return enumerator.Current;
            }
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
    }

    public interface IPartialEnumerable<TElement> : IEnumerable<TElement>
    {
        IEnumerable<TElement> InvokePartial<TResult>(Func<IEnumerable<TElement>, TResult> func, out TResult result);
    }
}