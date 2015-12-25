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
            V2 = 0,
            m2 = 1.5,
            c4 = 5,
            d4 = 0.5,
            alpha = Math.PI / 3,
            beta = Math.PI / 6,
            m3 = 2,
            c5 = 4,
            d5 = 0.5,
            g = 9.8;

        double time = -1;
        double T = 0;
        long ticks = 0;

        Func<double, double> x_func;

        void init()
        {
            double a = J1 / (R1 * R1) + m2 + m3;
            double b = c4 + c5;
            double c = g * (m3 * Math.Sin(beta) - m2 * Math.Sin(alpha)) + c5 * d5 - c4 * d4;
            T = Math.PI * 2 * Math.Sqrt(a / b);
            x_func = t => -c / b * Math.Cos(Math.Sqrt(b / a) * t) + V2 * Math.Sqrt(a / b) * Math.Sin(Math.Sqrt(b / a) * t) + c / b;
            Func<double, double> x_der = t => c / b * Math.Sqrt(b / a) * Math.Sin(Math.Sqrt(b / a) * t) + V2 * Math.Cos(Math.Sqrt(b / a) * t);
            Func<double, double> P = t =>
            {
                double x1 = d4 + x_func(t);
                double x2 = d5 - x_func(t);
                return 0.5 * (c4 * x1 * x1 + c5 * x2 * x2) + m2 * g * x_func(t) * Math.Sin(alpha) - m3 * g * x_func(t) * Math.Sin(beta);
            };
            Func<double, double> K = t =>
            {
                var xd = x_der(t);
                return 0.5 * ((J1 / (R1 * R1) + m2 + m3) * (xd * xd));
            };
            Func<double, double> E = t => P(t) + K(t);

            plot.addFunction(new PlotView.FunctionAppearance(x_func, 0xff0000, 0, T, 1, 0xffff), "x");       
            plot.addFunction(new PlotView.FunctionAppearance(K, 0x00ff00, 0, T, 1, 0xffff), "T");
            plot.addFunction(new PlotView.FunctionAppearance(P, 0x0000ff, 0, T, 1, 0xffff), "П");
            plot.addFunction(new PlotView.FunctionAppearance(E, 0xff00ff, 0, T, 1, 0xffff), "E");
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

            gl.Color(0, 0, 0, 1);
            drawCircle(gl, R1, 0, 0, -x_func(time) / R1);

            double bodyLen = 1;
            double beginSprLen = 1;

            drawBody(gl, -4 - R1 / Math.Sin(alpha), -4 * Math.Tan(alpha), alpha / Math.PI * 180, beginSprLen + d4 + x_func(time), 0.1, 1, bodyLen);

            gl.Begin(OpenGL.GL_LINES);

            gl.Vertex4d(-4 - R1 / Math.Sin(alpha) + (beginSprLen + d4 + x_func(time) + bodyLen) * Math.Cos(alpha), -4 * Math.Tan(alpha) + (beginSprLen + d4 + x_func(time) + bodyLen) * Math.Sin(alpha), 0, 1);
            gl.Vertex4d(-R1 * Math.Sin(alpha), R1 * Math.Cos(alpha), 0, 1);

            gl.Vertex4d(3 + R1 / Math.Sin(beta) - (beginSprLen + d5 - x_func(time) + bodyLen) * Math.Cos(beta), -3 * Math.Tan(beta) + (beginSprLen + d5 - x_func(time) + bodyLen) * Math.Sin(beta), 0, 1);
            gl.Vertex4d(R1 * Math.Sin(beta), R1 * Math.Cos(beta), 0, 1);

            
            gl.End();
            drawBody(gl, 3 + R1 / Math.Sin(beta), -3 * Math.Tan(beta), 180 - beta / Math.PI * 180, beginSprLen + d5 - x_func(time), 0.1, 1, bodyLen);



            gl.Flush();
        }

        private void capture_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {                        
            OpenGL gl = args.OpenGL;
            gl.ClearColor(1, 1, 1, 1);
            gl.Disable(OpenGL.GL_LIGHTING);
            init();
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
