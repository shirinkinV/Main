using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpGL;
using RungeKuttaMethod;

namespace Task1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        double
            J1 = 2.5,
            R1 = 1,
            V2 = 1,
            m2 = 0.9,
            c4 = 2,
            d4 = 0.5,
            alpha = Math.PI*0.1,
            beta = Math.PI*0.4,
            m3 = 0.3,
            c5 = 1,
            d5 = 0.5,
            g = 9.8;

        double time = -1;
        double T = 0;
        long ticks = 0;

        Func<double, double> x_func;
        Func<double, double> x_der;
        Func<double, double> x_func_numeric;     
        Func<double, double> x_der_numeric;
        Func<double, double> K;
        Func<double, double> P;
        Func<double, double> E;
        Func<double, double> K_numeric;
        Func<double, double> P_numeric;
        Func<double, double> E_numeric;

        bool initialized = true;
        double period=1000;
        double epsilon = 1e-5;


        void init()
        {
            if (initialized == false) { return; }
            initialized = false;
            double a = J1 / (R1 * R1) + m2 + m3;
            double b = c4 + c5;
            double c = g * (m3 * Math.Sin(beta) - m2 * Math.Sin(alpha)) + c5 * d5 - c4 * d4;
            T = Math.PI * 2 * Math.Sqrt(a / b);
            x_func = t => -c / b * Math.Cos(Math.Sqrt(b / a) * t) + V2 * Math.Sqrt(a / b) * Math.Sin(Math.Sqrt(b / a) * t) + c / b;
            x_der = t => c / b * Math.Sqrt(b / a) * Math.Sin(Math.Sqrt(b / a) * t) + V2 * Math.Cos(Math.Sqrt(b / a) * t);
            var x1p = d4 + x_func(0);
            var x2p = d5 - x_func(0);
            double P0 = 0.5 * (c4 * x1p * x1p + c5 * x2p * x2p) + m2 * g * x_func(0) * Math.Sin(alpha) - m3 * g * x_func(0) * Math.Sin(beta);
            P = t =>
            {
                double x1 = d4 + x_func(t);
                double x2 = d5 - x_func(t);
                return 0.5 * (c4 * x1 * x1 + c5 * x2 * x2) + m2 * g * x_func(t) * Math.Sin(alpha) - m3 * g * x_func(t) * Math.Sin(beta)-P0;
            };
            var xdp = x_der(0);
            double K0 = 0.5 * ((J1 / (R1 * R1) + m2 + m3) * (xdp * xdp));
            K = t =>
            {
                var xd = x_der(t);
                return 0.5 * ((J1 / (R1 * R1) + m2 + m3) * (xd * xd)) - K0;
            };
            E = t => P(t) + K(t);

            //Func<double, double[], double[]> system = (t, x) => new double[] { x[1], -b / a * x[0] + c / a };
            Func<double,double[], double[]> system = (t, x) => new double[] { x[1], -b / a * x[0] + c / a  };
            //double[] begin = new double[] { 0, V2 };
            double[] begin = new double[] { 0, V2 };
            double h = 0.001;         
            /*List<Method.ValueAndArgument> list = Method.integrateEquationVector(
                new Method.ValueAndArgument(begin, 0), system, epsilon,
                value =>
                {
                    T = value.argument;
                    return T <= period;
                }, 0.001);*/
            List<Method.ValueAndArgument> list = VerletMethod.Method.integrateEquationVector(
                new Method.ValueAndArgument(begin, 0), system, h,
                value =>
                {
                    T = value.argument;
                    return T <= period;
                }, 0.001);
            //x_func_numeric = Task2.Interpolator.interpolate(list, 0);
            //x_der_numeric = Task2.Interpolator.interpolate(list, 1);
            x_func_numeric = Task2.Interpolator.interpolate(list, 0);
            x_der_numeric = Task2.Interpolator.interpolate(list, 1);



            double x2pn = d5 - x_func_numeric(0); 
            double x1pn = d4 + x_func_numeric(0);
            double P0N= 0.5 * (c4 * x1pn * x1pn + c5 * x2pn * x2pn) + m2 * g * x_func_numeric(0) * Math.Sin(alpha) - m3 * g * x_func_numeric(0) * Math.Sin(beta);
            P_numeric = t =>
            {
                double x1 = d4 + x_func_numeric(t);
                double x2 = d5 - x_func_numeric(t);
                return 0.5 * (c4 * x1 * x1 + c5 * x2 * x2) + m2 * g * x_func_numeric(t) * Math.Sin(alpha) - m3 * g * x_func_numeric(t) * Math.Sin(beta) - P0N;
            };
            var xdn = x_der_numeric(0);
            var K0N = 0.5 * ((J1 / (R1 * R1) + m2 + m3) * (xdn * xdn));
            K_numeric = t =>
            {
                var xd = x_der_numeric(t);
                return 0.5 * ((J1 / (R1 * R1) + m2 + m3) * (xd * xd))-K0N;
            };
            E_numeric = t => P_numeric(t) + K_numeric(t);

            var ePeriod = E_numeric(period) - E(period);
            initialized = true;
            if (plot.graphics.functions.Count != 0)
            {
                plot.graphics.functions.ElementAt(0).func = x_func;
                plot.graphics.functions.ElementAt(1).func = K;
                plot.graphics.functions.ElementAt(2).func = P;
                plot.graphics.functions.ElementAt(3).func = E;
                plot.graphics.functions.ElementAt(4).func = x_func_numeric;
                plot.graphics.functions.ElementAt(5).func = K_numeric;
                plot.graphics.functions.ElementAt(6).func = P_numeric;
                plot.graphics.functions.ElementAt(7).func = E_numeric;
                plot.graphics.functions.ElementAt(8).func = x_der;
                plot.graphics.functions.ElementAt(9).func = x_der_numeric;

                plot.graphics.functions.ElementAt(0).b = T;
                plot.graphics.functions.ElementAt(1).b = T;
                plot.graphics.functions.ElementAt(2).b = T;
                plot.graphics.functions.ElementAt(3).b = T;
                plot.graphics.functions.ElementAt(4).b = T;
                plot.graphics.functions.ElementAt(5).b = T;
                plot.graphics.functions.ElementAt(6).b = T;
                plot.graphics.functions.ElementAt(7).b = T;
                plot.graphics.functions.ElementAt(8).b = T;
                plot.graphics.functions.ElementAt(9).b = T;

            }

        }

        void drawCircle(OpenGL gl, double radius, double x, double y, double phi)
        {
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int i = 0; i < 180; i++)
            {
                gl.Vertex4d(x + radius * Math.Cos(Math.PI / 180 * i * 2), y + radius * Math.Sin(Math.PI / 180 * i * 2), 0, 1);
            }
            gl.End();
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex4d(x, y, 0, 1);
            gl.Vertex4d(x + radius * Math.Cos(phi), y + Math.Sin(phi), 0, 1);
            gl.End();
        }

        void drawBody(OpenGL gl, double x0, double y0, double phi, double sprLen, double sprRadius, double bodySize, double bodyLen)
        {   

            gl.Translate(x0, y0, 0);
            gl.Rotate(phi-90,0,0,1);
            gl.Begin(OpenGL.GL_LINE_STRIP);
            for (int i = 0; i < 900; i++)
            {
                gl.Vertex4d(sprRadius * Math.Cos(Math.PI / 180 * i * 4), (double)( (sprLen) / 900 * i), sprRadius * Math.Sin(Math.PI / 180 * i * 4), 1);
            }
            gl.End();

            gl.Begin(OpenGL.GL_LINES);

            gl.Vertex4d(-0.7*bodySize,0,0,1);
            gl.Vertex4d(0.7 * bodySize, 0, 0, 1);
            gl.Vertex4d(-0.7 * bodySize, 0, 0, 1);
            gl.Vertex4d(-0.7 * bodySize, 2, 0, 1);
            gl.Vertex4d(0.7 * bodySize, 0, 0, 1);
            gl.Vertex4d(0.7 * bodySize, 2, 0, 1);

            gl.Vertex4d(-bodySize/2, sprLen, 0, 1);
            gl.Vertex4d(bodySize/2, sprLen, 0, 1);
            gl.Vertex4d(-bodySize/2, sprLen, 0, 1);
            gl.Vertex4d(-bodySize/2, sprLen+bodyLen, 0, 1);
            gl.Vertex4d(bodySize/2, sprLen, 0, 1);
            gl.Vertex4d(bodySize/2, sprLen+bodyLen, 0, 1);
            gl.Vertex4d(-bodySize/2, sprLen + bodyLen, 0, 1);
            gl.Vertex4d(bodySize/2, sprLen + bodyLen, 0, 1);


            gl.End();
            gl.Rotate(-phi + 90, 0, 0, 1);
            gl.Translate(-x0, -y0, 0);
            
        }

        

        private void capture_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            if (time == -1)
            {
                ticks = DateTime.Now.Ticks;
                time = 0;
            }
            else
            {
                long newTicks = DateTime.Now.Ticks;
                time += 1 * (double)(newTicks - ticks) / 10000000;
                ticks = newTicks;
                while (time > T)
                    time -= T;
            }

            OpenGL gl = args.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.Translate(0, 0, -1);
            gl.LineWidth(2);

            gl.Color(0, 0, 0, 1);
            drawCircle(gl, R1, 0, 0, -x_func_numeric(time) / R1);

            double bodyLen = 1;
            double beginSprLen = 1;

            drawBody(gl, -2 - R1 / Math.Sin(alpha), -2 * Math.Tan(alpha), alpha / Math.PI * 180, beginSprLen + d4 + x_func_numeric(time), 0.1, 1, bodyLen);

            gl.Begin(OpenGL.GL_LINES);

            gl.Vertex4d(-2 - R1 / Math.Sin(alpha) + (beginSprLen + d4 + x_func_numeric(time) + bodyLen) * Math.Cos(alpha), -2 * Math.Tan(alpha) + (beginSprLen + d4 + x_func_numeric(time) + bodyLen) * Math.Sin(alpha), 0, 1);
            gl.Vertex4d(-R1 * Math.Sin(alpha), R1 * Math.Cos(alpha), 0, 1);

            gl.Vertex4d(2 + R1 / Math.Sin(beta) - (beginSprLen + d5 - x_func_numeric(time) + bodyLen) * Math.Cos(beta), -2 * Math.Tan(beta) + (beginSprLen + d5 - x_func_numeric(time) + bodyLen) * Math.Sin(beta), 0, 1);
            gl.Vertex4d(R1 * Math.Sin(beta), R1 * Math.Cos(beta), 0, 1);

            
            gl.End();
            drawBody(gl, 2 + R1 / Math.Sin(beta), -2 * Math.Tan(beta), 180 - beta / Math.PI * 180, beginSprLen + d5 - x_func_numeric(time), 0.1, 1, bodyLen);



            gl.Flush();
        }

        private void capture_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {                        
            OpenGL gl = args.OpenGL;
            gl.ClearColor(1, 1, 1, 1);
            gl.Disable(OpenGL.GL_LIGHTING);
            init();
            plot.addFunction(new PlotView.FunctionAppearance(x_func, 0xff0000, 0, T, 1, 0xffff), "x");
            plot.addFunction(new PlotView.FunctionAppearance(K, 0x00AA00, 0, T, 1, 0xffff), "T");
            plot.addFunction(new PlotView.FunctionAppearance(P, 0x0000ff, 0, T, 1, 0xffff), "П");
            plot.addFunction(new PlotView.FunctionAppearance(E, 0xff00ff, 0, T, 3, 0xffff), "E");

            plot.addFunction(new PlotView.FunctionAppearance(x_func_numeric, 0xff0000, 0, T, 1, 0xf0f0), "x_n");
            plot.addFunction(new PlotView.FunctionAppearance(K_numeric, 0x00AA00, 0, T, 1, 0xf0f0), "T_n");
            plot.addFunction(new PlotView.FunctionAppearance(P_numeric, 0x0000ff, 0, T, 1, 0xf0f0), "П_n");
            plot.addFunction(new PlotView.FunctionAppearance(E_numeric, 0x000000, 0, T, 3, 0xf0f0), "E_n");

            plot.addFunction(new PlotView.FunctionAppearance(x_der, 0x7f3060, 0, T, 1, 0xffff), "x'");
            plot.addFunction(new PlotView.FunctionAppearance(x_der_numeric, 0x7f3060, 0, T, 1, 0xf0f0), "x'_n");
        }

        private void capture_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.Viewport(0, 0, (int)capture.ActualWidth, (int)capture.ActualHeight);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            double ratio = capture.ActualWidth / Math.Max(1, capture.ActualHeight);

            //gl.Frustum(-0.1 * ratio, 0.1 * ratio, -0.1, 0.1, 0.1, 10);
            gl.Ortho(-8 * ratio, 8 * ratio, -8, 8, 0.5, 10);
        }
    }
}
