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
        double R=0.5;
        double alpha_0=Math.PI/2.7;
        double l = 2;
        double g = 9.8;
        Func<double, double> x1_func;
        Func<double, double> x2_func;
        double T;
        double time=-1;
        long millis = 0;

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

        void init()
        {
            Func<double, double, double, double, double, double> d2x1ondt2 = (q_var, x1ont, x2ont_var, x1, x2_var) => (q_var * (x1ont - 2 * x2ont_var) * x1ont + g * (1 - q_var) * (x1 - l)) / (x1 - l - q_var * (x1 - x2_var));
            Func<double, double> x2 = x1 => (l * l - R * R - x1 * x1) / (2 * (l - x1));
            Func<double, double, double, double> x2ont = (x1, x2_var, x1ont) => x1ont * (x1 - x2_var) / (x1 - l);
            Func<double, double, double> sin_alpha = (x1, x2_var) => (x1 - x2_var) / Math.Sqrt((x1 - x2_var) * (x1 - x2_var) + R * R);
            Func<double, double> q = sin_alpha_var => sin_alpha_var / (1 - sin_alpha_var);
            Func<double, double, double> f = (x1, x1ont) => d2x1ondt2(q(sin_alpha(x1, x2(x1))), x1ont, x2ont(x1, x2(x1), x1ont), x1, x2(x1));
            double x1_0 = (l - R / Math.Cos(alpha_0)) + R * Math.Tan(alpha_0);
            double x1ont_0 = 0;
            x1_func = Interpolator.interpolate(Method.integrateEquationVector(
                new Method.ValueAndArgument(new double[] { x1_0, x1ont_0 }, 0),
                (t, x) => new double[] { x[1], f(x[0], x[1]) },
                1e-7, value => {
                    T = value.argument;
                    if (Method.difference(value.value, new double[] { x1_0, x1ont_0 }) < 1e-5 && T > 1e-4)
                        return false;
                    else
                        return true;
                }));
            x2_func = t => x2(x1_func(t));
            //label.Content = "" + (Math.Abs(x_func(0.4)-x_func(0)))+'\n'+t1;
            plot.addFunction(new PlotView.FunctionAppearance(x1_func, 0xff0000, 0, T, 2, 0xffff), "x1");
            plot.addFunction(new PlotView.FunctionAppearance(x2_func, 0x0000ff, 0, T, 2, 0xffff), "x2");
            plot.addFunction(new PlotView.FunctionAppearance(MathNet.Numerics.Differentiate.DerivativeFunc(x1_func, 1), 0x00ff00, 0, T, 2, 0xffff), "x1'");
            plot.addFunction(new PlotView.FunctionAppearance(t => x2ont(
                x1_func(t), x2_func(t), MathNet.Numerics.Differentiate.DerivativeFunc(x1_func, 1)(t)
                ), 0x00ffff, 0, T, 2, 0xffff), "x2'");
        }

        private void capture_Resized_1(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.Viewport(0, 0, (int)capture.ActualWidth, (int)capture.ActualHeight);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            double ratio = capture.ActualWidth / Math.Max(1, capture.ActualHeight);

            gl.Frustum(-0.1 * ratio, 0.1 * ratio, -0.1, 0.1, 0.1, 10);
        }

        private void capture_OpenGLDraw_1(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            if (time == -1)
            {
                millis = DateTime.Now.Ticks;
                time = 0;
            }
            else
            {
                long newMillis = DateTime.Now.Ticks;
                time += 0.1 * (double)(newMillis - millis) / 1000000;
                millis = newMillis;
                while (time > T)
                    time -= T;
            }

            OpenGL gl = args.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.Translate(0, 0, -2);
            gl.Rotate(45, 1, 0, 0);

            gl.LineWidth(2);
            gl.Color(1.0, 0, 0);
            drawCircle(gl, R, 1 - x1_func(time));

            gl.LineWidth(1);
            gl.Color(0.0, 0, 1);
            drawCircle(gl, 0.01, 1 - x2_func(time));

            gl.Color(0, 0, 0);
            gl.Begin(OpenGL.GL_LINES);
            double angle1 = 0, angle2 = Math.PI / 3 * 2, angle3 = -Math.PI / 3 * 2;
            gl.Vertex4d(0.01 * Math.Cos(angle1), 1, 0.01 * Math.Sin(angle1), 1);
            gl.Vertex4d(0.01 * Math.Cos(angle1), 1 - x2_func(time), 0.01 * Math.Sin(angle1), 1);
            gl.Vertex4d(0.01 * Math.Cos(angle2), 1, 0.01 * Math.Sin(angle2), 1);
            gl.Vertex4d(0.01 * Math.Cos(angle2), 1 - x2_func(time), 0.01 * Math.Sin(angle2), 1);
            gl.Vertex4d(0.01 * Math.Cos(angle3), 1, 0.01 * Math.Sin(angle3), 1);
            gl.Vertex4d(0.01 * Math.Cos(angle3), 1 - x2_func(time), 0.01 * Math.Sin(angle3), 1);

            gl.Vertex4d(R * Math.Cos(angle1), 1 - x1_func(time), R * Math.Sin(angle1), 1);
            gl.Vertex4d(0.01 * Math.Cos(angle1), 1 - x2_func(time), 0.01 * Math.Sin(angle1), 1);
            gl.Vertex4d(R * Math.Cos(angle2), 1 - x1_func(time), R * Math.Sin(angle2), 1);
            gl.Vertex4d(0.01 * Math.Cos(angle2), 1 - x2_func(time), 0.01 * Math.Sin(angle2), 1);
            gl.Vertex4d(R * Math.Cos(angle3), 1 - x1_func(time), R * Math.Sin(angle3), 1);
            gl.Vertex4d(0.01 * Math.Cos(angle3), 1 - x2_func(time), 0.01 * Math.Sin(angle3), 1);

            gl.End();

            gl.Flush();
        }

        private void capture_OpenGLInitialized_1(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.ClearColor(1, 1, 1, 1);
            gl.Disable(OpenGL.GL_LIGHTING);
            init();
        }
    }
}
