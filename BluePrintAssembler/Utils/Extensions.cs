using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BluePrintAssembler.Utils
{
    public static class Extensions
    {

        private class TopologicalSortItem<T>
        {
            public T Item;
            public bool TemporaryMark;
            public bool PermanentMark;
        }
        public static IEnumerable<T> TopologicalSort<T>(this IEnumerable<T> coll,Func<T,T,bool> is1DependsOn2)
        {
            var unsorted = coll.Select(x => new TopologicalSortItem<T> {Item = x}).ToArray();
            var sorted = new List<T>();

            void Visit(TopologicalSortItem<T> unode)
            {
                if (unode.PermanentMark) return;
                if (unode.TemporaryMark) throw new FormatException("Cyclic dependencies found!");
                unode.TemporaryMark = true;
                foreach (var depNode in unsorted.Where(item => is1DependsOn2(item.Item,unode.Item)))
                    Visit(depNode);
                unode.PermanentMark = true;
                sorted.Insert(0, unode.Item);
            }


            TopologicalSortItem<T> node;
            while ((node = unsorted.FirstOrDefault(x => !x.TemporaryMark && !x.PermanentMark)) != null)
                Visit(node);
            return sorted;
        }
    }
}
