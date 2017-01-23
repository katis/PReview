using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PReview
{
    static class TaskExt
    {
        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> en)
        {
            return Task.WhenAll(en);
        }

        public static T Defaulted<T>(this Nullable<T> value) where T : struct
        {
            return value.HasValue ? value.Value : default(T);
        }
    }
}
