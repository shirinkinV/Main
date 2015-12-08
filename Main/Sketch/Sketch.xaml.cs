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

namespace Sketches
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class Sketch : UserControl
    {

        public delegate void Draw(OpenGL gl);
        public Draw draw= gl=> { };
        int width, height;

        public Sketch()
        {
            InitializeComponent();
        }

        private void OpenGLControl_Resized(object sender, SharpGL.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            width = (int)glControl.ActualWidth;
            height = (int)glControl.ActualHeight;
            gl.Viewport(0, 0, (int)glControl.ActualWidth, (int)glControl.ActualHeight);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho(0, (int)(glControl.ActualWidth), 0, glControl.ActualHeight, 0.1, 10);
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.Translate(0, 0, -1);
            gl.Color(0, 0, 0, 1);
            gl.Disable(OpenGL.GL_LINE_STIPPLE);
            gl.LineWidth(1);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            gl.Vertex4d(0, 0, 0, 1);
            gl.Vertex4d(0 + width, 0, 0, 1);
            gl.Vertex4d(0 + width, 0 + height, 0, 1);
            gl.Vertex4d(0, 0 + height, 0, 1);
            gl.End();
            draw(gl);
            gl.Flush();
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.OpenGLEventArgs args)
        {
            OpenGL gl = args.OpenGL;
            gl.ClearColor(1, 1, 1, 1);
            gl.Disable(OpenGL.GL_LIGHTING);
        }
    }
}
