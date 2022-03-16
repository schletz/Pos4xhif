using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Extensions
{
    public static class StringExtensions
    {
        public static string Left(this string val, int count)
        {
            return val.Length < count 
                ? val 
                : val.Substring(0, count);
        }
    }
}
