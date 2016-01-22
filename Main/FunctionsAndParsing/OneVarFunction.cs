using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsAndParsing
{
    public class OneVarFunction : CommonFunction
    {
        public CommonFunction arg;

        public Func<double, double> function = p => Double.NaN;

        public string performance;

        public OneVarFunction(Func<double, double> function, CommonFunction arg, string performance)
        {
            if (performance == "") throw new ArgumentException("empty string of performance");
            if (performance.IndexOf("argument") == -1 && arg != null)
            {
                throw new ArgumentException("argument not found");
            }
            this.performance = performance;
            this.function = function;
            this.arg = arg;
        }

        public override Func<double[], double> getCommonFunction()
        {
            if (arg != null)
                return p => function(arg.getCommonFunction()(p));
            else
                return p => function(0);
        }

        public override bool IsNull()
        {
            if (arg == null)
            {
                if (function(0) == 0) return true;
                else return false;
            }
            else
                return false;
        }

        public override string print()
        {
            string result = performance;
            if (arg != null)
            {
                int indexOfArgument = result.IndexOf("argument");
                result = result.Remove(indexOfArgument, 8);
                if (result[indexOfArgument - 1] == '(' && result[indexOfArgument] == ')')
                    result = result.Insert(indexOfArgument, arg.print());
                else
                    result = result.Insert(indexOfArgument, "(" + arg.print() + ")");
            }
            return result;
        }

        public override Variable search(string name)
        {
            return arg.search(name);
        }
    }
}
