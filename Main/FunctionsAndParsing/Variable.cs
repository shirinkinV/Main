using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsAndParsing
{
    public class Variable : CommonFunction
    {
        public int index;
        public string name;

        public Variable(int index)
        {
            this.index = index;
        }

        public override Func<double[], double> getCommonFunction()
        {
            return p => p[index];
        }

        public override Variable search(string name)
        {
            if (this.name == name)
            {
                return this;
            }
            else
            {
                return null;
            }
        }
    }
}
