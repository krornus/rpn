using System;
using System.Linq;
using System.Collections.Generic;

public class Tree<K,D> where D : class 
{
   private Dictionary<K, Node<K,D>> _map = new Dictionary<K, Node<K,D>>();
   private Node<K,D> _current = new Node<K,D>();
   private Node<K,D> _head; 

   public Node<K,D> Current { get { return _current; } }

   public Tree()
   {
      _head = _current;
   }
   

   public void Insert(K key, D data)
   {
      _current[key] = new Node<K,D>(data);
   }

   public void Insert(K key, Node<K, D> node)
   {
      _current[key] = node;
   }

   public bool Traverse(K key)
   {
      if(_current.HasChild(key))
      {
         _current = _current[key];
         return true;
      }

      return false;
   }

   public void Head()
   {
      _current = _head;
   }

   public KeyValuePair<K, Node<K,D>> GetChildNodeByValue(D value, Func<D, D, bool> predicate)
   {

      return _current.GetNodeByValue(value, predicate);
   }

}

public class Node<K,D>  where D : class 
{
   public Node()
   {
   }

   public Node(D data)
   {
      Data = data;
   }

   public Node<K,D> this[K key]
   {
      get 
      {
         if(_children.ContainsKey(key))
         {
            return _children[key];
         }

         return null;
      }

      set
      {
         _children[key] = value;
      }
   }

   public D Data { get; set; }
   private Dictionary<K, Node<K,D>> _children = new Dictionary<K, Node<K,D>>();

   public Dictionary<K, Node<K,D>> Children { get { return _children; } }

   public Node<K,D> Insert(K key, D data)
   {
      Node<K,D> node = new Node<K,D>(data);
      _children[key] = node;

      return node;
   }
   
   public bool HasChild(K key)
   {

      return _children.ContainsKey(key);
   }

   public KeyValuePair<K, Node<K,D>> GetNodeByValue(D value, Func<D, D, bool> predicate)
   {

      return _children.FirstOrDefault(x => predicate(value, x.Value.Data));
   }
}
