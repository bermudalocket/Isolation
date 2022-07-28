using System;
using System.Collections.Generic;
using System.Linq;

namespace Isolation.Scripts {

    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExtensions {
        
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            foreach (var item in source) {
                action(item);
            }
        }
        
        public static T RandomElement<T>(this IEnumerable<T> enumerable) {
            return enumerable.RandomElement(new Random());
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable, Random rand) {
            var enumerable1 = enumerable as T[] ?? enumerable.ToArray();
            int index = rand.Next(0, enumerable1.Count());
            return enumerable1.ElementAt(index);
        }
        
    }

}