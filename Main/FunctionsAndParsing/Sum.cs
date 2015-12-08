using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsAndParsing
{
    public class Sum : CommonFunction
    {
        public List<CommonFunction> operands;
        public List<bool> signs;

        public Sum()
        {
            operands = new List<CommonFunction>();
            signs = new List<bool>();
        }

        public Sum(List<CommonFunction> operands, List<bool> signs)
        {
            this.operands = operands;
            this.signs = signs;
        }

        private double func(double[] p)
        {
            double result = 0;
            for (int i = 0; i < operands.Count; i++)
            {
                if (signs[i])
                {
                    result += operands[i].getFunction()(p)[0];
                }
                else
                {
                    result -= operands[i].getFunction()(p)[0];
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
            Variable result = null;

            for (int i = 0; i < operands.Count; i++)
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
