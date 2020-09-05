using Hra.Framework.Utils;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// No use of linq => low-allocations => better performance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static int GetCount<T>(this IEnumerable<T> collection)
        {
            Ensure.NotNull(collection, typeof(T).Name);

            if (collection is ICollection<T> col) return col.Count;

            if (collection is IList<T> list) return list.Count;

            int count = 0;
            using IEnumerator<T> enumerator = collection.GetEnumerator();
            checked
            {
                while (enumerator.MoveNext()) count++;
            }

            return count;
        }

        /// <summary>
        /// Batch a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> BatchList<T>(this List<T> list, int batchSize = 100)
        {
            if (list == null) throw new ArgumentNullException(typeof(T).Name);

            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

            for (int index = 0; index < list.Count; index += batchSize)
                yield return list.GetRange(index, Math.Min(batchSize, list.Count - index));
        }

        /// <summary>
        /// /batch IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="batchSize"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> BatchEnumerable<T>(this IEnumerable<T> collection, int batchSize = 100)
        {
            Ensure.NotNullOrEmpty(collection, typeof(T).Name);

            if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

            List<T> batch = new List<T>(batchSize);
            foreach (T element in collection)
            {
                batch.Add(element);
                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<T>();
                }
            }

            if (batch.Count > 0) yield return batch;
        }
    }
}
