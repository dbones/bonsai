namespace Bonsai.Internal
{
    using System;

    /// <summary>
    /// simple guard class
    /// </summary>
    public static class Code
    {
        /// <summary>
        /// requires that code passes a predicate
        /// </summary>
        /// <param name="predicate">a truthful statement that is require to pass</param>
        /// <param name="ex">the exception th throw</param>
        public static void Require<T>(Func<bool> predicate, Func<T> ex) where T : Exception
        {
            if (predicate())
            {
                return;
            }

            throw ex();
        }

        /// <summary>
        /// requires that code passes a predicate
        /// </summary>
        /// <param name="predicate">a truthful statement that is require to pass</param>
        /// <param name="name">name of the property, parameter being test</param>
        /// <exception cref="ArgumentException"></exception>
        public static void Require(Func<bool> predicate, string name)
        {
            if (predicate())
            {
                return;
            }

            throw new ArgumentException(name);
        }

        /// <summary>
        /// ensure that the code has mutated the state correct
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="ex"></param>
        /// <typeparam name="T"></typeparam>
        public static void Ensure<T>(Func<bool> predicate, Func<T> ex) where T : Exception
        {
            if (predicate())
            {
                return;
            }

            throw ex();
        }
    }
}