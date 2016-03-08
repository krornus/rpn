using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class ParseTree 
{
   public static System.Collections.Generic.IEnumerable<KeyValuePair<string,string>> All(Tree<string, string> parse_tree, IEnumerable<string> enumerable_sentence)
   {
      // Create RPN stacks
      Stack<int> integers = new Stack<int>();
      Stack<char> operations = new Stack<char>();

      foreach(string value in enumerable_sentence)
      {
         KeyValuePair<string, Node<string,string>> node_kvp = parse_tree.GetChildNodeByValue(value, Regex.IsMatch);

         if(node_kvp.Value != null)
         {
            yield return new KeyValuePair<string, string>(node_kvp.Key, value);
            parse_tree.Traverse(node_kvp.Key);
         }
         else
         {
            Console.WriteLine("Syntax error!");
            Console.WriteLine($"Value: '{value}'");
            Console.WriteLine("Expected one of the following:");

            foreach(KeyValuePair<string, Node<string, string>> node in parse_tree.Current.Children)
            {
               Console.WriteLine($"\t'{node.Value.Data}' : {node.Key}");
            }
            throw new Exception("Bad syntax");
         }
      }
   }

   public static Tree<string, string> ParseGrammar(string start,List<string> grammar, Dictionary<string, string> token_definitions)
   {
      Tree<string,string> tree = new Tree<string, string>();
      Dictionary<string, Node<string,string>> nodes = new Dictionary<string, Node<string, string>>();

      // Create nodes
      foreach(KeyValuePair<string, string> kvp in token_definitions)
      {
         nodes[kvp.Key] = new Node<string, string>(kvp.Value);
      }

      // 
      foreach(string line in grammar)
      {
         Match match = Regex.Match(line, @"([^:\s]+)\s?:\s?");

         string[] values = Regex.Replace(line, @"[^:\s]+\s?:\s?", "").Split('|');

         if(!match.Success || values == null)
         {
            Console.WriteLine("Bad grammar!");
            return null;
         }

         string definition = match.Groups[1].Value;

         if(definition == start)
         {
            if(!nodes.ContainsKey(definition))
            {
               Console.WriteLine($"definition {definition} does not exist!");
               return null;
            }
            tree.Insert(definition, nodes[definition]);
         }

         tree.Traverse(definition);

         foreach(string value in values)
         {
            if(!nodes.ContainsKey(value.Trim()))
            {
               Console.WriteLine($"cannot set {definition} to {value.Trim()}, {value.Trim()} does not exist!");

               return null;
            }

            tree.Insert(value.Trim(), nodes[value.Trim()]);
         }
      }

      tree.Head();

      return tree;
   }
}

