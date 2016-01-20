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
            string e = Console.ReadLine();
            FunctionsAndParsing.CommonFunction func = FunctionsAndParsing.Parser.ParseExpressionObject(e, new string[] { "x", "y", "z" });
            Console.Read();
        }
    }
}
