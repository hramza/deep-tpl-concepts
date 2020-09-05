using Hra.Framework.Utils;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace System.Threading.Tasks
{
    public static class HraTaskExtensions
    {
        public static T WaitForResult<T>(this Task<T> task, TimeSpan? timeOut = null)
        {
            if (timeOut == null) task.Wait();
            else task.Wait(timeOut.Value);

            return task.IsFaulted ? throw task.Exception : task.Result;
        }

        public static async Task<T> WaitForResultAsync<T>(this Task<T> task, TimeSpan timeOut)
        => await Task.WhenAny(new[] { task, Task.Delay(timeOut) }).ConfigureAwait(false) == task ?
                await task.ConfigureAwait(false) :
                throw new TimeoutException($"Task failed to complete within {timeOut}");

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="parallelNumber"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static Task Execute<T>(this IEnumerable<T> source, int partitionCount, Func<T, Task> body)
        {
            Ensure.NotNullOrEmpty(source, nameof(source));

            if (partitionCount <= 0) throw new ArgumentOutOfRangeException(nameof(partitionCount));

            var tasks = Partitioner.Create(source).GetPartitions(partitionCount)
                .Select(async partition =>
                {
                    using (partition)
                        while (partition.MoveNext()) await body(partition.Current).ConfigureAwait(false);
                });

            return Task.WhenAll(tasks);
        }

        public static async Task Execute<T>(this IEnumerable<T> collection,
            int maxTasksNumber,
            Func<T, Task> executeFunc,
            CancellationToken? cancellationToken = default)
        {
            Ensure.NotNullOrEmpty(collection, typeof(T).Name);

            if (maxTasksNumber <= 0) throw new ArgumentOutOfRangeException(nameof(maxTasksNumber));

            int count = collection.GetCount();

            int index = 0;
            Func<Task>[] funcs = new Func<Task>[count];
            foreach (T element in collection)
            {
                funcs[index++] = () => executeFunc(element);
            }

            // Execute with partitions is much more faster
            // avoid loosing time on implementing a class that derives from TaskScheduler and better than semaphoreslim
            // https://source.dot.net/#System.Private.CoreLib/TaskScheduler.cs,b76a4a6f77962f28
            ConcurrentQueue<Func<Task>> queue = new ConcurrentQueue<Func<Task>>(funcs);

            Task[] tasks = new Task[maxTasksNumber];
            index = 0;
            while (index < maxTasksNumber)
            {
                if (cancellationToken != null && cancellationToken.Value.IsCancellationRequested) break;

                tasks[index++] = Task.Run(async () =>
                {
                    while (!queue.IsEmpty)
                    {
                        if (queue.TryDequeue(out Func<Task> execute))
                        {
                            await execute().ConfigureAwait(false);
                        }
                    }
                }, cancellationToken.Value);
            }

            cancellationToken?.ThrowIfCancellationRequested();

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Avoid TaskCancelledException
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task WithoutTaskCancelledException(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (TaskCanceledException) { }
        }

        public static Type GetTaskResultType(this Task task)
        {
            if (task == null) return null;

            Type type = task.GetType();
            return type != typeof(Task) ?
                (type.IsGenericParameter ? type.GetGenericArguments()[0] : null) : null;
        }
    }
}
