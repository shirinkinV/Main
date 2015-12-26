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
using SharpGL;
using Sketches; 

namespace PlotView
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class PlotView : UserControl
    {
        static Color f = Color.FromRgb(0xff, 0, 0);
        static Color y = Color.FromRgb(0, 0xff, 0);
        static Color yder = Color.FromRgb(0, 0, 0xff);
        public Grapics graphics;
        public List<Label> names=new List<Label>();

        public PlotView()
        {
            InitializeComponent();
            glControl.Focus();
            graphics = new Grapics(-20, 20, 40, 40);
        }

        public void addFunction(FunctionAppearance func, string name)
        {
            int y = 10 + 31 * names.Count;
            
            Label label = new Label();
            label.Content = name;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignment = VerticalAlignment.Top;
            label.Margin = new Thickness(45, y, 0, 0);
            names.Add(label);
            grid.Children.Add(label);
            Sketches.Sketch sketch = new Sketches.Sketch();
            sketch.draw = gl => {
                gl.Color((float)(func.color >> 16 & 0xff) / 255, (float)(func.color >> 8 & 0xff) / 255, (float)(func.color & 0xff) / 255);
                gl.LineWidth(func.lineWidth);
                gl.Enable(OpenGL.GL_LINE_STIPPLE);
                gl.LineStipple(1, func.mask);
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex4d(0, sketch.ActualHeight / 2, 0, 1);
                gl.Vertex4d(sketch.ActualWidth, sketch.ActualHeight / 2, 0, 1);
                gl.End();
            };
            sketch.HorizontalAlignment = HorizontalAlignment.Left;
            sketch.VerticalAlignment = VerticalAlignment.Top;
            sketch.Margin = new Thickness(10, y, 0, 0);
            grid.Children.Add(sketch);

            graphics.functions.Add(func);
        }

        public void clearFunctions()
        {
            graphics.functions.Clear();
            System.Threading.Monitor.Enter(grid);
            try {
                grid.Children.Clear();
            }
            finally
            {
                System.Threading.Monitor.Exit(grid);
            }
            names.Clear();
        }

        bool move;
        double cursorX, cursorY;

        private void glControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            move = true;
            cursorX = e.GetPosition(glControl).X;
            cursorY = e.GetPosition(glControl).Y;
        }

        private void glControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            move = false;
        }

        private void glControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            graphics.zoom(e.Delta > 0 ? 1.1 : 0.9);
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.Translate(0, 0, -1);
            graphics.paint(gl);                
            gl.Flush();
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.ClearColor(0.7f, 0.7f, 0.7f, 1);
            gl.Disable(OpenGL.GL_LIGHTING);
        }

        private void OpenGLControl_Resized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.Viewport(0, 0, (int)glControl.ActualWidth, (int)glControl.ActualHeight);
            graphics.height = glControl.ActualHeight - 10;
            graphics.width = glControl.ActualWidth - 10;
            graphics.x = -glControl.ActualWidth / 2 + 5;
            graphics.y = -glControl.ActualHeight / 2 + 5;
            graphics.centerCoordinateX = 0;
            graphics.centerCoordinateY = 0;
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho((int)(-glControl.ActualWidth / 2), (int)(glControl.ActualWidth / 2), (int)(-glControl.ActualHeight / 2), (int)(glControl.ActualHeight / 2), 0.1, 10);
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (move)
            {
                graphics.translate(e.GetPosition(glControl).X - cursorX, cursorY - e.GetPosition(glControl).Y);
                cursorX = e.GetPosition(glControl).X;
                cursorY = e.GetPosition(glControl).Y;
            }
        }
    }
}
