using System;

namespace Program
{
   class Program 
   {
     public static void Main(string[] args)
     {
        Console.WriteLine(RPNCalc.Calculate("5 1 2 + 4 * + 3 -"));
     }
   }
}
