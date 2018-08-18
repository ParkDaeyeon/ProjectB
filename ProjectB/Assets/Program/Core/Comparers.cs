using System;
using System.Collections.Generic;

namespace Program.Core
{
    public class CodeComparer : IComparer<long>
    {
        public int Compare(long x, long y)
        {
            return x < y ? -1 : x > y ? +1 : 0;
        }
    }
    public class IndexComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x < y ? -1 : x > y ? +1 : 0;
        }
    }
}
