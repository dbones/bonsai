namespace Bones
{
    using System;

    /// <summary>
    /// The main container, please use <see cref="CreateScope"/>
    /// </summary>
    public interface IContainer : IDisposable
    {
        /// <summary>
        /// Creates a scope which you can use to resolve objects with 
        /// </summary>
        /// <param name="name">the name of the scope, this will default to 'scope'</param>
        IScope CreateScope(string name = "scope");
    }
}