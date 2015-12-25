using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RungeKuttaMethod
{
    public class Method
    {
        public static double[] MainMethod(int n, double a, double b, Func<double, double, double> yDer, double y0, double[] p1, double[][] p2, double[] p3)
        {
            double[] result = new double[n + 1];
            double h = (b - a) / n;
            result[0] = y0;
            double x_k = a;

            for (int k = 0; k < n; k++)
            {
                double dy = 0;
                double[] K_i_k = new double[p1.Length];
                for (int i = 0; i < p1.Length; i++)
                {
                    double sumForSecondArgument = 0;
                    for (int j = 0; j < i; j++)
                    {
                        sumForSecondArgument += p2[i][j] * K_i_k[j];
                    }
                    K_i_k[i] = h * yDer(x_k + p1[i] * h, result[k] + h * sumForSecondArgument);
                    dy += p3[i] * K_i_k[i];
                }

                result[k + 1] = result[k] + dy;
                x_k += h;
            }

            return result;
        }

        public static double[][] MainMethodVector(int n, double a, double b, Func<double, double[], double[]> system, double[] begin, double[] p1, double[][] p2, double[] p3)
        {
            double[][] result = new double[begin.Length][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[n + 1];
            }
            double h = (b - a) / n;
            for (int i = 0; i < result.Length; i++)
            {
                result[i][0] = begin[i];
            }
            double x_k = a;

            for (int k = 0; k < n; k++)
            {
                for (int p = 0; p < result.Length; p++)
                {
                    double dy = 0;
                    double[] K_i_k = new double[p1.Length];
                    for (int i = 0; i < p1.Length; i++)
                    {
                        double sumForSecondArgument = 0;
                        for (int j = 0; j < i; j++)
                        {
                            sumForSecondArgument += p2[i][j] * K_i_k[j];
                        }
                        double[] vectorY_k = new double[result.Length];
                        for (int q = 0; q < result.Length; q++)
                        {
                            vectorY_k[q] = result[q][k] + h * sumForSecondArgument;
                        }
                        K_i_k[i] = h * system(x_k + p1[i] * h, vectorY_k)[p];
                        dy += p3[i] * K_i_k[i];
                    }
                    result[p][k + 1] = result[p][k] + dy;
                }
                x_k += h;
            }

            return result;
        }

        public class NetVector
        {
            public List<double> parameters;
            public List<double>[] values;
            public NetVector(List<double> parameters, List<double>[] values)
            {
                this.parameters = parameters;
                this.values = values;
            }

        }

        public static NetVector MainMethodVector(double a, double b, Func<double, double[], double[]> system, double[] begin, double[] p1, double[][] p2, double[] p3)
        {
            List<double> parameters = new List<double>();
            List<double>[] values = new List<double>[begin.Length];
            for (int i = 0; i < values.Length; i++)
                values[i] = new List<double>();
            double h = 0.0001;

            for (int i = 0; i < values.Length; i++)
            {
                values[i].Add(begin[i]);
            }

            parameters.Add(a);
            while (parameters.Last() <= b)
            {
                double[] dyV = new double[values.Length];
                for (int p = 0; p < values.Length; p++)
                {
                    double dy = 0;
                    double[] K_i_k = new double[p1.Length];
                    for (int i = 0; i < p1.Length; i++)
                    {
                        double sumForSecondArgument = 0;
                        for (int j = 0; j < i; j++)
                        {
                            sumForSecondArgument += p2[i][j] * K_i_k[j];
                        }
                        double[] vectorY_k = new double[values.Length];
                        for (int q = 0; q < values.Length; q++)
                        {
                            vectorY_k[q] = values[q].Last() + h * sumForSecondArgument;
                        }
                        K_i_k[i] = h * system(parameters.Last() + p1[i] * h, vectorY_k)[p];
                        dy += p3[i] * K_i_k[i];
                    }
                    dyV[p] = dy;
                }
                for (int q = 0; q < values.Length; q++)
                {
                    values[q].Add(values[q].Last() + dyV[q]);
                }
                parameters.Add(parameters.Last() + h);
            }
            return new NetVector(parameters, values);
        }

        public static double[] Method3Order(int n, double a, double b, Func<double, double, double> yDer, double y0)
        {
            return MainMethod(n, a, b, yDer, y0,
                new double[] { 0, 1.0 / 3, 2.0 / 3 },
                new double[][] {
                    new double[] { 0, 0, 0 },
                    new double[] { 1.0 / 3, 0, 0 },
                    new double[] { 0, 2.0 / 3, 0 } },
                new double[] { 0.25, 0, 0.75 });
        }

        public static double[] Method4Order(int n, double a, double b, Func<double, double, double> yDer, double y0)
        {
            return MainMethod(n, a, b, yDer, y0,
                new double[] { 0, 0.5, 0.5, 1 },
                new double[][] {
                    new double[] { 0, 0, 0, 0 },
                    new double[] { 0.5, 0, 0, 0 },
                    new double[] { 0, 0.5, 0, 0 },
                    new double[] { 0, 0, 0.5, 0 }
                },
                new double[] { 1.0 / 6, 1.0 / 3, 1.0 / 3, 1.0 / 6 });
        }

        public static double[][] Method4OrderVector(int n, double a, double b, Func<double, double[], double[]> system, double[] begin)
        {
            return MainMethodVector(n, a, b, system, begin,
                new double[] { 0, 0.5, 0.5, 1 },
                new double[][] {
                    new double[] { 0, 0, 0, 0 },
                    new double[] { 0.5, 0, 0, 0 },
                    new double[] { 0, 0.5, 0, 0 },
                    new double[] { 0, 0, 0.5, 0 }
                },
                new double[] { 1.0 / 6, 1.0 / 3, 1.0 / 3, 1.0 / 6 });
        }

        public static NetVector Method4OrderVector(double a, double b, Func<double, double[], double[]> system, double[] begin)
        {
            return MainMethodVector(a, b, system, begin,
                new double[] { 0, 0.5, 0.5, 1 },
                new double[][] {
                    new double[] { 0, 0, 0, 0 },
                    new double[] { 0.5, 0, 0, 0 },
                    new double[] { 0, 0.5, 0, 0 },
                    new double[] { 0, 0, 0.5, 0 }
                },
                new double[] { 1.0 / 6, 1.0 / 3, 1.0 / 3, 1.0 / 6 });
        }



        //--------------------------new part

        public class ValueAndArgument
        {
            public double[] value;
            public double argument;
            public ValueAndArgument(double[] value, double argument)
            {
                this.value = value;
                this.argument = argument;
            }
        }

        public static ValueAndArgument nextStepVector(double h, ValueAndArgument preStep, Func<double, double[], double[]> system, double[] p1, double[][] p2, double[] p3)
        {                          
            double[] value = new double[preStep.value.Length];
            double[] dyV = new double[preStep.value.Length];
            for (int p = 0; p < value.Length; p++)
            {
                double dy = 0;
                double[] K_i_k = new double[p1.Length];
                for (int i = 0; i < p1.Length; i++)
                {
                    double sumForSecondArgument = 0;
                    for (int j = 0; j < i; j++)
                    {
                        sumForSecondArgument += p2[i][j] * K_i_k[j];
                    }
                    double[] vectorY_k = new double[value.Length];
                    for (int q = 0; q < value.Length; q++)
                    {
                        vectorY_k[q] = preStep.value[q] + h * sumForSecondArgument;
                    }
                    K_i_k[i] = h * system(preStep.argument + p1[i] * h, vectorY_k)[p];
                    dy += p3[i] * K_i_k[i];
                }
                dyV[p] = dy;
            }
            for(int q = 0; q < value.Length; q++)
            {
                value[q] = preStep.value[q] + dyV[q];
            }       
            return new ValueAndArgument(value,preStep.argument+ h);
        }

        public static ValueAndArgument nextStepVector4order(double h, ValueAndArgument preStep, Func<double, double[], double[]> system)
        {
            return nextStepVector(h, preStep, system,
                new double[] { 0, 0.5, 0.5, 1 },
                new double[][] {
                    new double[] { 0, 0, 0, 0 },
                    new double[] { 0.5, 0, 0, 0 },
                    new double[] { 0, 0.5, 0, 0 },
                    new double[] { 0, 0, 0.5, 0 }
                },
                new double[] { 1.0 / 6, 1.0 / 3, 1.0 / 3, 1.0 / 6 });
        }

        public static ValueAndArgument nextStep(double h, ValueAndArgument preStep, Func<double, double, double> derivative, double[] p1, double[][] p2, double[] p3)
        {
            return nextStepVector(h, preStep, (x, y) => new double[] { derivative(x, y[0]) }, p1, p2, p3);
        }

        public static ValueAndArgument nextStep4order(double h, ValueAndArgument preStep, Func<double, double, double> derivative, double epsilon)
        {
            return nextStep(h, preStep, derivative,
                new double[] { 0, 0.5, 0.5, 1 },
                new double[][] {
                    new double[] { 0, 0, 0, 0 },
                    new double[] { 0.5, 0, 0, 0 },
                    new double[] { 0, 0.5, 0, 0 },
                    new double[] { 0, 0, 0.5, 0 }
                },
                new double[] { 1.0 / 6, 1.0 / 3, 1.0 / 3, 1.0 / 6 });
        }

        public static List<ValueAndArgument> integrateEquationVector(ValueAndArgument begin, Func<double, double[], double[]> system, double epsilon, Func<ValueAndArgument,bool> condition, double dataStep)
        {
            List<ValueAndArgument> result = new List<ValueAndArgument>();
            result.Add(begin);
            ValueAndArgument current = begin;
            double h = 0.00001;
            double step = 0;
            while (condition(current))
            {
                ValueAndArgument y2h = nextStepVector4order(h, current, system);
                ValueAndArgument yh = nextStepVector4order(h / 2, current, system);
                if (difference(y2h.value, yh.value) / 15 < epsilon)
                {
                    step += h;
                    if (difference(y2h.value, yh.value) / 15 < 0.5 * epsilon) 
                    {
                        h *= 1.01;
                    }
                    
                    if (step > dataStep)
                    {
                        result.Add(y2h);
                        step = 0;
                    }
                    
                    current = y2h;
                }
                else
                {
                    h *= 0.9;
                }
            }
            return result;
        }

        public static List<ValueAndArgument> integrateEquationVector(ValueAndArgument begin, Func<double, double[], double[]> system, Func<ValueAndArgument, bool> condition)
        {
            List<ValueAndArgument> result = new List<ValueAndArgument>();
            result.Add(begin);
            ValueAndArgument current = begin;
            double h = 0.0000001;
            while (condition(current))
            {
                ValueAndArgument yh = nextStepVector4order(h, current, system);
                current = yh;
                result.Add(yh);
            }
            return result;
        }

        public static ValueAndArgument integrateEquationVectorTo(ValueAndArgument begin, Func<double, double[], double[]> system, double epsilon, Func<ValueAndArgument, bool> condition)
        {
            ValueAndArgument current = begin;
            double h = 0.00001;
            while (condition(current))
            {
                ValueAndArgument y2h = nextStepVector4order(h, current, system);
                ValueAndArgument yh = nextStepVector4order(h / 2, current, system);
                if (difference(y2h.value, yh.value) / 15 < epsilon)
                {
                    if (difference(y2h.value, yh.value) / 15 < 0.5 * epsilon)
                    {
                        h *= 1.2;
                    }  
                    current = y2h;
                }
                else
                {
                    h *= 0.9;
                }
            }
            return current;
        }

        public static double difference(double[] v1, double[] v2)
        {
            double result = 0;
            for (int i = 0; i < v1.Length; i++)
            {
                result += Math.Abs(v1[i] - v2[i]);
            }
            return result;
        }
    }
}
