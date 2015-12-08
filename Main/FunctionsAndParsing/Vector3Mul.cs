using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsAndParsing
{
    public class Vector3Mul : VectorFunction
    {
        public Vector3Mul(VectorFunction v1, VectorFunction v2)
            : base(new List<CommonFunction>())
        {
            List<CommonFunction> x = new List<CommonFunction>();
            List<bool> x_ = new List<bool>();
            List<CommonFunction> x1 = new List<CommonFunction>();
            List<bool> x1_ = new List<bool>();

            x1.Add(v1.components[1]);
            x1_.Add(true);
            x1.Add(v2.components[2]);
            x1_.Add(true);
            Mul x1O = new Mul(x1, x1_);
            x.Add(x1O);
            x_.Add(true);

            List<CommonFunction> x2 = new List<CommonFunction>();
            List<bool> x2_ = new List<bool>();
            x2.Add(v1.components[2]);
            x2_.Add(true);
            x2.Add(v2.components[1]);
            x2_.Add(true);
            Mul x2O = new Mul(x2, x2_);
            x.Add(x2O);
            x_.Add(false);

            components.Add(new Sum(x, x_));

            List<CommonFunction> y = new List<CommonFunction>();
            List<bool> y_ = new List<bool>();
            List<CommonFunction> y1 = new List<CommonFunction>();
            List<bool> y1_ = new List<bool>();

            y1.Add(v1.components[2]);
            y1_.Add(true);
            y1.Add(v2.components[0]);
            y1_.Add(true);
            Mul y1O = new Mul(y1, y1_);
            y.Add(y1O);
            y_.Add(true);

            List<CommonFunction> y2 = new List<CommonFunction>();
            List<bool> y2_ = new List<bool>();
            y2.Add(v1.components[0]);
            y2_.Add(true);
            y2.Add(v2.components[2]);
            y2_.Add(true);
            Mul y2O = new Mul(y2, y2_);
            y.Add(y2O);
            y_.Add(false);

            components.Add(new Sum(y, y_));

            List<CommonFunction> z = new List<CommonFunction>();
            List<bool> z_ = new List<bool>();
            List<CommonFunction> z1 = new List<CommonFunction>();
            List<bool> z1_ = new List<bool>();

            z1.Add(v1.components[0]);
            z1_.Add(true);
            z1.Add(v2.components[1]);
            z1_.Add(true);
            Mul z1O = new Mul(z1, z1_);
            z.Add(z1O);
            z_.Add(true);

            List<CommonFunction> z2 = new List<CommonFunction>();
            List<bool> z2_ = new List<bool>();
            z2.Add(v1.components[1]);
            z2_.Add(true);
            z2.Add(v2.components[0]);
            z2_.Add(true);
            Mul z2O = new Mul(z2, z2_);
            z.Add(z2O);
            z_.Add(false);

            components.Add(new Sum(z, z_));
        }
    }
}
