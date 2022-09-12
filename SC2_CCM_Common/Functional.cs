using System;
using System.Linq;

namespace SC2_CCM_Common
{
    public static class Functional
    {
        public static Func<T, bool> And<T>(params Func<T, bool>[] funcs)
        {
            return input => funcs.All(func => func(input));
        }
    }
}