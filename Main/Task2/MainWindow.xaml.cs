﻿using System;
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
        double T;
        double time=-1;
        long ticks = 0;
        double dist = 0;
        double epsilon = 1e-5;
        bool initialized = true;
        double period = 20;

        Func<double, double> P;
        Func<double, double> K;
        Func<double, double> E;


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
                   var ddtx2deronx1der = -Rsquare*p2/p1cube;
                   var ddtx2deronx3der = -ddtx2deronx1der;

                   var p4 = x2onx1;
                   var p5 = p4 * x2deronx3der;
                   var p6 = p4 * x2deronx1der;
                   var p7 = x2onx3;
                   var p8 = 1 + x2deronx3der * p7;
                   var p9 = x2deronx1der * p7;
                   var p10 = Rsquare * p2 * p2 / p1cube * x2deronx3der - x2der * ddtx2deronx3der + x2der * x2deronx3 + g * (1 + x2onx3);
                   var p11 = Rsquare * p2 * p2 / p1cube * x2deronx1der - x2der * ddtx2deronx1der + x2der * x2deronx1 - c / m * x1 + g * x2onx1;
                   var p12 = (p5 * p11 - p6 * p10) / (p5*p9-p6*p8);
                   var p13 = (p10 - p8 * p12) / p5;

                   return new double[] { x[1], p13, x[3], p12 };
               };

            double x10 = dist;
            double x1d0 = 0;
            double x30 = (l - R / Math.Cos(alpha_0)) + R * Math.Tan(alpha_0);
            double x3d0 = 0;

            /*List<Method.ValueAndArgument> list = Method.integrateEquationVector(
                new Method.ValueAndArgument(new double[] { x10, x1d0, x30, x3d0 }, 0), system, epsilon,
                value =>
                {
                    T = value.argument;
                    
                    return T <= period;
                }, 0.001); */ 
            double[] begin = new double[] { x10, x1d0, x30, x3d0 };
            double[] postBegin = new double[] { x10 + x1d0 * epsilon, x1d0 + system(0, begin)[1] * epsilon, x30 + x3d0 * epsilon, x3d0 + system(0, begin)[3] * epsilon };
            List<Method.ValueAndArgument> list = VerletMethod.Method.integrateEquationVectorWithSpeed(
                new Method.ValueAndArgument(begin, 0), new Method.ValueAndArgument(postBegin, epsilon), system, epsilon,
                value =>
                {
                    T = value.argument;

                    return T <= period;
                },0.001);

            x1_func = Interpolator.interpolate(list, 0);
            Func<double, double> x1_der = Interpolator.interpolate(list, 1);
            x3_func = Interpolator.interpolate(list, 2);
            Func<double, double> x3_der = Interpolator.interpolate(list, 3);
            x2_func = t => 0.5 * (l + x1_func(t) + x3_func(t)) - Rsquare / (2 * (l + x1_func(t) - x3_func(t)));

            Func<double, double> x2_der = t =>
            {
                var x1der = x1_der(t);
                var x3der = x3_der(t);
                var x1 = x1_func(t);
                var x3 = x3_func(t); 
                return 0.5 * (x1der + x3der) + Rsquare * (x1der - x3der) / (2 * (l + x1 - x3)*(l+x1- x3));
            };
            double P0 = -g * m * (x2_func(0) + x3_func(0))+0.5*c*x1_func(0)*x2_func(0);
            P = t =>
            {
                var x1 = x1_func(t);
                return -m * g * (x2_func(t) + x3_func(t)) + 0.5*c * x1 * x1 - P0;
            };
            K = t =>
            {                          
                var x2der = x2_der(t);
                var x3der = x3_der(t);   
                return 0.5 * m * (x2der*x2der+x3der*x3der);
            };
            E = t => P(t) + K(t); ;  
            
            initialized = true;
            if (plot.graphics.functions.Count != 0)
            {
                plot.graphics.functions.ElementAt(0).func = x1_func;
                plot.graphics.functions.ElementAt(1).func = x2_func;
                plot.graphics.functions.ElementAt(2).func = x3_func;
                plot.graphics.functions.ElementAt(3).func = P;
                plot.graphics.functions.ElementAt(4).func = K;
                plot.graphics.functions.ElementAt(5).func = E;
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
            epsilon= FunctionsAndParsing.Parser.ParseExpression(epsilon_box.Text, null)(null);
            period= FunctionsAndParsing.Parser.ParseExpression(T_box.Text, null)(null);
            Task task = new Task(init);
            task.Start();
        }
    }
}
