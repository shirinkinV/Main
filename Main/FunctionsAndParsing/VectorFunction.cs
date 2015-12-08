using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsAndParsing
{
    public class VectorFunction : Function
    {
        public readonly List<CommonFunction> components = new List<CommonFunction>();

        public VectorFunction(List<CommonFunction> components)
        {
            this.components = components;
        }

        public VectorFunction(List<DefinedCommonFunction> components)
        {
            this.components = new List<CommonFunction>();
            for (int i = 0; i < components.Count; i++)
            {
                this.components.Add(components[i]);
            }
        }

        protected double[] func(double[] p)
        {
            if (components.Count == 0)
            {
                return null;
            }
            double[] result = new double[components.Count];
            for (int i = 0; i < components.Count; i++)
            {
                result[i] = components[i].getCommonFunction()(p);
            }
            return result;
        }

        public Func<double[], double[]> getFunction()
        {
            return func;
        }

        public Variable search(string name)
        {
            Variable result = null;
            for(int i = 0; i < components.Count; i++)
            {
                if (result == null)
                {
                    result = components.ElementAt(i).search(name);
                }
                else
                {
                    return result;
                }
            }
            return result;
        }
    }
}
