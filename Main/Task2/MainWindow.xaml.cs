using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RungeKuttaMethod;
using SharpGL;

namespace Task2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double R=1;
        double alpha_0=Math.PI/3;
        double l = 6;
        double g = 9.8;
        double m = 1;
        double c = 100;

        Func<double, double> x1_func;
        Func<double, double> x2_func;
        Func<double, double> x3_func;

        Func<double, double> x1_func_2v;
        Func<double, double> x2_func_2v;
        Func<double, double> x3_func_2v;

        double T;
        double time=-1;
        long ticks = 0;
        double dist = 0;
        double h1 = 1e-3;
        double h2 = 1e-5;
        bool initialized = true;
        double period = 20;

        Func<double, double> P;
        Func<double, double> K;
        Func<double, double> E;

        Func<double, double> P_2v;
        Func<double, double> K_2v;
        Func<double, double> E_2v;

        bool verletFlag;


        public MainWindow()
        {
            InitializeComponent();              
        }

        

        

        void drawCircle(OpenGL gl, double radius, double y)
        {
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for(int i = 0; i < 180; i++)
            {
                gl.Vertex4d(radius*Math.Cos(Math.PI/180*i*2), y, radius * Math.Sin(Math.PI / 180 * i*2), 1);
            }
            gl.End();
        }

        void drawSpring(OpenGL gl, double radius, double y1,double y2)
        {
            gl.Begin(OpenGL.GL_LINE_STRIP);
            for (int i = 0; i < 900; i++)
            {
                gl.Vertex4d(radius * Math.Cos(Math.PI / 180 * i * 4), (double)(y1+(y2- y1)/900*i), radius * Math.Sin(Math.PI / 180 * i * 4), 1);
            }
            gl.End();
        }

        void init()
        {
            if(initialized== false) { return; }
            initialized = false;
            var Rsquare = R * R;
            //x1 - x[0], x1' - x[1], x3 - x[2], x3' - x[3]
            Func<double, double[], double[]> system = (t, x) =>
               {
                   var x3 = x[2];
                   var x3der = x[3];
                   var x1der = x[1];
                   var x1 = x[0];

                   var p1 = l + x1 - x3;
                   var p2 = x1der - x3der;
                   var p1cube = p1 * p1 * p1;
                   var p1square = p1 * p1;

                   var x2 = 0.5 * (l + x1 + x3) - Rsquare / (2 * p1);
                   var x2der = 0.5 * (x1der + x3der) - Rsquare * p2 / (2 * p1square);
                   var x2deronx1 = -Rsquare * p2 / p1cube;
                   var x2deronx3 = -x2deronx1;
                   var x2onx1 = 0.5 + Rsquare / (2 * p1square);
                   var x2onx3 = 0.5 - Rsquare / (2 * p1square);
                   var x2deronx1der = x2onx1;
                   var x2deronx3der = x2onx3;
                   var ddtx2deronx1der = -Rsquare * p2 / p1cube;
                   var ddtx2deronx3der = -ddtx2deronx1der;

                   var p4 = x2onx1;
                   var p5 = p4 * x2deronx3der;
                   var p6 = p4 * x2deronx1der;
                   var p7 = x2onx3;
                   var p8 = 1 + x2deronx3der * p7;
                   var p9 = x2deronx1der * p7;
                   var p10 = Rsquare * p2 * p2 / p1cube * x2deronx3der - x2der * ddtx2deronx3der + x2der * x2deronx3 + g * (1 + x2onx3);
                   var p11 = Rsquare * p2 * p2 / p1cube * x2deronx1der - x2der * ddtx2deronx1der + x2der * x2deronx1 - c / m * x1 + g * x2onx1;
                   var p12 = (p5 * p11 - p6 * p10) / (p5 * p9 - p6 * p8);
                   var p13 = (p10 - p8 * p12) / p5;

                   return new double[] { x[1], p13, x[3], p12 };
               };
            double x10 = dist;
            double x1d0 = 0;
            double x20 = (l - R / Math.Cos(alpha_0));
            double x2d0 = 0;
            double x30 = (l - R / Math.Cos(alpha_0)) + R * Math.Tan(alpha_0);
            double x3d0 = 4;

            List<Method.ValueAndArgument> listRunge = Method.integrateEquationVector(h1,
                new Method.ValueAndArgument(new double[] { x10, x1d0, x30, x3d0 }, 0), system,
                value =>
                {
                    T = value.argument;
                    
                    return T <= period;
                }, 0.001);

            Func<double, double[], double[]> system2 = (t, x) =>
                {
                    var x2 = x[0];
                    var x3 = x[2];
                    var x1 = x2 - l + Math.Sqrt(Rsquare + (x3 - x2) * (x3 - x2));

                    var x1onx2 = 1 - (x3 - x2) / (Math.Sqrt(Rsquare + (x3 - x2) * (x3 - x2)));
                    var x1onx3 = (x3 - x2) / (Math.Sqrt(Rsquare + (x3 - x2) * (x3 - x2)));
                    var p1 = -c / m * x1 * x1onx2 + g;
                    var p2 = -c / m * x1 * x1onx3 + g;

                    return new double[] {x[1],p1,x[3],p2};
                };

            double[] begin = new double[] { x20, x2d0, x30, x3d0 };
            
            List<Method.ValueAndArgument> listVerlet = VerletMethod.Method.integrateEquationVectorWithSpeed(
                new Method.ValueAndArgument(begin, 0),  system2, h2,
                value =>
                {
                    T = value.argument;
                    if (RungeKuttaMethod.Method.difference(begin, value.value) < 1e-3&&T>0.1)
                    {
                        return false;
                    }
                    return T <= period;
                },0.001);

            x1_func = Interpolator.interpolate(listRunge, 0);
            Func<double, double> x1_der = Interpolator.interpolate(listRunge, 1);
            x3_func = Interpolator.interpolate(listRunge, 2);
            Func<double, double> x3_der = Interpolator.interpolate(listRunge, 3);
            x2_func = t => 0.5 * (l + x1_func(t) + x3_func(t)) - Rsquare / (2 * (l + x1_func(t) - x3_func(t)));

            x2_func_2v = Interpolator.interpolate(listVerlet, 0);
            Func<double, double> x2_der_2v = Interpolator.interpolate(listVerlet, 1);
            x3_func_2v = Interpolator.interpolate(listVerlet, 2);
            Func<double, double> x3_der_2v = Interpolator.interpolate(listVerlet, 3);
            x1_func_2v = t => Math.Sqrt(Math.Pow(x3_func_2v(t)- x2_func_2v(t),2)+Rsquare)-l+x2_func_2v(t);
             


            Func<double, double> x2_der = t =>
            {
                var x1der = x1_der(t);
                var x3der = x3_der(t);
                var x1 = x1_func(t);
                var x3 = x3_func(t); 
                return 0.5 * (x1der + x3der) + Rsquare * (x1der - x3der) / (2 * (l + x1 - x3)*(l+x1- x3));
            };


            double P0 = -g * m * (x2_func(0) + x3_func(0)) + 0.5 * c * x1_func(0) * x2_func(0);
            P = t =>
            {
                var x1 = x1_func(t);
                return -m * g * (x2_func(t) + x3_func(t)) + 0.5 * c * x1 * x1 - P0;
            };

            double P02V = -g * m * (x2_func_2v(0) + x3_func_2v(0)) + 0.5 * c * x1_func_2v(0) * x1_func_2v(0);
            P_2v = t =>
            {
                var x1 = x1_func_2v(t);
                return -m * g * (x2_func_2v(t) + x3_func_2v(t)) + 0.5 * c * x1 * x1 - P02V;
            };

                double K0 = 0.5 * m * (x2_der(0) * x2_der(0) + x3_der(0) * x3_der(0));
            K = t =>
            {                          
                var x2der = x2_der(t);
                var x3der = x3_der(t);   
                return 0.5 * m * (x2der*x2der+x3der*x3der);
            };
            E = t => P(t) + K(t); ;

            double K02V = 0.5 * m * (x2_der(0) * x2_der(0) + x3_der(0) * x3_der(0));
            K_2v = t =>
            {
                var x2der = x2_der_2v(t);
                var x3der = x3_der_2v(t);
                return 0.5 * m * (x2der * x2der + x3der * x3der);
            };
            E_2v = t => P_2v(t) + K_2v(t);


            initialized = true;
            if (plot.graphics.functions.Count != 0)
            {
                if (!verletFlag)
                {  
                    plot.graphics.functions.ElementAt(0).func = x1_func;
                    plot.graphics.functions.ElementAt(1).func = x2_func;
                    plot.graphics.functions.ElementAt(2).func = x3_func;
                    plot.graphics.functions.ElementAt(3).func = P;
                    plot.graphics.functions.ElementAt(4).func = K;
                    plot.graphics.functions.ElementAt(5).func = E;
                }
                else
                {
                    plot.graphics.functions.ElementAt(0).func = x1_func_2v;
                    plot.graphics.functions.ElementAt(1).func = x2_func_2v;
                    plot.graphics.functions.ElementAt(2).func = x3_func_2v;
                    plot.graphics.functions.ElementAt(3).func = P_2v;
                    plot.graphics.functions.ElementAt(4).func = K_2v;
                    plot.graphics.functions.ElementAt(5).func = E_2v;
                }   
                plot.graphics.functions.ElementAt(0).b = T;
                plot.graphics.functions.ElementAt(1).b = T;
                plot.graphics.functions.ElementAt(2).b = T;
                plot.graphics.functions.ElementAt(3).b = T;
                plot.graphics.functions.ElementAt(4).b = T;
                plot.graphics.functions.ElementAt(5).b = T;
            }
        }

        private void capture_Resized_1(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.Viewport(0, 0, (int)capture.ActualWidth, (int)capture.ActualHeight);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            double ratio = capture.ActualWidth / Math.Max(1, capture.ActualHeight);

            //gl.Frustum(-0.1 * ratio, 0.1 * ratio, -0.1, 0.1, 0.1, 10);
            gl.Ortho(-5.1 * ratio, 5.1 * ratio, -5.1, 5.1, 0.5,10);
        }

        private void capture_OpenGLDraw_1(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            if (!initialized)
            {
                gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
                gl.MatrixMode(OpenGL.GL_MODELVIEW);
                gl.LoadIdentity();
                gl.Translate(0, -0, -2);
                gl.LineWidth(4);
                gl.Color(0.0, 1, 0);
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex4d(-2, 0, 0, 1);
                gl.Vertex4d(-2 + 4 * T / period, 0, 0, 1);
                gl.Color(1.0, 0, 0);
                gl.Vertex4d(-2 + 4 * T / period, 0, 0, 1);
                gl.Vertex4d(2, 0, 0, 1);
                gl.End();
                gl.Flush();
                System.Threading.Thread.Sleep(100);
                return;
            }
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

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.Translate(0, 3, -2);
            gl.Rotate(10, 1, 0, 0);
            if (!verletFlag)
            { 
                gl.LineWidth(1);
                gl.Color(0, 0, 0);
                gl.Begin(OpenGL.GL_LINES);
                double angle1 = 0, angle2 = Math.PI / 3 * 2, angle3 = -Math.PI / 3 * 2;
                gl.Vertex4d(0.05 * Math.Cos(angle1), 1 - x1_func(time), 0.05 * Math.Sin(angle1), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle1), 1 - x2_func(time), 0.05 * Math.Sin(angle1), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle2), 1 - x1_func(time), 0.05 * Math.Sin(angle2), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle2), 1 - x2_func(time), 0.05 * Math.Sin(angle2), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle3), 1 - x1_func(time), 0.05 * Math.Sin(angle3), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle3), 1 - x2_func(time), 0.05 * Math.Sin(angle3), 1);

                gl.Vertex4d(R * Math.Cos(angle1), 1 - x3_func(time), R * Math.Sin(angle1), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle1), 1 - x2_func(time), 0.05 * Math.Sin(angle1), 1);
                gl.Vertex4d(R * Math.Cos(angle2), 1 - x3_func(time), R * Math.Sin(angle2), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle2), 1 - x2_func(time), 0.05 * Math.Sin(angle2), 1);
                gl.Vertex4d(R * Math.Cos(angle3), 1 - x3_func(time), R * Math.Sin(angle3), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle3), 1 - x2_func(time), 0.05 * Math.Sin(angle3), 1);

                gl.End();

                gl.LineWidth(2);
                gl.Color(1.0, 0, 0);
                drawCircle(gl, R, 1 - x3_func(time));

                gl.Color(0.0, 1, 0);
                drawCircle(gl, 0.05, 1 - x2_func(time));

                gl.LineWidth(1);
                gl.Color(0.0, 0, 1);
                drawSpring(gl, 0.05, 2, 1 - x1_func(time));

                gl.Color(0, 0, 0);
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex4d(-1, 2, 0, 1);
                gl.Vertex4d(1, 2, 0, 1);
                gl.End();
            }
            else
            {
                gl.LineWidth(1);
                gl.Color(0, 0, 0);
                gl.Begin(OpenGL.GL_LINES);
                double angle1 = 0, angle2 = Math.PI / 3 * 2, angle3 = -Math.PI / 3 * 2;
                gl.Vertex4d(0.05 * Math.Cos(angle1), 1 - x1_func_2v(time), 0.05 * Math.Sin(angle1), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle1), 1 - x2_func_2v(time), 0.05 * Math.Sin(angle1), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle2), 1 - x1_func_2v(time), 0.05 * Math.Sin(angle2), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle2), 1 - x2_func_2v(time), 0.05 * Math.Sin(angle2), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle3), 1 - x1_func_2v(time), 0.05 * Math.Sin(angle3), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle3), 1 - x2_func_2v(time), 0.05 * Math.Sin(angle3), 1);

                gl.Vertex4d(R * Math.Cos(angle1), 1 - x3_func_2v(time), R * Math.Sin(angle1), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle1), 1 - x2_func_2v(time), 0.05 * Math.Sin(angle1), 1);
                gl.Vertex4d(R * Math.Cos(angle2), 1 - x3_func_2v(time), R * Math.Sin(angle2), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle2), 1 - x2_func_2v(time), 0.05 * Math.Sin(angle2), 1);
                gl.Vertex4d(R * Math.Cos(angle3), 1 - x3_func_2v(time), R * Math.Sin(angle3), 1);
                gl.Vertex4d(0.05 * Math.Cos(angle3), 1 - x2_func_2v(time), 0.05 * Math.Sin(angle3), 1);

                gl.End();

                gl.LineWidth(2);
                gl.Color(1.0, 0, 0);
                drawCircle(gl, R, 1 - x3_func_2v(time));

                gl.Color(0.0, 1, 0);
                drawCircle(gl, 0.05, 1 - x2_func_2v(time));

                gl.LineWidth(1);
                gl.Color(0.0, 0, 1);
                drawSpring(gl, 0.05, 2, 1 - x1_func_2v(time));

                gl.Color(0, 0, 0);
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex4d(-1, 2, 0, 1);
                gl.Vertex4d(1, 2, 0, 1);
                gl.End();
            }  

            gl.Flush();
        }

        private void capture_OpenGLInitialized_1(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.ClearColor(1, 1, 1, 1);
            gl.Disable(OpenGL.GL_LIGHTING);
            init();
            plot.addFunction(new PlotView.FunctionAppearance(x1_func, 0x0000aa, 0, T, 3, 0xffff), "x1");
            plot.addFunction(new PlotView.FunctionAppearance(x2_func, 0x008800, 0, T, 3, 0xffff), "x2");
            plot.addFunction(new PlotView.FunctionAppearance(x3_func, 0x660000, 0, T, 3, 0xffff), "x3");
            plot.addFunction(new PlotView.FunctionAppearance(P, 0x777700, 0, T, 2, 0x0f0f), "П");
            plot.addFunction(new PlotView.FunctionAppearance(K, 0x00aaaa, 0, T, 2, 0x0fff), "T");
            plot.addFunction(new PlotView.FunctionAppearance(E, 0x000000, 0, T, 4, 0xf7b8), "E");
        }

        private void compute_Click(object sender, RoutedEventArgs e)
        {
            alpha_0 = FunctionsAndParsing.Parser.ParseExpression(alpha_box.Text, null)(null);
            m = FunctionsAndParsing.Parser.ParseExpression(m_box.Text, null)(null);
            c= FunctionsAndParsing.Parser.ParseExpression(c_box.Text, null)(null);
            h1= FunctionsAndParsing.Parser.ParseExpression(epsilon_box.Text, null)(null);
            period= FunctionsAndParsing.Parser.ParseExpression(T_box.Text, null)(null);
            h2 = FunctionsAndParsing.Parser.ParseExpression(h_box.Text, null)(null);
            Task task = new Task(init);
            task.Start();
        }

        private void runge_Checked(object sender, RoutedEventArgs e)
        {
            verletFlag = false;
            if (plot.graphics.functions.Count != 0)
            {
                plot.graphics.functions.ElementAt(0).func = x1_func;
                plot.graphics.functions.ElementAt(1).func = x2_func;
                plot.graphics.functions.ElementAt(2).func = x3_func;
                plot.graphics.functions.ElementAt(3).func = P;
                plot.graphics.functions.ElementAt(4).func = K;
                plot.graphics.functions.ElementAt(5).func = E;
            }
        }

        private void verlet_Checked(object sender, RoutedEventArgs e)
        {
            verletFlag = true;
            if (plot.graphics.functions.Count != 0)
            {
                plot.graphics.functions.ElementAt(0).func = x1_func_2v;
                plot.graphics.functions.ElementAt(1).func = x2_func_2v;
                plot.graphics.functions.ElementAt(2).func = x3_func_2v;
                plot.graphics.functions.ElementAt(3).func = P_2v;
                plot.graphics.functions.ElementAt(4).func = K_2v;
                plot.graphics.functions.ElementAt(5).func = E_2v;
            }
        }
    }
}
