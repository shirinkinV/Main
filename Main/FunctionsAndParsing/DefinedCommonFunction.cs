using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsAndParsing
{
    public class DefinedCommonFunction : CommonFunction
    {
        protected Func<double[], double> function;

        public DefinedCommonFunction(Func<double[], double> function)
        {
            this.function = function;
        }

        public override Func<double[], double> getCommonFunction()
        {
            return function;
        }

        public override bool IsNull()
        {
            return false;
        }

        public override Variable search(string name)
        {
            return null;
        }
    }
}
