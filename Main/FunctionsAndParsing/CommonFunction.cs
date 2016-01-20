using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsAndParsing
{
    public abstract class CommonFunction : Function
    {
        public static CommonFunction operator +(CommonFunction f1, CommonFunction f2)
        {
            Sum result = new Sum();

            if (f1 is Sum)
            {
                result.operands = ((Sum)f1).operands;
                result.signs = ((Sum)f1).signs;
            }
            else
            {
                result.operands.Add(f1);
                result.signs.Add(true);
            }
            if (f2 is Sum)
            {
                result.operands.AddRange(((Sum)f2).operands);
                result.signs.AddRange(((Sum)f2).signs);
            }
            else
            {
                result.operands.Add(f2);
                result.signs.Add(true);
            }
            return result;
        }

        public static CommonFunction operator -(CommonFunction f1, CommonFunction f2)
        {
            return f1 + (-f2);
        }

        public static CommonFunction operator -(CommonFunction f1)
        {
            Sum result = new Sum();
            if (f1 is Sum)
            {
                result = (Sum)f1;
                for (int i = 0; i < result.signs.Count; i++)
                {
                    result.signs[i] = !result.signs[i];
                }
            }
            else
            {
                result.operands.Add(f1);
                result.signs.Add(false);
            }
            return result;
        }

        public static CommonFunction operator *(CommonFunction f1, CommonFunction f2)
        {
            Mul result = new Mul();
            if (f1 is Mul)
            {
                result.operands = ((Mul)f1).operands;
                result.powers = ((Mul)f1).powers;
            }
            else
            {
                result.operands.Add(f1);
                result.powers.Add(true);
            }
            if (f2 is Mul)
            {
                result.operands.AddRange(((Mul)f2).operands);
                result.powers.AddRange(((Mul)f2).powers);
            }
            else
            {
                result.operands.Add(f2);
                result.powers.Add(true);
            }
            return result;
        }

        public static CommonFunction operator /(CommonFunction f1, CommonFunction f2)
        {
            Mul result = new Mul();
            if (f1 is Mul)
            {
                result.operands = ((Mul)f1).operands;
                result.powers = ((Mul)f1).powers;
            }
            else
            {
                result.operands.Add(f1);
                result.powers.Add(true);
            }
            if (f2 is Mul)
            {
                result.operands.AddRange(((Mul)f2).operands);
                foreach (bool flag in ((Mul)f2).powers)
                {
                    result.powers.Add(!flag);
                }
            }
            else
            {
                result.operands.Add(f2);
                result.powers.Add(false);
            }
            return result;
        }

        public Pow pow(CommonFunction f1)
        {
            Pow result = new Pow();
            result.baseAndPower.Add(this);
            result.baseAndPower.Add(f1);
            return result;
        }

        public abstract Func<double[], double> getCommonFunction();

        public abstract Variable search(string name);

        public Func<double[], double[]> getFunction()
        {
            return p => new double[] { getCommonFunction()(p) };
        }

        public abstract bool IsNull();
        public abstract string print();
    }
}
