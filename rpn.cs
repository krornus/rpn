using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class RPNCalc
{
   public static int Calculate(string sentence)
   {
      Tree<string, string> parse_tree = GetTree();

      foreach(string token in ParseTree.All(parse_tree, sentence))
      {
         Console.WriteLine(token);
      }

      return 0;
   }

   private static Tree<string, string> ParseGrammar(string start,List<string> grammar, Dictionary<string, string> token_definitions)
   {
      // start : integer
      // integer : whitespace | integer
      // whitespace : integer | binary_operator
      // binary_operator : binary_whitespace
      // binary_whitespace : binary_operator

      Tree<string,string> tree = new Tree<string, string>();
      Dictionary<string, Node<string,string>> nodes = new Dictionary();

      foreach(KeyValuePair<string, string> kvp in token_definitions)
      {
         nodes[kvp.Key] = new Node<string, string>(kvp.Value);
      }
      
      foreach(string line in grammar)
      {
         Match match = Regex.Match(line, @"([^:\s]+)\s?:\s?");
         
         string[] values = Regex.Replace(line, @"[^:\s]+\s?:\s?", "").Split('|');

         if(!match.Success || values.Count == null)
         {
            Console.WriteLine("Bad grammar!");
            break;
         }
         
         string definition = match.Groups[1].Value;
         
         if(definition == start)
         {
            parse_tree.Insert(definition, nodes[definition]);
         }

         parse_tree.Traverse(definition);

         foreach(string value in values)
         {
            parse_Tree.Insert(value, nodes[value]);
         }
      }

   }

   private static Tree<string, string> GetTree()
   {
      string binary_operator_pattern = @"[\+\-\*\\%\^]";
      string integer_pattern = @"[0-9]";
      string whitespace_pattern = @"\s";

      Tree<string, string> parse_tree = new Tree<string, string>();

      // Create nodes so we can backreference them as a child
      // EX: integer -> integer will always expect an integer
      Node<string, string> integer= new Node<string, string>(integer_pattern);
      Node<string, string> binary_operator= new Node<string, string>(binary_operator_pattern);
      Node<string, string> whitespace = new Node<string, string>(whitespace_pattern);
      Node<string, string> binary_whitespace = new Node<string, string>(whitespace_pattern);

      // Assumes tree is trimmed of whitespace
      // This means the sentence must start with an integer to be valid
      parse_tree.Insert("integer", integer);

      // Enter integer branch
      // Expect either whitespace, or a longer integer
      parse_tree.Traverse("integer");
      parse_tree.Insert("whitespace", whitespace);
      parse_tree.Insert("integer", integer);

      // Enter whitespace branch
      // Expect either integer or binary operator
      parse_tree.Traverse("whitespace");
      parse_tree.Insert("integer", integer);
      parse_tree.Insert("binary_operator", binary_operator);
      
      // Enter binary_operator branch
      // Expect only whitespace
      parse_tree.Traverse("binary_operator");
      parse_tree.Insert("binary_whitespace", binary_whitespace);

      // Enter whitespace (different from other whitespace in integer branch)
      // Expect only binary operator
      parse_tree.Traverse("binary_whitespace");
      parse_tree.Insert("binary_operator", binary_operator);
      
      parse_tree.Head();

      return parse_tree;
   }

}

public class ParseTree 
{
   public Dictionary<string, int> tokens = new  Dictionary<string, int>(); 

   public ParseTree(string tokens)
   {
      Tokens = GetEnumFromString(tokens);   
   }

   public Dictionary<string, int> tokens = new Dictionary<string, int>();

   public System.Collections.Generic.IEnumerable<string> All(Tree<string, string> parse_tree, string sentence)
   {
      // Format input string, remove newlines and leading/trailing whitespace
      sentence = Regex.Replace(sentence, @"[\n\r]", "").Trim();

      // Create RPN stacks
      Stack<int> integers = new Stack<int>();
      Stack<char> operations = new Stack<char>();

      for(int i = 0; i < sentence.Length; i++)
      {
         char c = sentence[i];

         KeyValuePair<string, Node<string,string>> node_kvp = parse_tree.GetChildNodeByValue(c.ToString(), Regex.IsMatch);

         if(node_kvp.Value != null)
         {
            yield return node_kvp.Key;
            parse_tree.Traverse(node_kvp.Key);
         }
         else
         {
            Console.WriteLine("Syntax error!");
            Console.WriteLine($"Character {i + 1}: '{c}'");
            Console.WriteLine("Expected one of the following:");
            
            foreach(KeyValuePair<string, Node<string, string>> node in parse_tree.Current.Children)
            {
               Console.WriteLine($"\t'{node.Value.Data}' : {node.Key}");
            }
            throw new Exception("Bad syntax");
         }
      }
   
   }
}
