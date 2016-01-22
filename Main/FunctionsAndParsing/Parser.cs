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
            return char.IsLetter(c)||c=='\'';
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
            if (variables != null)
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
            CommonFunction term = null;
            if (!IsAddop(look))
            {
                term = Term();
                if (!term.IsNull())
                {
                    sum.operands.Add(term);
                    sum.signs.Add(true);
                }
            }
            while (IsAddop(look))
            {

                switch (look)
                {
                    case '+':
                        Match('+');
                        term = Term();
                        if (!term.IsNull())
                        {
                            sum.operands.Add(term);
                            sum.signs.Add(true);
                        }
                        break;
                    case '-':
                        Match('-');
                        term = Term();
                        if (!term.IsNull())
                        {
                            sum.operands.Add(term);
                            sum.signs.Add(false);
                        }
                        break;
                    default:
                        throw new Exception();
                }
            }
            if (sum.operands.Count == 0)
            {
                return new OneVarFunction(x => 0, null, "0");
            }
            return sum;
        }

        private CommonFunction Term()
        {
            Mul mul = new Mul();
            CommonFunction factor = Factor();
            if (!factor.IsNull())
            {
                mul.operands.Add(factor);
                mul.powers.Add(true);
            }
            else {
                while (IsMulop(look))
                {
                    switch (look)
                    {
                        case '*':
                            Match('*'); break;
                        case '/':
                            Match('/'); break;
                    }
                    Factor();
                }        
                return new OneVarFunction(x => 0, null, "0");
            }
            while (IsMulop(look))
            {
                switch (look)
                {
                    case '*':
                        Match('*');
                        factor = Factor();
                        if (!factor.IsNull())
                        {
                            mul.operands.Add(factor);
                            mul.powers.Add(true);
                        }
                        else {
                            while (IsMulop(look))
                            {
                                switch (look)
                                {
                                    case '*':
                                        Match('*'); break;
                                    case '/':
                                        Match('/'); break;
                                }
                                Factor();
                            }
                            return new OneVarFunction(x => 0, null, "0");
                        }
                        break;
                    case '/':
                        Match('/');
                        factor = Factor();
                        if (!factor.IsNull())
                        {
                            mul.operands.Add(factor);
                            mul.powers.Add(false);
                        }
                        else
                            throw new ArithmeticException("null devision");
                        break;
                    default:
                        throw new Exception();
                }
            }
            if (mul.operands.Count == 0) return new OneVarFunction(x => 0, null, "0");
            if (mul.operands.Count == 1) return mul.operands[0];
            return mul;
        }

        private CommonFunction Factor()
        {
            Pow pow = new Pow();
            CommonFunction power = Power();
            if (!power.IsNull())
            {
                pow.baseAndPower.Add(power);
            }
            else {
                while (IsCaret(look))
                {
                    Match('^');
                    Power();
                }
                return new OneVarFunction(x => 0, null, "0");
            }
            while (IsCaret(look))
            {
                Match('^');
                power = Power();
                if (!power.IsNull())
                {
                    pow.baseAndPower.Add(power);
                }
                else
                {
                    if (pow.baseAndPower.Count > 1)
                    {
                        pow.baseAndPower.RemoveAt(pow.baseAndPower.Count - 1);
                        while (IsCaret(look))
                        {
                            Match('^');
                            Power();
                        }
                        return pow;
                    }
                    else
                    {
                        while (IsCaret(look))
                        {
                            Match('^');
                            Power();
                        }
                        return new OneVarFunction(x => 1, null, "1");
                    }
                }
            }
            if (pow.baseAndPower.Count == 1)
            {
                return pow.baseAndPower[0];
            }
            else
            {
                return pow;
            }
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
                    CommonFunction arg = (CommonFunction)Expression();
                    result = new OneVarFunction(builtInFunctions[name], arg, name + "(argument)");
                    Match(')');
                }
                else
                {
                    if (variables.ContainsKey(name))
                        result = new Variable(variables[name],name);
                    else
                    {
                        if (name == "PI" || name == "pi")
                        {
                            return new OneVarFunction(x => Math.PI, null, "PI");
                        }
                        if (name == "e" || name == "E")
                        {
                            return new OneVarFunction(x => Math.E, null, "E");
                        }
                        variables[name] = variables.Count != 0 ? variables.Last().Value + 1 : 0;
                        result = new Variable(variables[name],name);
                    }
                    ((Variable)result).name = name;
                }
            }
            else
            {
                double constant = GetNum();
                result = new OneVarFunction(x => constant, null, "" + constant);
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
