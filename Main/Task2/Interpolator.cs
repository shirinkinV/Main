using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Interpolation;

namespace Task2
{
    public class Interpolator
    {
        public static Func<double, double> interpolate(List<RungeKuttaMethod.Method.ValueAndArgument> list, int index)
        {
            double[] values = new double[list.Count];
            double[] arguments = new double[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                values[i] = list[i].value[index];
                arguments[i] = list[i].argument;
            }
            CubicSpline spline = CubicSpline.InterpolateAkima(arguments, values);
            return x_var => spline.Interpolate(x_var);
        }
    }
}
