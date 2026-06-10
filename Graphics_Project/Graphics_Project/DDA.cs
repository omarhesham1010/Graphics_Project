using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics_Project
{
    public class DDA
    {
        public float Xst, Yst;
        public float Xend, Yend;
        float dy, dx, m;
        public float cx, cy;
        public float angle;
        int speed = 10;

        public void calc()
        {
            dy = Yend - this.Yst;
            dx = Xend - this.Xst;
            angle = (float)(Math.Atan2(dy, dx) * 180 / Math.PI);
            m = dy / dx;
            cx = Xst;
            cy = Yst;
        }
        public bool CalcNextPoint(float Speed)
        {
            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (Xst < Xend)
                {
                    cx += Speed;
                    cy += m * Speed;
                    if (cx >= Xend)
                    {
                        return false;
                    }

                }
                else
                {
                    cx -= Speed;
                    cy -= m * Speed;
                    if (cx <= Xend)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (Yst < Yend)
                {
                    cy += Speed;
                    cx += 1 / m * Speed;
                    if (cy >= Yend)
                    {
                        return false;
                    }
                }
                else
                {
                    cy -= Speed;
                    cx -= 1 / m * Speed;
                    if (cy <= Yend)
                    {
                        return false;
                    }
                }

            }
            return true;
        }

    }
}
