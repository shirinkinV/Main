using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsAndParsing
{
    public class Curve : VectorFunction
    {
        public Curve(List<CommonFunction> trajectory) : base(trajectory) { }

        public Func<double, double[]> trajectory()
        {
            return t => func(new double[] { t });
        }
    }
}
