using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trail
{
    public class CurvePointClass
    {
        public PointF Cp = new Point();
        public bool FlagVis = true;
    }
    public class BezierCurve
    {

        public List<PointF> ControlPoints;
        public List<CurvePointClass> CurvePoints = new List<CurvePointClass>();
        public float t_inc = 0.001f;

        public Color cl = Color.Red;
        public Color clr1 = Color.Blue;
        public Color ftColor = Color.Black;

        public BezierCurve()
        {
            ControlPoints = new List<PointF>();
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

        public PointF GetPoint(int i)
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

        private void DrawControlPoints(Graphics g)
        {
            Font Ft = new Font("System", 10);
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                g.FillEllipse(new SolidBrush(clr1),
                                ControlPoints[i].X - 5,
                                ControlPoints[i].Y - 5, 10, 10);

                //g.DrawString("P# " + i, Ft, new SolidBrush(ftColor), ControlPoints[i].X - 30, ControlPoints[i].Y - 30);
                //g.DrawString(ControlPoints[i].X + ", " + ControlPoints[i].Y, Ft, new SolidBrush(ftColor), ControlPoints[i].X - 15, ControlPoints[i].Y - 15);
            }
        }

        public int isCtrlPoint(int XMouse, int YMouse)
        {
            Rectangle rc;
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                rc = new Rectangle((int)ControlPoints[i].X - 5, (int)ControlPoints[i].Y - 5, 10, 10);
                if (XMouse >= rc.Left && XMouse <= rc.Right && YMouse >= rc.Top && YMouse <= rc.Bottom)
                {
                    return i;
                }
            }
            return -1;
        }

        public void ModifyCtrlPoint(int i, float XMouse, float YMouse)
        {
            PointF p = ControlPoints[i];

            p.X = XMouse;
            p.Y = YMouse;
            ControlPoints[i] = p;
        }

        public void SetControlPoint(PointF pt)
        {
            ControlPoints.Add(pt);
        }

        private void DrawCurvePoints(Graphics g)
        {
            if (ControlPoints.Count <= 0)
                return;

            PointF curvePoint;
            for (float t = 0.0f; t <= 1.0; t += t_inc)
            {
                curvePoint = CalcCurvePointAtTime(t);
                g.FillEllipse(new SolidBrush(cl),
                                curvePoint.X - 2, curvePoint.Y - 2,
                                8, 8);
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
        private void DrawCurvePointsOld(Graphics g)
        {
            for (int i = 0; i < CurvePoints.Count; i++)
            {
                if (CurvePoints[i].FlagVis == true)
                {
                    g.FillEllipse(Brushes.Blue, CurvePoints[i].Cp.X - 4, CurvePoints[i].Cp.Y - 4, 8, 8);

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
        public void DrawCurve(Graphics g)
        {
            DrawCurvePoints(g);
            DrawControlPoints(g);
        }


    }
}
