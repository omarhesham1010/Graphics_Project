using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics_Project
{
    public class CurvePointClass
    {
        public PointF Cp = new Point();
        public bool FlagVis = true;
    }
    public class BezierCurve
    {

        public List<Point> ControlPoints;
        public List<CurvePointClass> CurvePoints = new List<CurvePointClass>();
        public float t_inc = 0.007f;
        public float theta = 0;

        public Color cl = Color.Red;
        public Color clr1 = Color.Blue;
        public Color ftColor = Color.Black;

        public BezierCurve()
        {
            ControlPoints = new List<Point>();
        }

        public float GetAngle()
        {
            float dt = 0.01f;

            PointF p1 = CalcCurvePointAtTime(theta);

            PointF p2;

            if (theta + dt <= 1.0f)
                p2 = CalcCurvePointAtTime(theta + dt);
            else
                p2 = CalcCurvePointAtTime(theta - dt);

            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            return (float)(Math.Atan2(dy, dx) * 180.0 / Math.PI);
        }

        private float Factorial(int n)
        {
            float res = 1.0f;

            for (int i = 2; i <= n; i++)
                res *= i;

            return res;
        }

        private float C(int n, int i)
        {
            float res = Factorial(n) / (Factorial(i) * Factorial(n - i));
            return res;
        }

        private double Calc_B(float t, int i)
        {
            int n = ControlPoints.Count - 1;
            double res = C(n, i) *
                            Math.Pow((1 - t), (n - i)) *
                            Math.Pow(t, i);
            return res;
        }

        public Point GetPoint(int i)
        {
            return ControlPoints[i];
        }

        public PointF CalcCurvePointAtTime(float t)
        {
            PointF pt = new PointF();
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                float B = (float)Calc_B(t, i);
                pt.X += B * ControlPoints[i].X;
                pt.Y += B * ControlPoints[i].Y;
            }

            return pt;
        }

        private void DrawControlPoints(Graphics g, float StartDisplay)
        {
            Font Ft = new Font("System", 10);
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                g.FillEllipse(new SolidBrush(clr1),
                                ControlPoints[i].X - 5 - StartDisplay,
                                ControlPoints[i].Y - 5, 10, 10);

                //g.DrawString("P# " + i, Ft, new SolidBrush(ftColor), ControlPoints[i].X - 15, ControlPoints[i].Y - 15);
            }
        }

        public int isCtrlPoint(int XMouse, int YMouse)
        {
            Rectangle rc;
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                rc = new Rectangle(ControlPoints[i].X - 5, ControlPoints[i].Y - 5, 10, 10);
                if (XMouse >= rc.Left && XMouse <= rc.Right && YMouse >= rc.Top && YMouse <= rc.Bottom)
                {
                    return i;
                }
            }
            return -1;
        }

        public void ModifyCtrlPoint(int i, int XMouse, int YMouse)
        {
            Point p = ControlPoints[i];

            p.X = XMouse;
            p.Y = YMouse;
            ControlPoints[i] = p;
        }

        public void SetControlPoint(Point pt)
        {
            ControlPoints.Add(pt);
        }

        private void DrawCurvePoints2(Graphics g, float StartDisplay)
        {
            float trackWidth = 12;

            PointF prevP = CalcCurvePointAtTime(0);

            for (float t = t_inc; t <= 1.0f; t += t_inc)
            {
                PointF curP = CalcCurvePointAtTime(t);

                float dx = curP.X - prevP.X;
                float dy = curP.Y - prevP.Y;

                float len = (float)Math.Sqrt(dx * dx + dy * dy);

                if (len > 0)
                {
                    dx /= len;
                    dy /= len;

                    float nx = -dy;
                    float ny = dx;

                    PointF rail1s = new PointF(
                        prevP.X + nx * trackWidth - StartDisplay,
                        prevP.Y + ny * trackWidth);

                    PointF rail1e = new PointF(
                        curP.X + nx * trackWidth - StartDisplay,
                        curP.Y + ny * trackWidth);

                    PointF rail2s = new PointF(
                        prevP.X - nx * trackWidth - StartDisplay,
                        prevP.Y - ny * trackWidth);

                    PointF rail2e = new PointF(
                        curP.X - nx * trackWidth - StartDisplay,
                        curP.Y - ny * trackWidth);

                    g.DrawLine(Pens.DimGray, rail1s, rail1e);
                    g.DrawLine(Pens.DimGray, rail2s, rail2e);
                }

                prevP = curP;
            }

            // Sleepers
            int counter = 0;
            prevP = CalcCurvePointAtTime(0);

            for (float t = t_inc; t <= 1.0f; t += t_inc)
            {
                PointF curP = CalcCurvePointAtTime(t);

                float dx = curP.X - prevP.X;
                float dy = curP.Y - prevP.Y;

                float len = (float)Math.Sqrt(dx * dx + dy * dy);

                if (len > 0)
                {
                    dx /= len;
                    dy /= len;

                    float nx = -dy;
                    float ny = dx;

                    if (counter % 8 == 0)
                    {
                        PointF p1 = new PointF(
                            curP.X + nx * trackWidth - StartDisplay,
                            curP.Y + ny * trackWidth);

                        PointF p2 = new PointF(
                            curP.X - nx * trackWidth - StartDisplay,
                            curP.Y - ny * trackWidth);

                        g.DrawLine(
                            new Pen(Color.SaddleBrown, 5),
                            p1,
                            p2);
                    }
                }

                counter++;
                prevP = curP;
            }
        }

        public void SaveCurvePoints()
        {
            if (ControlPoints.Count <= 0)
                return;
            PointF curvePoint;
            for (float t = 0.0f; t <= 1.0; t += t_inc)
            {
                curvePoint = CalcCurvePointAtTime(t);
                CurvePointClass temp = new CurvePointClass();
                temp.Cp = curvePoint;
                CurvePoints.Add(temp);
            }

        }
        private void DrawCurvePoints(Graphics g, float StartDisplay)
        {
            float trackWidth = 12;

            for (int i = 1; i < CurvePoints.Count; i++)
            {
                if (!CurvePoints[i].FlagVis || !CurvePoints[i - 1].FlagVis)
                {
                    continue;
                }
                PointF p1 = CurvePoints[i - 1].Cp;
                PointF p2 = CurvePoints[i].Cp;
                float dx = p2.X - p1.X;
                float dy = p2.Y - p1.Y;
                float len = (float)Math.Sqrt(dx * dx + dy * dy);
                if (len == 0)
                {
                    continue;
                }
                dx /= len;
                dy /= len;
                float nx = -dy;
                float ny = dx;
                PointF rail1s = new PointF(p1.X + nx * trackWidth - StartDisplay,p1.Y + ny * trackWidth);
                PointF rail1e = new PointF(p2.X + nx * trackWidth - StartDisplay,p2.Y + ny * trackWidth);
                PointF rail2s = new PointF(p1.X - nx * trackWidth - StartDisplay,p1.Y - ny * trackWidth);
                PointF rail2e = new PointF(p2.X - nx * trackWidth - StartDisplay,p2.Y - ny * trackWidth);
                g.DrawLine(new Pen(Color.DimGray, 4),rail1s,rail1e);
                g.DrawLine(new Pen(Color.DimGray, 4),rail2s,rail2e);
                if (i % 8 == 0)
                {
                    g.DrawLine(new Pen(Color.SaddleBrown, 5), rail1s, rail2s);
                }
            }
        }
        public bool IsHit(Point bullet)
        {
            for (int i = 0; i < CurvePoints.Count; i++)
            {
                Rectangle r = new Rectangle((int)CurvePoints[i].Cp.X - 5, (int)CurvePoints[i].Cp.Y - 5, 10, 10);
                if (CurvePoints[i].FlagVis == true)
                {
                    if (bullet.X > r.Left && bullet.X < r.Right && bullet.Y > r.Top && bullet.Y < r.Bottom)
                    {
                        return false;

                    }
                }
            }
            return true;
        }
        public void DrawCurve(Graphics g, float StartDisplay)
        {
            //DrawControlPoints(g, StartDisplay);
            DrawCurvePoints2(g, StartDisplay);
        }


    }
}
