using System;
using System.Collections.Generic;
using System.Linq;

namespace FunctionsAndParsing
{
    public class Pow : CommonFunction
    {
        public List<CommonFunction> baseAndPower;

        public Pow() { baseAndPower = new List<CommonFunction>(); }

        public Pow(List<CommonFunction> baseAndPower)
        {
            this.baseAndPower = baseAndPower;
        }

        private double compute(double[] p)
        {
            if (baseAndPower.Count == 1) return baseAndPower[0].getFunction()(p)[0];
            else
            {
                double result = baseAndPower[baseAndPower.Count - 1].getFunction()(p)[0];
                for (int i = baseAndPower.Count - 2; i >= 0; i--)
                {
                    result = Math.Pow(baseAndPower[i].getFunction()(p)[0], result);
                }
                return result;
            }
        }

        public override Func<double[], double> getCommonFunction()
        {
            return p => compute(p);
        }

        public override Variable search(string name)
        {
            Variable result = null;
            for(int i = 0; i < baseAndPower.Count; i++)
            {
                if (result == null)
                {
                    result = baseAndPower.ElementAt(i).search(name);
                }
                else
                {
                    return result;
                }
            }
            return result;
        }

        public override bool IsNull()
        {
            if (baseAndPower.Count == 0) return true;
            return baseAndPower[0].IsNull();
        }

        public override string print()
        {
            string result = "";
            if (baseAndPower[0] is Mul || baseAndPower[0] is Sum)
                result += "(" + baseAndPower[0].print() + ")";
            else
                result += baseAndPower[0].print();
            for (int i = 1; i < baseAndPower.Count; i++)
            {
                if (i != 0) result += "^";
                if(baseAndPower[i] is Mul||baseAndPower[i] is Sum)
                {
                    result += "(" + baseAndPower[i].print() + ")";
                }
                else
                {
                    result += baseAndPower[i].print();
                }
            }
            return result;
        }
    }
}
