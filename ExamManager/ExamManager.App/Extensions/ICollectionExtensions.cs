using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Extensions
{
    public static class ICollectionExtensionsHelper
    {
        public static void AddRange<T>(
            this ICollection<T> coll, 
            IEnumerable<T> data)
        {
            foreach(T d in data)
            {
                coll.Add(d);
            }
        }
    }
}
