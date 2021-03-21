using System.Collections.Generic;
#if !NET5
namespace LoggerHealthCheck
{
    public static class StackExtensionMethods
    {
        /// <summary>
        /// Pollyfill
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stack"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryPeek<T>(this Stack<T> stack, out T? result)
        {
            if (stack.Count > 0)
            {
                result = stack.Peek();
                return true;
            }
            result = default;
            return false;
        }
    }
}
#endif