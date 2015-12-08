using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace FunctionsAndParsing
{
    public class Parser
    {
        private string expression;
        private int position;
        private char look;
        public readonly Dictionary<string, Func<double, double>> builtInFunctions
            = new Dictionary<string, Func<double, double>>();
        private Dictionary<string, int> variables
            = new Dictionary<string, int>();

        public void InitBuiltinFunctions()
        {
            builtInFunctions["abs"] = Math.Abs;
            builtInFunctions["acos"] = Math.Acos;
            builtInFunctions["acosh"] = MathNet.Numerics.Trig.Acosh;
            builtInFunctions["asin"] = Math.Asin;
            builtInFunctions["asinh"] = MathNet.Numerics.Trig.Asinh;
            builtInFunctions["atan"] = Math.Atan;
            builtInFunctions["atanh"] = MathNet.Numerics.Trig.Atanh;
            builtInFunctions["cbrt"] = x => Math.Pow(x, 1 / 3);
            builtInFunctions["ceil"] = Math.Ceiling;
            builtInFunctions["cos"] = Math.Cos;
            builtInFunctions["cosh"] = Math.Cosh;
            builtInFunctions["exp"] = Math.Exp;
            builtInFunctions["floor"] = Math.Floor;
            builtInFunctions["log"] = Math.Log;
            builtInFunctions["log10"] = Math.Log10;
            builtInFunctions["signum"] = x => Math.Sign(x);
            builtInFunctions["sin"] = Math.Sin;
            builtInFunctions["sinh"] = Math.Sinh;
            builtInFunctions["sqrt"] = Math.Sqrt;
            builtInFunctions["tan"] = Math.Tan;
            builtInFunctions["tanh"] = Math.Tanh;
            builtInFunctions["rtd"] = x => x / Math.PI * 180;
            builtInFunctions["dtr"] = x => x * Math.PI / 180;
        }

        public static int CharToInt(char c)
        {
            return Convert.ToInt32(c) - Convert.ToInt32('0');
        }

        private bool IsDigit(char c)
        {
            return char.IsDigit(c);
        }

        private bool IsAlpha(char c)
        {
            return char.IsLetter(c);
        }

        private static bool IsAddop(char c)
        {
            return "+-".Contains(c);
        }

        private bool IsMulop(char c)
        {
            return "*/".Contains(c);
        }
        private bool IsCaret(char c)
        {
            return c == '^';
        }

        private bool IsWhiteSpace(char c)
        {
            return Char.IsWhiteSpace(c);
        }

        private void Expected(string what)
        {
            throw new ParserException(what + " expected");
        }

        private void SkipWhiteSpace()
        {
            while (IsWhiteSpace(look))
            {
                GetChar();
            }
        }
        private double GetNum()
        {
            string result = "";
            if (!IsDigit(look)) Expected("Number in position " + position);
            while (IsDigit(look))
            {
                result += look;
                GetChar();
            }
            if (look == '.')
            {
                result += look;
                GetChar();

                while (IsDigit(look))
                {
                    result += look;
                    GetChar();
                }
            }
            SkipWhiteSpace();
            return double.Parse(result, CultureInfo.InvariantCulture.NumberFormat);
        }

        private string GetName()
        {
            string result = "";
            if (!IsAlpha(look)) Expected("Name");
            while (IsDigit(look) || IsAlpha(look))
            {
                result += look;
                GetChar();
            }
            SkipWhiteSpace();
            return result;
        }

        private char Read()
        {
            if (position < expression.Length)
                return expression[position++];
            return '\0';
        }

        private void GetChar()
        {
            look = Read();
        }

        private void Match(char c)
        {
            if (look == c)
            {
                GetChar();
                SkipWhiteSpace();
            }
            else
                Expected(string.Format("'{0}'", c));
        }

        private Parser(string expression, string[] variables)
        {
            InitBuiltinFunctions();
            this.expression = expression;
            if(variables!=null)
            for (int i = 0; i < variables.Length; i++)
            {
                this.variables[variables[i]] = i;
            }
            Reset();
        }

        public void Reset()
        {
            position = 0;
            GetChar();
            SkipWhiteSpace();
        }

        public CommonFunction Parse()
        {
            Reset();
            return ((CommonFunction)Expression());
        }


        //Получить выражение в виде графа
        private CommonFunction Expression()
        {
            Sum sum = new Sum();
            if (!IsAddop(look))
            {
                sum.operands.Add(Term());
                sum.signs.Add(true);
            }
            while (IsAddop(look))
            {
                switch (look)
                {
                    case '+':
                        Match('+');
                        sum.operands.Add(Term());
                        sum.signs.Add(true);
                        break;
                    case '-':
                        Match('-');
                        sum.operands.Add(Term());
                        sum.signs.Add(false);
                        break;
                    default:
                        throw new Exception();
                }
            }
            return sum;
        }

        private CommonFunction Term()
        {
            Mul mul = new Mul();
            mul.operands.Add(Factor());
            mul.powers.Add(true);
            while (IsMulop(look))
            {
                switch (look)
                {
                    case '*':
                        Match('*');
                        mul.operands.Add(Factor());
                        mul.powers.Add(true);
                        break;
                    case '/':
                        Match('/');
                        mul.operands.Add(Factor());
                        mul.powers.Add(false);
                        break;
                    default:
                        throw new Exception();
                }
            }
            return mul;
        }

        private CommonFunction Factor()
        {
            Pow pow = new Pow();
            pow.baseAndPower.Add(Power());
            while (IsCaret(look))
            {
                Match('^');
                pow.baseAndPower.Add(Power());
            }
            return pow;
        }

        private CommonFunction Power()
        {
            CommonFunction result = null;
            if (look == '(')
            {
                Match('(');
                result = Expression();
                Match(')');
                return result;              
            }
            if (IsAlpha(look))
            {
                var name = GetName();
                if (look == '(')
                {
                    Match('(');
                    result = new OneVarFunction(builtInFunctions[name],(CommonFunction)Expression());
                    Match(')');
                }
                else
                {
                    if (variables.ContainsKey(name))
                        result = new Variable(variables[name]);
                    else
                    {
                        variables[name] = variables.Last().Value + 1;
                        result = new Variable(variables[name]);
                    }
                    ((Variable)result).name = name;
                }
            }
            else
            {
                double constant=GetNum();
                result = new OneVarFunction(x => constant,null);
            }

            return result;
        }

        public static Func<double[], double> ParseExpression(string expression, string[] variables)
        {
            return new Parser(expression, variables).Parse().getCommonFunction();
        }

        public static CommonFunction ParseExpressionObject(string expression, string[] variables)
        {
            return new Parser(expression, variables).Parse();
        }
    }
}
