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
                if (i % 2 == 0)
                    values[i] = 2 * curStep.value[i] - preStep.value[i] + system(curStep.argument, curStep.value)[i + 1] * h * h;
                else
                    values[i] = (values[i - 1] - curStep.value[i - 1]) / h;
            }
            return new RungeKuttaMethod.Method.ValueAndArgument(values, curStep.argument + h);
        }

        public static List<RungeKuttaMethod.Method.ValueAndArgument> integrateEquationVector(RungeKuttaMethod.Method.ValueAndArgument begin, Func<double, double[], double[]> system, double h, Func<RungeKuttaMethod.Method.ValueAndArgument, bool> condition, double dataStep)
        {
            RungeKuttaMethod.Method.ValueAndArgument pre = begin;
            double[] twoStep = new double[begin.value.Length];
            for(int i = 0; i < twoStep.Length; i++)
            {
                if (i % 2 == 0)
                    twoStep[i] = begin.value[i] + begin.value[i + 1] * h + h * h * system(0, begin.value)[i + 1];
                else
                    twoStep[i] = (twoStep[i - 1] - begin.value[i - 1]) / h;
            }
            RungeKuttaMethod.Method.ValueAndArgument current = new RungeKuttaMethod.Method.ValueAndArgument(twoStep, h);
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

        public static RungeKuttaMethod.Method.ValueAndArgument nextStepWithSpeed(double h, RungeKuttaMethod.Method.ValueAndArgument preStep, Func<double, double[], double[]> system)
        {
            double[] values = new double[preStep.value.Length];
            for(int i = 0; i < values.Length;i++)
            {
                values[i] = preStep.value[i];
            }
            for (int i = 0; i < values.Length; i += 2)
            {       
                values[i] = preStep.value[i] + preStep.value[i + 1] * h + 0.5 * system(preStep.argument, preStep.value)[i + 1] * h * h;
            }
            for (int i = 1; i < values.Length; i += 2)
            {
                values[i] = preStep.value[i] + 0.5 * h * (system(preStep.argument, preStep.value)[i] + system(preStep.argument + h, values)[i]);
            }
            
            return new RungeKuttaMethod.Method.ValueAndArgument(values, preStep.argument + h);
        }

        public static List<RungeKuttaMethod.Method.ValueAndArgument> integrateEquationVectorWithSpeed(RungeKuttaMethod.Method.ValueAndArgument begin, Func<double, double[], double[]> system, double h, Func<RungeKuttaMethod.Method.ValueAndArgument, bool> condition, double dataStep)
        {
            RungeKuttaMethod.Method.ValueAndArgument pre = begin;                                              

            List<RungeKuttaMethod.Method.ValueAndArgument> result = new List<RungeKuttaMethod.Method.ValueAndArgument>();
            result.Add(begin);
            double step = 0;
            while (condition(pre))
            {
                step += h;
                RungeKuttaMethod.Method.ValueAndArgument next = nextStepWithSpeed(h, pre, system);
                if (step > dataStep)
                {
                    result.Add(next);
                    step = 0;
                }
                pre = next;      
            }
            return result;
        }
    }
}
