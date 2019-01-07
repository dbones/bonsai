namespace Bones
{
    using System;

    public interface IContainer : IDisposable
    {
        IScope CreateScope(string name = "scope");
    }
}