using System;
using System.Collections.Generic;
using System.Linq;

namespace BluePrintAssembler.Utils
{
    public static class Extensions
    {
        public static T? MinOrDefault<T>(this IEnumerable<T> that)
            where T : struct, IComparable
        {
            if (!that.Any())
            {
                return null;
            }
            return that.Min();
        }

        public static T? MaxOrDefault<T>(this IEnumerable<T> that)
            where T : struct, IComparable
        {
            if (!that.Any())
            {
                return null;
            }
            return that.Max();
        }
        public static T SelectMax<T>(this IEnumerable<T> collection,Func<T,int> selector)
        {
            int? oldMax = null;
            var maxItem = default(T);
            foreach (var item in collection)
            {
                var value = selector(item);
                if (oldMax != null && oldMax >= value) continue;
                maxItem = item;
                oldMax = value;
            }
            return maxItem;
        }
        public static T SelectMax<T>(this IEnumerable<T> collection,Func<T,int,int> selector)
        {
            int? oldMax = null;
            var maxItem = default(T);
            var index = 0;
            foreach (var item in collection)
            {
                var value = selector(item,index++);
                if (oldMax != null && oldMax >= value) continue;
                maxItem = item;
                oldMax = value;
            }
            return maxItem;
        }

        public static int FindMax<T>(this IEnumerable<T> collection, Func<T, int, int> selector)
        {
            int oldMaxIndex = -1;
            int oldMaxValue = 0;
            var index = -1;
            foreach (var item in collection)
            {
                index++;
                var value = selector(item, index);
                if (oldMaxIndex != -1 && oldMaxValue >= value) continue;
                oldMaxValue = value;
                oldMaxIndex = index;
            }
            return oldMaxIndex;
        }
        public static int FindMax<T>(this IEnumerable<T> collection, Func<T, int, double> selector)
        {
            int oldMaxIndex = -1;
            double oldMaxValue = 0;
            var index = -1;
            foreach (var item in collection)
            {
                index++;
                var value = selector(item, index);
                if (oldMaxIndex != -1 && oldMaxValue >= value) continue;
                oldMaxValue = value;
                oldMaxIndex = index;
            }
            return oldMaxIndex;
        }

        public static T SelectMin<T>(this IEnumerable<T> collection, Func<T, int> selector)
        {
            int? oldMin = null;
            var minItem = default(T);
            foreach (var item in collection)
            {
                var value = selector(item);
                if (oldMin != null && oldMin <= value) continue;
                minItem = item;
                oldMin = value;
            }
            return minItem;
        }
        public static T SelectMin<T>(this IEnumerable<T> collection, Func<T,int, int> selector)
        {
            int? oldMin = null;
            var minItem = default(T);
            var index = 0;
            foreach (var item in collection)
            {
                var value = selector(item,index++);
                if (oldMin != null && oldMin <= value) continue;
                minItem = item;
                oldMin = value;
            }
            return minItem;
        }
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

        public static V GetOrCreate<K, V>(this IDictionary<K, V> dict, K key, Func<V> creatorFunc)
        {
            if (dict.TryGetValue(key, out V val))
                return val;
            return dict[key] = creatorFunc();
        }
        public static V GetOrDefault<K, V>(this IDictionary<K, V> dict, K key, Func<V> creatorFunc)
        {
            if (dict.TryGetValue(key, out V val))
                return val;
            return creatorFunc();
        }
    }
}
