namespace Bones.Internal
{
    using System;

    public static class Code 
    {
        public static void Require<T>(Func<bool> predicate, Func<T> ex) where T: Exception
        {
            if (predicate()) return;
            throw ex();
        }

        public static void Require(Func<bool> predicate, string name)
        {
            if (predicate()) return;
            throw new ArgumentException(name);
        }

        public static void Ensure<T>(Func<bool> predicate, Func<T> ex) where T: Exception
        {
            if (predicate()) return;
            throw ex();
        }

    }
}