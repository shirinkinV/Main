using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlotView
{
    public class FunctionAppearance
    {
        public Func<double, double> func = null;
        public float lineWidth = 1;
        public int color = 0;
        public double a = -1, b = 1;
        public ushort mask;

        public FunctionAppearance(Func<double, double> func, int color, double a, double b, float lineWidth, ushort mask)
        {
            this.func = func;
            this.color = color;
            this.lineWidth = lineWidth;
            this.a = a;
            this.b = b;
            this.mask = mask; 
        }
    }
}
