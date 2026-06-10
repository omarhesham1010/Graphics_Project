using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics_Project
{
    internal class Transformation
    {
        public DDA Rotate(DDA l, PointF refPoint, float angle)
        {
            float x, y;
            x = l.Xst - refPoint.X;
            y = l.Yst - refPoint.Y;
            l.Xst = (float)(x * Math.Cos(angle) - y * Math.Sin(angle)) + refPoint.X;
            l.Yst = (float)(x * Math.Sin(angle) + y * Math.Cos(angle)) + refPoint.Y;
            x = l.Xend - refPoint.X;
            y = l.Yend - refPoint.Y;
            l.Xend = (float)(x * Math.Cos(angle) - y * Math.Sin(angle)) + refPoint.X;
            l.Yend = (float)(x * Math.Sin(angle) + y * Math.Cos(angle)) + refPoint.Y;
            return l;
        }
        public BezierCurve Rotate(BezierCurve curve,PointF refPoint,float angle)
        {
            for (int i = 0; i < curve.ControlPoints.Count; i++)
            {
                float x = curve.ControlPoints[i].X - refPoint.X;
                float y = curve.ControlPoints[i].Y - refPoint.Y;
                curve.ControlPoints[i] = new Point((int)((x * Math.Cos(angle) - y * Math.Sin(angle)) + refPoint.X), (int)((x * Math.Sin(angle) + y * Math.Cos(angle)) + refPoint.Y));
            }
            curve.SaveCurvePoints();
            return curve;
        }
    }
}
