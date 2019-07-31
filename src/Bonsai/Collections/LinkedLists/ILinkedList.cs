namespace Bonsai.Collections.LinkedLists
{
    using System.Collections.Generic;

    public interface ILinkedList<T>
    {
        void Add(T item);
        IEnumerable<T> GetAll();
    }
}