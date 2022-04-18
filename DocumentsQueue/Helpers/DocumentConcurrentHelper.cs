using System.Collections.Concurrent;

namespace ArturBhasker.TerralinkTestProject.Helpers
{
    internal static class DocumentConcurrentHelper
    {
        public static IEnumerable<T> DequeueList<T>(this ConcurrentQueue<T> queue, int count) where T : struct
        {
            for (int i = 0; i < count; i++)
            {
                if (!queue.TryDequeue(out T item))
                {
                    break;
                }

                yield return item;
            }
        }
    }
}