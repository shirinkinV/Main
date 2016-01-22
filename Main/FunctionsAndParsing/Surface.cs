using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Differentiation;

namespace FunctionsAndParsing
{
    public class Surface
    {
        private VectorFunction r, normal;

        public Surface(VectorFunction r)
        {
            this.r = r;

            List<CommonFunction> components = r.components;
            List<DefinedCommonFunction> derivativesU = new List<DefinedCommonFunction>();
            List<DefinedCommonFunction> derivativesV = new List<DefinedCommonFunction>();
            NumericalDerivative derivative = new NumericalDerivative();
            derivativesU.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[0].getCommonFunction(), 0, 1), "null"));
            derivativesV.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[0].getCommonFunction(), 1, 1), "null"));
            derivativesU.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[1].getCommonFunction(), 0, 1), "null"));
            derivativesV.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[1].getCommonFunction(), 1, 1), "null"));
            derivativesU.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[2].getCommonFunction(), 0, 1), "null"));
            derivativesV.Add(new DefinedCommonFunction(derivative.CreatePartialDerivativeFunctionHandle(components[2].getCommonFunction(), 1, 1), "null"));

            VectorFunction v1 = new VectorFunction(derivativesU);
            VectorFunction v2 = new VectorFunction(derivativesV);

            VectorFunction normalLength = new Vector3Mul(v1, v2);
            CommonFunction length = new Length(normalLength);

            List<bool> normalPow = new List<bool>();
            normalPow.Add(true); normalPow.Add(false);

            List<CommonFunction> normalx = new List<CommonFunction>();
            normalx.Add(normalLength.components[0]); normalx.Add(length);

            List<CommonFunction> normaly = new List<CommonFunction>();
            normaly.Add(normalLength.components[1]); normaly.Add(length);

            List<CommonFunction> normalz = new List<CommonFunction>();
            normalz.Add(normalLength.components[2]); normalz.Add(length);

            List<CommonFunction> normalCoordinates = new List<CommonFunction>();
            normalCoordinates.Add(new Mul(normalx, normalPow));
            normalCoordinates.Add(new Mul(normaly, normalPow));
            normalCoordinates.Add(new Mul(normalz, normalPow));

            normal = new VectorFunction(normalCoordinates);
        }
    }
}
