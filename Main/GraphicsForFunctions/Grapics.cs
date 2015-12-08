using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGL;

namespace Application_for_course_project
{
    class Grapics
    {

        public double x=-1, y=-1, width=2, height=2;
        public List<FunctionAppearance> functions=new List<FunctionAppearance>();
        public double unitsProPixel=0.10;
        public double centerCoordinateX;
        public double centerCoordinateY;

        public Grapics(double x,double y,double width,double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            centerCoordinateX = x + width / 2;
            centerCoordinateY = y + height / 2;
        }

        public void paint(OpenGL gl)
        {
            gl.Color(1f, 1f, 1f);
            gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
                gl.Vertex(x, y + height, 0);
                gl.Vertex(x, y, 0);
                gl.Vertex(x+width, y + height, 0);
                gl.Vertex(x+width, y, 0);
            gl.End();
            gl.LineWidth(1);
            gl.Begin(OpenGL.GL_LINES);
            gl.Color(0, 0, 0);
                if (centerCoordinateX <= x + width && centerCoordinateX >= x)
                {
                    gl.Vertex4d(centerCoordinateX, y, 0, 1);
                    gl.Vertex4d(centerCoordinateX, y + height, 0, 1);
                }
                if (centerCoordinateY <= y + height && centerCoordinateY >= y)
                {
                    gl.Vertex4d(x, centerCoordinateY, 0, 1);
                    gl.Vertex4d(x+width, centerCoordinateY, 0, 1);
                }
            gl.End();
            for(int i = 0; i < functions.Count; i++)
            {
                drawFunction(gl, functions.ElementAt(i));
            }
        }

        public void translate(double xPix,double yPix)
        {
            centerCoordinateX += xPix;
            centerCoordinateY += yPix;
        }

        public void zoom(double zoom)
        {
            centerCoordinateX *= zoom;
            centerCoordinateY *= zoom;
            unitsProPixel /= zoom;
        }

        void drawFunction(OpenGL gl, FunctionAppearance app)
        {
            
            gl.LineWidth(app.lineWidth);
            double aInPaintCoords = centerCoordinateX + app.a / unitsProPixel;
            double bInPaintCoords = centerCoordinateX + app.b / unitsProPixel;
            double a = 0;
            double b = 0;
            if (aInPaintCoords<=x+width)          
            {
                if (bInPaintCoords >= x)
                {
                    if (aInPaintCoords >= x)
                    {
                        a = app.a;
                    }
                    else
                    {
                        a = (x-centerCoordinateX) * unitsProPixel;
                    }
                    if (bInPaintCoords <= x + width)
                    {
                        b = app.b;
                    }
                    else
                    {
                        b = (x + width - centerCoordinateX) * unitsProPixel;
                    }

                    gl.Begin(OpenGL.GL_LINE_STRIP);
                    double xInFunctionCoords = a;
                    double xInPaintCoords = Math.Max(aInPaintCoords, x);
                    gl.Color((float)(app.color >> 16 & 0xff) / 255, (float)(app.color >> 8 & 0xff) / 255, (float)(app.color & 0xff) / 255);
                    while (xInFunctionCoords <= b)
                    {
                        double yInPaintCoords = centerCoordinateY + app.func(xInFunctionCoords) / unitsProPixel;

                        if (yInPaintCoords >= y && yInPaintCoords <= y + height)
                        {
                            gl.Vertex(xInPaintCoords, yInPaintCoords, 0);
                        }
                        else
                        {
                            gl.End();
                            gl.Begin(OpenGL.GL_LINE_STRIP);
                        }

                        xInFunctionCoords += unitsProPixel;
                        xInPaintCoords++;
                    }
                    gl.End();

                }
            }
        }
    }
}
