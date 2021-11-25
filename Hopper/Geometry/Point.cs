using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hopper.Geometry
{
    public class Point
    {
        public float x { get; set; }
        public float y { get; set; }

        public Point()
        {

        }

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator + (Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }

        public static Point operator - (Point a, Point b)
        {
            return new Point(a.x - b.x, a.y - b.y);
        }
        
        public static Point operator * (Point a, Point b)
        {
            return new Point(a.x * b.x, a.y * b.x);
        }

        public static Point operator * (Point a, float b)
        {
            return new Point(a.x * b, a.y * b);
        }

        public float Distance(Point b)
        {
            float xD = (x - b.x);
            float yD = (y - b.y);
            return (float)Math.Sqrt((xD * xD) + (yD * yD));
        }
    }
}
