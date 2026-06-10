using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics_Project
{
    public class Circle
    {
        public int Rad;
        public int XC;
        public int YC;
        public float thRadian;
        public float theta;
        public float st, end;

        public void Drawcircle(Graphics g, float StartDisplay)
        {
            float trackWidth = 10;

            PointF prevOuter = PointF.Empty;
            PointF prevInner = PointF.Empty;

            bool first = true;

            for (float i = st; i <= end; i += 1.0f)
            {
                float rad = (float)(i * Math.PI / 180.0);
                float x = (float)(Rad * Math.Cos(rad)) + XC;
                float y = (float)(Rad * Math.Sin(rad)) + YC;
                float dx = x - XC;
                float dy = y - YC;
                float len = (float)Math.Sqrt(dx * dx + dy * dy);
                dx /= len;
                dy /= len;
                PointF outer = new PointF(x + dx * trackWidth - StartDisplay,y + dy * trackWidth);
                PointF inner = new PointF(x - dx * trackWidth - StartDisplay,y - dy * trackWidth);
                if (!first)
                {
                    g.DrawLine(new Pen(Color.DimGray, 4),prevOuter,outer);
                    g.DrawLine(new Pen(Color.DimGray, 4),prevInner,inner);
                }
                if (((int)i) % 8 == 0)
                {
                    g.DrawLine(new Pen(Color.SaddleBrown, 5), outer, inner);
                }
                prevOuter = outer;
                prevInner = inner;
                first = false;
            }
        }
        public PointF Getnextpoint(int theta)
        {

            PointF p = new PointF();

            thRadian = (float)(theta * Math.PI / 180);

            p.X = (float)(Rad * Math.Cos(thRadian)) + XC;
            p.Y = (float)(Rad * Math.Sin(thRadian)) + YC;
            return p;
        }
    }
}
