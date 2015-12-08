using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsAndParsing
{
    public class Mul : CommonFunction
    {
        public List<CommonFunction> operands;
        public List<bool> powers;

        public Mul()
        {
            operands = new List<CommonFunction>();
            powers = new List<bool>();
        }

        public Mul(List<CommonFunction> operands, List<bool> powers)
        {
            this.operands = operands;
            this.powers = powers;
        }

        private double func(double[] p)
        {
            double result = operands[0].getFunction()(p)[0];
            for (int i = 1; i < operands.Count; i++)
            {
                if (powers[i])
                {
                    result *= operands[i].getFunction()(p)[0];
                }
                else
                {
                    result /= operands[i].getFunction()(p)[0];
                }
            }
            return result;
        }

        public override Func<double[], double> getCommonFunction()
        {
            return func;
        }

        public override Variable search(string name)
        {
            Variable result=null;
            for(int i = 0; i < operands.Count; i++)
            {
                if (result == null)
                {
                    result = operands.ElementAt(i).search(name);
                }
                else
                {
                    return result;
                }
            }
            return result;
        }
    }
}
