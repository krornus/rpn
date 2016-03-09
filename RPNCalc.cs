using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class RPNCalculator
{
   private Dictionary<string, Func<float, float, float>> binary_operators 
      = new Dictionary<string, Func<float, float, float>>()
      {
         { "+", (x, y) => x + y },
         { "-", (x, y) => x - y },
         { "*", (x, y) => x * y },
         { "/", (x, y) => x / y }, 
         { "%", (x, y) => x % y },
         { "^", (x, y) => (float)Math.Pow((int)x, (int)y) }
      };
   private Dictionary<string, Func<float, float>> unary_operators 
      = new Dictionary<string, Func<float, float>>()
      {
         { "sin", (x) => (float)Math.Sin(x)         },
         { "cos", (x) => (float)Math.Cos(x)         },
         { "tan", (x) => (float)Math.Tan(x)         },
         { "asin", (x) => (float)Math.Asin(x)       },
         { "acos", (x) => (float)Math.Acos(x)       },
         { "atan", (x) => (float)Math.Atan(x)       },
         { "ceiling", (x) => (float)Math.Ceiling(x) },
         { "floor", (x) => (float)Math.Floor(x)     },
         { "log", (x) => (float)Math.Log(x)         },
         { "abs", (x) => (float)Math.Abs(x)         },
         { "round", (x) => (float)Math.Round(x)     },
         { "sqrt", (x) => (float)Math.Sqrt(x)       }
      };

   private Dictionary<string, float> variables 
      = new Dictionary<string, float>();


   public float Calculate(string sentence)
   {
      sentence = sentence.Trim();

      Match match = Regex.Match(sentence, @"^([_a-z][A-Z_\-a-z0-9]+)\s*=\s*");

      if(match.Success)
      {
         sentence = Regex.Replace(sentence, @"^\s*[^=\s]+\s*=\s*", "");
      }

      Tree<string, string> parse_tree = GetTree();

      Stack<float> value_stack = new Stack<float>();
      
      foreach(KeyValuePair<string, string> token in ParseTree.All(parse_tree, sentence.Split()))
      {
         if(token.Key == "binary_operator")
         {
            float a = value_stack.Pop();
            float b = value_stack.Pop();

            value_stack.Push(binary_operators[token.Value](b, a)); 
         }
         else if(token.Key == "unary_operator")
         {
            value_stack.Push(unary_operators[token.Value](value_stack.Pop()));
         }
         else if(token.Key == "integer")
         {
            value_stack.Push(float.Parse(token.Value));
         }
         else if(token.Key == "variable" && variables.ContainsKey(token.Value))
         {
            value_stack.Push(variables[token.Value]);
         }
         else
         {
            Console.WriteLine("Invalid syntax.");
            Console.WriteLine($"Attempted operation: {token.Key}");
            throw new ArgumentException($"Inalid syntax {sentence}");
         }
      }
      
      if(match.Success)
      {
         variables[match.Groups[1].Value] = value_stack.Peek();
      }

      return value_stack.Pop();
   }

   private void Print(Stack<float> stack)
   {
      foreach(float f in stack)
      {
         Console.WriteLine(f);
      }

      Console.WriteLine("---");
   }

   private Tree<string, string> GetTree()
   {
      string binary_operator_pattern = @"[\+\-\*/%\^]";
      string unary_operator_pattern = @"sin|cos|tan|asin|acos|atan|ceiling|floor|log|abs|round|sqrt";
      string integer_pattern = @"^-?[0-9]+.?[0-9]*";
      string variable_pattern = @"^[_a-zA-Z][0-9a-z-A-Z_\-]*";

      Dictionary<string,string> tokens = new Dictionary<string, string>()
      {
         { "integer", integer_pattern },
         { "variable", variable_pattern },
         { "binary_operator", binary_operator_pattern },
         { "unary_operator",  unary_operator_pattern }
      };

      List<string> grammar = new List<string>();
      grammar.Add("integer : binary_operator | integer | variable | unary_operator");
      grammar.Add("variable : binary_operator | integer | unary_operator | variable");
      grammar.Add("binary_operator : integer | binary_operator | unary_operator | variable");
      grammar.Add("unary_operator : integer | binary_operator | unary_operator | variable");

      return ParseTree.ParseGrammar("integer", grammar, tokens);
   }

}
