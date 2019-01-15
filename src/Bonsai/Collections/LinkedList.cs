namespace Bonsai.Collections
{
    using System.Collections.Generic;

    public class LinkedList<T>
    {
        public Node<T> FirstNode { get; set; }

        public void AddNode(T item)
        {
            FirstNode = new Node<T>()
            {
                Next = FirstNode,
                Value = item
            };
        }

        public IEnumerable<T> GetItems()
        {
            Node<T> node = FirstNode;
            while (node != null)
            {
                yield return node.Value;
                node = node.Next;
            }
        }
   
        
        
        public class Node<T>
        {
            public Node<T> Next { get; set; }
            public T Value { get; set; }
        }
    }


   
    
}