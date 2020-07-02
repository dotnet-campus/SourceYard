using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace dotnetCampus.SourceYard.Utils
{
    public class CombineReadonlyList<T> : IReadOnlyList<T>
    {
        public CombineReadonlyList(params IReadOnlyList<T>[] source)
        {
            Source = source;
        }

        public IReadOnlyList<T>[] Source { get; }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var list in Source)
            {
                foreach (var temp in list)
                {
                    yield return temp;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => Source.Sum(temp => temp.Count);

        public T this[int index]
        {
            get
            {
                var n = index;
                var source = Source;

                foreach (var list in source)
                {
                    if (n < list.Count)
                    {
                        return list[n];
                    }

                    n -= list.Count;
                }

                throw new IndexOutOfRangeException();
            }
        }
    }
}