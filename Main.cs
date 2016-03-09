using System;

namespace Program
{
   class Program 
   {
      public static void Main(string[] args)
      {
         RPNCalculator calc = new RPNCalculator();

         Console.WriteLine(calc.Calculate("var1=0 4 2 5 * + 1 3 2 * + / - abs"));
         Console.WriteLine(calc.Calculate("var1 3 ^"));
      }
   }
}
