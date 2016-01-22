using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParserTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "functions:\n" +
                "abs\n" +
                "acos\n" +
                "acosh\n" +
                "asin\n" +
                "asinh\n" +
                "atan\n" +
                "atanh\n" +
                "cbrt\n" +
                "ceil\n" +
                "cos\n" +
                "cosh\n" +
                "exp\n" +
                "floor\n" +
                "log\n" +
                "log10\n" +
                "signum\n" +
                "sin\n" +
                "sinh\n" +
                "sqrt\n" +
                "tan\n" +
                "tanh\n" +
                "rtd\n" +
                "dtr"
                );
            while (true)
            {
                Console.Write("expression: ");
                string e = Console.ReadLine();
                FunctionsAndParsing.CommonFunction func = FunctionsAndParsing.Parser.ParseExpressionObject(e, null);
                string function = func.print();
                Console.WriteLine("reduced: " + function);
                Console.Write("variable: ");
                string v = Console.ReadLine();
                string derivative = StringDerivative.Derivative.GetDerivative(function, v);
                Console.WriteLine("derivative on "+v+": " + derivative);
                FunctionsAndParsing.CommonFunction der = FunctionsAndParsing.Parser.ParseExpressionObject(derivative, null);
                derivative = der.print();
                Console.WriteLine("reduced: " + derivative);
                Console.WriteLine("reduced twice: " + FunctionsAndParsing.Parser.ParseExpressionObject(derivative, null).print());
                Console.WriteLine();
            }
        }
    }
}
