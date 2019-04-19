namespace Bonsai.Collections.LinkedLists
{
    using System.Collections.Generic;


    public class LinkedList<T> : ILinkedList<T>
    {
        private Node<T> _firstNode;

        public virtual void Add(T item)
        {
            _firstNode = new Node<T>
            {
                Next = _firstNode,
                Value = item
            };
        }
        

        public virtual IEnumerable<T> GetAll()
        {
            var node = _firstNode;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }   
    }
}