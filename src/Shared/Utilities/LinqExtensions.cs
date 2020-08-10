using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDMT.Shared
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? items)
        {
            return items ?? Enumerable.Empty<T>();
        }
    }
}
