namespace SC2_CCM_Common
{
    /// <summary>
    /// Helper function for high-order functions
    /// </summary>
    public static class Functional
    {
        /// <summary>
        /// Takes several predicate functions and returns a function that will check if a value passes all predicates
        /// </summary>
        /// <param name="funcs">Predicate functions to take</param>
        /// <typeparam name="T">Type of input for the predicate</typeparam>
        /// <returns>A function that takes an input and returns whether it passes all predicates</returns>
        public static Func<T, bool> And<T>(params Func<T, bool>[] funcs)
        {
            return input => funcs.All(func => func(input));
        }
    }
}