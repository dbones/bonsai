namespace Bones.Tests.TestModels
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     counter of when a dispose method is called
    /// </summary>
    public class ClassMonitor
    {
        public IDictionary<object, int> Disposed { get; set; }

        public ClassMonitor()
        {
            Disposed = new Dictionary<object, int>();
        }

        /// <summary>
        ///     check for the number of times a type of object is disposed of
        /// </summary>
        /// <typeparam name="T">the object type in interest</typeparam>
        public int NumberOfDisposedInstancesOf<T>()
        {
            var count = Disposed.Where(item => item.Key.GetType() == typeof(T)).Sum(item => item.Value);
            return count;
        }

        /// <summary>
        ///     check for the number of times a type of object is disposed of
        /// </summary>
        /// <typeparam name="T">the object type in interest</typeparam>
        public int NumberOfDisposedAnyInstancesOf<T>()
        {
            var count = Disposed.Where(item => typeof(T).IsAssignableFrom(item.Key.GetType())).Sum(item => item.Value);
            return count;
        }

        /// <summary>
        ///     returns the number of times an instance is disposed
        /// </summary>
        /// <param name="instance">the instance to check</param>
        public int CheckObject(object instance)
        {
            return !Disposed.ContainsKey(instance) ? 0 : Disposed[instance];
        }

        /// <summary>
        ///     Call this inside the dispose methods of classes which you are interested in
        /// </summary>
        /// <param name="instance">pass the object instance into this method</param>
        public void ObjectDisposed(object instance)
        {
            if (!Disposed.ContainsKey(instance))
                Disposed.Add(instance, 0);

            Disposed[instance]++;
        }

    }
}