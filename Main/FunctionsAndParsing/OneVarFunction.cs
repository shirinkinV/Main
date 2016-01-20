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

        public OneVarFunction(Func<double, double> function, CommonFunction arg)
        {
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

        public override Variable search(string name)
        {
            return arg.search(name);
        }
    }
}
