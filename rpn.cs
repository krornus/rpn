using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class RPNCalc
{
   public static float Calculate(string sentence)
   {
      Tree<string, string> parse_tree = GetTree();

      Stack<float> value_stack = new Stack<float>();
      
      Dictionary<string, Func<float, float, float>> operators
         = new Dictionary<string, Func<float, float, float>>()
         {
            { "+", (x, y) => x + y },
            { "-", (x, y) => x - y },
            { "*", (x, y) => x * y },
            { "/", (x, y) => x / y }, 
            { "%", (x, y) => x % y },
         };


      foreach(KeyValuePair<string, string> token in ParseTree.All(parse_tree, sentence.Split()))
      {
         Print(value_stack);
         if(token.Key == "binary_operator")
         {
            float a = value_stack.Pop();
            float b = value_stack.Pop();

            value_stack.Push(operators[token.Value](b, a)); 
         }
         else
         {
            value_stack.Push(float.Parse(token.Value));
         }
      }

      
      return value_stack.Pop();
   }

   private static void Print(Stack<float> stack)
   {
      foreach(float f in stack)
      {
         Console.WriteLine(f);
      }

      Console.WriteLine("---");
   }

   private static Tree<string, string> GetTree()
   {
      string binary_operator_pattern = @"[\+\-\*\\%\^]";
      string integer_pattern = @"-?[0-9]+.?[0-9]*";
      string whitespace_pattern = @"\s";

      Dictionary<string,string> tokens = new Dictionary<string, string>()
      {
         { "integer", integer_pattern },
         { "binary_operator", binary_operator_pattern }
      };

      List<string> grammar = new List<string>();
      grammar.Add("integer : binary_operator | integer");
      grammar.Add("binary_operator : integer | binary_operator");

      return ParseTree.ParseGrammar("integer", grammar, tokens);
   }

}


