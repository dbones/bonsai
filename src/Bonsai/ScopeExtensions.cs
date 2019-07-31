namespace Bonsai
{
    using System.Collections.Generic;

    public static class ScopeExtensions
    {
        /// <summary>
        /// resolves the service into all instances of the target type
        /// </summary>
        /// <typeparam name="TService">the required service</typeparam>
        /// <returns>instance of the service</returns>
        public static IEnumerable<TService> ResolveAll<TService>(this IScope scope)
        {
            return scope.Resolve<IEnumerable<TService>>();
        }
    }
}