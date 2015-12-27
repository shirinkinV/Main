using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerletMethod
{
    public class Method
    {
        public static RungeKuttaMethod.Method.ValueAndArgument nextStep(double h, RungeKuttaMethod.Method.ValueAndArgument preStep, RungeKuttaMethod.Method.ValueAndArgument curStep, Func<double, double[], double[]> system)
        {
            double[] values = new double[preStep.value.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = 2 * curStep.value[i] - preStep.value[i] + system(curStep.argument, curStep.value)[i] * h * h;
            }
            return new RungeKuttaMethod.Method.ValueAndArgument(values, curStep.argument + h);
        }

        public static List<RungeKuttaMethod.Method.ValueAndArgument> integrateEquationVector(RungeKuttaMethod.Method.ValueAndArgument begin, RungeKuttaMethod.Method.ValueAndArgument postBegin, Func<double, double[], double[]> system, double h, Func<RungeKuttaMethod.Method.ValueAndArgument, bool> condition, double dataStep)
        {
            RungeKuttaMethod.Method.ValueAndArgument pre = begin;
            RungeKuttaMethod.Method.ValueAndArgument current = postBegin;
            List<RungeKuttaMethod.Method.ValueAndArgument> result = new List<RungeKuttaMethod.Method.ValueAndArgument>();
            result.Add(begin);
            double step = 0;
            while (condition(current))
            {
                step += h;
                RungeKuttaMethod.Method.ValueAndArgument next = nextStep(h, pre, current, system);
                if (step > dataStep)
                {
                    result.Add(next);
                    step = 0;
                }
                pre = current;
                current = next;
            }
            return result;
        }

        public static RungeKuttaMethod.Method.ValueAndArgument nextStepWithSpeed(double h, RungeKuttaMethod.Method.ValueAndArgument preStep, RungeKuttaMethod.Method.ValueAndArgument curStep, Func<double, double[], double[]> system)
        {
            double[] values = new double[preStep.value.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (i % 2 == 0)
                    values[i] = 2 * curStep.value[i] - preStep.value[i] + system(curStep.argument, curStep.value)[i + 1] * h * h;
                else
                    values[i] = (values[i - 1] - curStep.value[i - 1]) / h;
            }
            return new RungeKuttaMethod.Method.ValueAndArgument(values, curStep.argument + h);
        }

        public static List<RungeKuttaMethod.Method.ValueAndArgument> integrateEquationVectorWithSpeed(RungeKuttaMethod.Method.ValueAndArgument begin, RungeKuttaMethod.Method.ValueAndArgument postBegin, Func<double, double[], double[]> system, double h, Func<RungeKuttaMethod.Method.ValueAndArgument, bool> condition, double dataStep)
        {
            RungeKuttaMethod.Method.ValueAndArgument pre = begin;
            RungeKuttaMethod.Method.ValueAndArgument current = postBegin;
            List<RungeKuttaMethod.Method.ValueAndArgument> result = new List<RungeKuttaMethod.Method.ValueAndArgument>();
            result.Add(begin);
            double step = 0;
            while (condition(current))
            {
                step += h;
                RungeKuttaMethod.Method.ValueAndArgument next = nextStepWithSpeed(h, pre, current, system);
                if (step > dataStep)
                {
                    result.Add(next);
                    step = 0;
                }
                pre = current;
                current = next;
            }
            return result;
        }
    }
}
