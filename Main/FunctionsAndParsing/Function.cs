using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsAndParsing
{
    public interface Function
    {
        Func<double[], double[]> getFunction();
        Variable search(string name);
    }
}
