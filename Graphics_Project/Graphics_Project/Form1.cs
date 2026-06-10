using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics_Project
{
    public class Option
    {
        public Bitmap img_1;
        public Bitmap img_2;
        public PointF Location;
    }

    public class Buttons
    {
        public Bitmap Image;
        public PointF Location;
    }

    public class Road
    {
        // Type: 0 for line, 1 for circle, 2 for curve
        public int Type = -1;
        public DDA Line;
        public Circle Circle;
        public BezierCurve Curve;
    }
    
    public class Car
    {
        public Bitmap Image;
        public PointF Location;
        public float Angle;
        public float RequiredAngle;
        public PointF pt1;
        public PointF pt2;
        public PointF pt3;
        public PointF pt4;

        public void ReviewAngle()
        {
            if (RequiredAngle > Angle + 5)
            {
                Angle += 5;
            }
            else if (RequiredAngle < Angle - 5)
            {
                Angle -= 5;
            }
        }
    }

    public partial class Form1 : Form
    {
        Bitmap off;
        Bitmap BackGround;
        Car Ball;
        List<Buttons> buttons = new List<Buttons>();
        List<Option> options = new List<Option>();
        List<Car> Cars = new List<Car>();
        List<Road> roads = new List<Road>();
        Transformation Transformation = new Transformation();
        Timer tt = new Timer();
        float xCar = 0;
        float yCar = 500;
        float StartDisplay = 0;
        float LastRoadX;
        float LastRoadY;
        float CurrentX;
        float CurrentY;
        float XinMouseDown;
        float YinMouseDown;
        float XoutMouseDown;
        float YoutMouseDown;
        float Speed = 10;
        int NearingCarRoad = 10;
        int SelectedOption = -1;
        int CurrentSimulateion = 0;
        bool isSimulate = false;
        bool isRightScrolling = false;
        bool isLeftScrolling = false;
        bool isPlay = false;
        bool isTransform = false;
        bool usingCars = true;


        public Form1()
        {
            this.WindowState = FormWindowState.Maximized;
            this.Paint += Form1_Paint;
            this.Load += Form1_Load;
            this.MouseMove += Form1_MouseMove;
            tt.Tick += Tt_Tick;
            this.MouseClick += Form1_MouseClick;
            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;
            this.KeyDown += Form1_KeyDown;
            tt.Interval = 1;
            tt.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                if (Speed <= 20)
                {
                    Speed++;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (Speed > 0)
                {
                    Speed--;
                }
            }
            else if (e.KeyCode == Keys.B)
            {
                usingCars = false;
            }
            else if (e.KeyCode == Keys.T)
            {
                usingCars = true;
            }
        }

        private void Tt_Tick(object sender, EventArgs e)
        {
            if(isSimulate && roads.Count != 0)
            {
                if (roads[CurrentSimulateion].Type == 0)
                {
                    if (!roads[CurrentSimulateion].Line.CalcNextPoint(Speed))
                    {
                        roads[CurrentSimulateion].Line.calc();
                        CurrentSimulateion++;
                        if (CurrentSimulateion >= roads.Count)
                        {
                            CurrentSimulateion = 0;
                            isSimulate = false;
                        }
                    }
                    else
                    {
                        xCar = roads[CurrentSimulateion].Line.cx - StartDisplay;
                        yCar = roads[CurrentSimulateion].Line.cy - Cars[0].Image.Height + NearingCarRoad;
                        Cars[0].RequiredAngle = roads[CurrentSimulateion].Line.angle;
                    }
                }
                else if (roads[CurrentSimulateion].Type == 1)
                {
                    PointF NextPoint = roads[CurrentSimulateion].Circle.Getnextpoint((int)roads[CurrentSimulateion].Circle.theta);
                    if (roads[CurrentSimulateion].Circle.theta > -270)
                    {
                        roads[CurrentSimulateion].Circle.theta -= (Speed / 2);
                        Cars[0].RequiredAngle = roads[CurrentSimulateion].Circle.theta - 90;
                        xCar = NextPoint.X - StartDisplay;
                        yCar = NextPoint.Y - Cars[0].Image.Height + NearingCarRoad;
                    }
                    else
                    {
                        roads[CurrentSimulateion].Circle.theta = 90;
                        CurrentSimulateion++;
                        if (CurrentSimulateion >= roads.Count)
                        {
                            CurrentSimulateion = 0;
                            isSimulate = false;
                        }
                    }
                }
                else if (roads[CurrentSimulateion].Type == 2)
                {
                    if(roads[CurrentSimulateion].Curve.theta + 0.05f < 1.0f)
                    {
                        roads[CurrentSimulateion].Curve.theta += (0.001f * Speed);
                        PointF NextPoint = roads[CurrentSimulateion].Curve.CalcCurvePointAtTime(roads[CurrentSimulateion].Curve.theta);
                        xCar = NextPoint.X - StartDisplay;
                        yCar = NextPoint.Y - Cars[0].Image.Height + NearingCarRoad;
                        Cars[0].RequiredAngle = roads[CurrentSimulateion].Curve.GetAngle();
                    }
                    else
                    {
                        roads[CurrentSimulateion].Curve.theta = 0.0f;
                        CurrentSimulateion++;
                        if (CurrentSimulateion >= roads.Count)
                        {
                            CurrentSimulateion = 0;
                            isSimulate = false;
                        }
                    }
                }
                if (xCar > this.ClientSize.Width / 2)
                {
                    StartDisplay += Speed;
                }
            }
            else
            {
                if (isLeftScrolling)
                {
                    StartDisplay -= 20;
                    if (StartDisplay < 0) StartDisplay = 0;
                }
                else if (isRightScrolling)
                {
                    StartDisplay += 20;
                    if (StartDisplay > 2000) StartDisplay = 2000;
                }
            }
            this.Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            isLeftScrolling = false;
            isRightScrolling = false;
            isTransform = false;
            if(isPlay)
            {
                if (SelectedOption == 0)
                {
                    DDA line = new DDA() { Xst = LastRoadX, Yst = LastRoadY, Xend = e.X + StartDisplay, Yend = e.Y };
                    line.calc();
                    roads.Add(new Road() { Type = 0, Line = line });
                    LastRoadX = e.X + StartDisplay;
                    LastRoadY = e.Y;
                }
                else if (SelectedOption == 1)
                {
                    bool isValid = true;
                    float Radius = Math.Max(Math.Abs(XoutMouseDown - XinMouseDown), Math.Abs(YoutMouseDown - YinMouseDown)) / 2;
                    Circle circle = new Circle() { XC = (int)LastRoadX, YC = (int)(LastRoadY - Radius), Rad = (int)Radius, st = 0, end = 360, theta = 90 };
                    for(int i=0;i<roads.Count;i++)
                    {
                        if(roads[i].Type == 1)
                        {
                            for(int j = 0; j < 360; j++)
                            {
                                if (IsHitCircle(circle.Getnextpoint(j).X, circle.Getnextpoint(j).Y, roads[i].Circle.XC, roads[i].Circle.YC, roads[i].Circle.Rad + 20))
                                {
                                    isValid = false;
                                }
                            }
                        }
                    }
                    if (isValid)
                    {
                        roads.Add(new Road() { Type = 1, Circle = circle });
                    }
                }
                else if (SelectedOption == 2)
                {
                    BezierCurve curve = new BezierCurve();
                    Point p;
                    p = new Point((int)(LastRoadX), (int)LastRoadY);
                    curve.SetControlPoint(p);
                    float xPull = ((CurrentX + LastRoadX) / 2) + XoutMouseDown - XinMouseDown;
                    float yPull = ((CurrentY + LastRoadY) / 2) + YoutMouseDown - YinMouseDown;
                    p = new Point((int)xPull, (int)yPull);
                    curve.SetControlPoint(p);
                    p = new Point((int)(XinMouseDown + StartDisplay), (int)YinMouseDown);
                    curve.SetControlPoint(p);
                    curve.SaveCurvePoints();
                    roads.Add(new Road() { Type = 2, Curve = curve });
                    LastRoadX = XinMouseDown + StartDisplay;
                    LastRoadY = YinMouseDown;
                }
                isPlay = false;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isSimulate)
            {
                for (int i = 0; i < options.Count; i++)
                {
                    if (isHit(e.X, e.Y, options[i].Location.X, options[i].Location.Y, options[i].img_1.Width, options[i].img_1.Height))
                    {
                        return;
                    }
                }
                if (isHit(e.X, e.Y, buttons[0].Location.X, buttons[0].Location.Y, buttons[0].Image.Width, buttons[0].Image.Height))
                {
                    return;
                }
                if (isHit(e.X, e.Y, buttons[1].Location.X, buttons[1].Location.Y, buttons[1].Image.Width, buttons[1].Image.Height))
                {
                    isLeftScrolling = true;
                    return;
                }
                else if (isHit(e.X, e.Y, buttons[2].Location.X, buttons[2].Location.Y, buttons[2].Image.Width, buttons[2].Image.Height))
                {
                    isRightScrolling = true;
                    return;
                }
                else if (IsHitCircle(e.X, e.Y, LastRoadX - StartDisplay, LastRoadY, 15))
                {
                    isTransform = true;
                    return;
                }
                if (SelectedOption != -1)
                {
                    isPlay = true;
                    XinMouseDown = e.X;
                    YinMouseDown = e.Y;
                }
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if(!isSimulate)
            {
                for (int i = 0; i < options.Count; i++)
                {
                    if (isHit(e.X, e.Y, options[i].Location.X, options[i].Location.Y, options[i].img_1.Width, options[i].img_1.Height))
                    {
                        SelectedOption = i;
                        return;
                    }
                }
                if (isHit(e.X, e.Y, buttons[0].Location.X, buttons[0].Location.Y, buttons[0].Image.Width, buttons[0].Image.Height))
                {
                    isSimulate = true;
                    StartDisplay = 0;
                }
            }
        }

        bool isHit(float X, float Y, float oX, float oY, float oW, float oH)
        {
            if (X >= oX && Y >= oY && X <= oX + oW && Y <= oY + oH)
            {
                return true;
            }
            return false;
        }

        public bool IsHitCircle(float mouseX, float mouseY, float centerX, float centerY, float radius)
        {
            float dx = mouseX - centerX;
            float dy = mouseY - centerY;
            return (dx * dx + dy * dy) <= (radius * radius);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if(!isSimulate)
            {
                bool isHover = false;
                for(int i=0;i<buttons.Count;i++)
                {
                    if (isHit(e.X, e.Y, buttons[i].Location.X, buttons[i].Location.Y, buttons[i].Image.Width, buttons[i].Image.Height))
                    {
                        isHover = true;
                        break;
                    }
                }
                for (int i = 0; i < options.Count; i++)
                {
                    if (isHit(e.X, e.Y, options[i].Location.X, options[i].Location.Y, options[i].img_1.Width, options[i].img_1.Height))
                    {
                        isHover = true;
                        break;
                    }
                }
                if(isHover)
                {
                    this.Cursor = Cursors.Hand;
                    return;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
                XoutMouseDown = e.X;
                YoutMouseDown = e.Y;
            }
            if(isTransform)
            {
                if (roads[roads.Count-1] != null)
                {
                    if(roads[roads.Count-1].Type == 0)
                    {
                        roads[roads.Count - 1].Line = Transformation.Rotate(roads[roads.Count - 1].Line, new PointF(roads[roads.Count-1].Line.Xst, roads[roads.Count-1].Line.Yst), (e.Y - LastRoadY) / 5000);
                        LastRoadX = roads[roads.Count - 1].Line.Xend;
                        LastRoadY = roads[roads.Count - 1].Line.Yend;
                    }
                    else if (roads[roads.Count-1].Type == 2)
                    {
                        roads[roads.Count - 1].Curve = Transformation.Rotate(roads[roads.Count - 1].Curve, roads[roads.Count - 1].Curve.ControlPoints[0], (e.Y - LastRoadY) / 5000);
                        LastRoadX = roads[roads.Count - 1].Curve.ControlPoints[roads[roads.Count - 1].Curve.ControlPoints.Count - 1].X;
                        LastRoadY = roads[roads.Count - 1].Curve.ControlPoints[roads[roads.Count - 1].Curve.ControlPoints.Count - 1].Y;
                    }
                }
            }
            CurrentX = e.X + StartDisplay;
            CurrentY = e.Y;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            off = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);

            BackGround = new Bitmap("BackGround.png");
            BackGround = new Bitmap(BackGround, new Size(this.ClientSize.Width, this.ClientSize.Height));

            buttons.Add(new Buttons() { Image = new Bitmap("Start.png")});
            buttons[buttons.Count - 1].Image = new Bitmap(buttons[buttons.Count - 1].Image, new Size((this.ClientSize.Width / 5), (this.ClientSize.Height / 10)));
            buttons[buttons.Count - 1].Location.X = (this.ClientSize.Width / 2) - (buttons[buttons.Count - 1].Image.Width / 2);
            buttons[buttons.Count - 1].Location.Y = 50;

            buttons.Add(new Buttons() { Image = new Bitmap("ScrollLeft.png") });
            buttons[buttons.Count - 1].Image = new Bitmap(buttons[buttons.Count - 1].Image, new Size((this.ClientSize.Height / 10), (this.ClientSize.Height / 10)));
            buttons[buttons.Count - 1].Location.X = 50;
            buttons[buttons.Count - 1].Location.Y = 50;

            buttons.Add(new Buttons() { Image = new Bitmap("ScrollRight.png") });
            buttons[buttons.Count - 1].Image = new Bitmap(buttons[buttons.Count - 1].Image, new Size((this.ClientSize.Height / 10), (this.ClientSize.Height / 10)));
            buttons[buttons.Count - 1].Location.X = this.ClientSize.Width - 50 - buttons[buttons.Count - 1].Image.Width;
            buttons[buttons.Count - 1].Location.Y = 50;

            int Padding = 50;
            options.Add(new Option() { img_1 = new Bitmap("Line1.png"), img_2 = new Bitmap("Line2.png") });
            options[options.Count - 1].img_1 = new Bitmap(options[options.Count - 1].img_1, new Size((this.ClientSize.Height / 7), (this.ClientSize.Height / 7)));
            options[options.Count - 1].img_2 = new Bitmap(options[options.Count - 1].img_2, new Size((this.ClientSize.Height / 7), (this.ClientSize.Height / 7)));
            options[options.Count - 1].Location.X = (this.ClientSize.Width / 2) - Padding - (float)(1.5 * (this.ClientSize.Height / 7));
            options[options.Count - 1].Location.Y = this.ClientSize.Height - 50 - options[options.Count - 1].img_1.Height;

            options.Add(new Option() { img_1 = new Bitmap("Circle1.png"), img_2 = new Bitmap("Circle2.png") });
            options[options.Count - 1].img_1 = new Bitmap(options[options.Count - 1].img_1, new Size((this.ClientSize.Height / 7), (this.ClientSize.Height / 7)));
            options[options.Count - 1].img_2 = new Bitmap(options[options.Count - 1].img_2, new Size((this.ClientSize.Height / 7), (this.ClientSize.Height / 7)));
            options[options.Count - 1].Location.X = (this.ClientSize.Width / 2) - (float)(0.5 * (this.ClientSize.Height / 7));
            options[options.Count - 1].Location.Y = this.ClientSize.Height - 50 - options[options.Count - 1].img_1.Height;

            options.Add(new Option() { img_1 = new Bitmap("Curve1.png"), img_2 = new Bitmap("Curve2.png") });
            options[options.Count - 1].img_1 = new Bitmap(options[options.Count - 1].img_1, new Size((this.ClientSize.Height / 7), (this.ClientSize.Height / 7)));
            options[options.Count - 1].img_2 = new Bitmap(options[options.Count - 1].img_2, new Size((this.ClientSize.Height / 7), (this.ClientSize.Height / 7)));
            options[options.Count - 1].Location.X = (this.ClientSize.Width / 2) + Padding + (float)(0.5 * (this.ClientSize.Height / 7));
            options[options.Count - 1].Location.Y = this.ClientSize.Height - 50 - options[options.Count - 1].img_1.Height;
            
            for (int i = 0; i < 10; i++)
            {
                Car Car = new Car() { Image = new Bitmap("Car" + i + ".png"), Angle = 0, Location = new PointF(0, 500)};
                Car.Image = new Bitmap(Car.Image, new Size(this.ClientSize.Width / 10, this.ClientSize.Height / 5));
                Car.pt1 = Car.Location;
                Car.pt2 = new PointF(Car.Location.X + Car.Image.Width, Car.Location.Y);
                Car.pt3 = new PointF(Car.Location.X + Car.Image.Width, Car.Location.Y + Car.Image.Height);
                Car.pt4 = new PointF(Car.Location.X, Car.Location.Y + Car.Image.Height);
                Cars.Add(Car);
            }

            Ball = new Car() { Image = new Bitmap("Ball.png"), Angle = 0, Location = new PointF(0, 500) };
            Ball.Image = new Bitmap(Ball.Image, new Size(this.ClientSize.Width / 10, this.ClientSize.Height / 5));
            Ball.pt1 = Ball.Location;
            Ball.pt2 = new PointF(Ball.Location.X + Ball.Image.Width, Ball.Location.Y);
            Ball.pt3 = new PointF(Ball.Location.X + Ball.Image.Width, Ball.Location.Y + Ball.Image.Height);
            Ball.pt4 = new PointF(Ball.Location.X, Ball.Location.Y + Ball.Image.Height);

            LastRoadX = 0;
            LastRoadY = this.ClientSize.Height / 2;

            DrawDubb(this.CreateGraphics());
            this.DoubleBuffered = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawDubb(e.Graphics);
        }

        public void DrawLine(Graphics g,float StartDisplay,float Xst,float Yst,float Xend,float Yend)
        {
            float dx = Xend - Xst;
            float dy = Yend - Yst;
            float len = (float)Math.Sqrt(dx * dx + dy * dy);
            if (len == 0)
                return;
            dx /= len;
            dy /= len;
            float nx = -dy;
            float ny = dx;
            float railWidth = 10;
            PointF rail1S = new PointF(Xst + nx * railWidth - StartDisplay, Yst + ny * railWidth);
            PointF rail1E = new PointF(Xend + nx * railWidth - StartDisplay, Yend + ny * railWidth);
            PointF rail2S = new PointF(Xst - nx * railWidth - StartDisplay, Yst - ny * railWidth);
            PointF rail2E = new PointF(Xend - nx * railWidth - StartDisplay, Yend - ny * railWidth);
            Pen railPen = new Pen(Color.DimGray, 4);
            g.DrawLine(railPen, rail1S, rail1E);
            g.DrawLine(railPen, rail2S, rail2E);
            Pen sleeperPen = new Pen(Color.SaddleBrown, 5);
            for (float t = 0; t < len; t += 25)
            {
                float cx = Xst + dx * t;
                float cy = Yst + dy * t;
                PointF p1 = new PointF(cx + nx * railWidth - StartDisplay, cy + ny * railWidth);
                PointF p2 = new PointF(cx - nx * railWidth - StartDisplay, cy - ny * railWidth);
                g.DrawLine(sleeperPen, p1, p2);
            }
        }

        void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
            float StartD = StartDisplay % this.ClientSize.Width;
            g.DrawImage(BackGround, -StartD, 0);
            g.DrawImage(BackGround, this.ClientSize.Width - StartD, 0);
            Cars[0].Angle = 0;
            if (isSimulate)
            {
                if(usingCars)
                {
                    g.TranslateTransform(xCar + Cars[0].Image.Width / 2, yCar + Cars[0].Image.Height / 2);
                    Cars[0].ReviewAngle();
                    g.RotateTransform(Cars[0].Angle);
                    g.DrawImage(Cars[0].Image, -Cars[0].Image.Width / 2, -Cars[0].Image.Height / 2);
                    g.ResetTransform();
                }
                else
                {
                    Ball.RequiredAngle = Cars[0].RequiredAngle;
                    g.TranslateTransform(xCar + Ball.Image.Width / 2, yCar + Ball.Image.Height / 2);
                    Ball.ReviewAngle();
                    g.RotateTransform(Ball.Angle);
                    g.DrawImage(Ball.Image, -Ball.Image.Width / 2, -Ball.Image.Height / 2);
                    g.ResetTransform();
                }
            }
            else
            {
                for (int i = 0; i < buttons.Count; i++)
                {
                    g.DrawImage(buttons[i].Image, buttons[i].Location.X, buttons[i].Location.Y);
                }
                for (int i = 0; i < options.Count; i++)
                {
                    if (SelectedOption == i)
                    {
                        g.DrawImage(options[i].img_2, options[i].Location.X, options[i].Location.Y);
                    }
                    else
                    {
                        g.DrawImage(options[i].img_1, options[i].Location.X, options[i].Location.Y);
                    }
                }
            }
            for (int i = 0; i < roads.Count; i++)
            {
                if (roads[i].Type == 0)
                {
                    DrawLine(g, StartDisplay, roads[i].Line.Xst, roads[i].Line.Yst, roads[i].Line.Xend, roads[i].Line.Yend);
                }
                else if (roads[i].Type == 1)
                {
                    roads[i].Circle.Drawcircle(g, StartDisplay);
                }
                else if (roads[i].Type == 2)
                {
                    roads[i].Curve.DrawCurve(g, StartDisplay);
                }
            }
            if(isPlay)
            {
                if (SelectedOption == 0)
                {
                    DrawLine(g, StartDisplay, LastRoadX, LastRoadY, CurrentX, CurrentY);
                }
                else if (SelectedOption == 1)
                {
                    float Radius = Math.Max(Math.Abs(XoutMouseDown - XinMouseDown), Math.Abs(YoutMouseDown - YinMouseDown)) / 2;
                    Circle circle = new Circle() { XC = (int)(LastRoadX), YC = (int)(LastRoadY - Radius), Rad = (int)Radius, st = 0, end = 360 };
                    circle.Drawcircle(g, StartDisplay);
                }
                else if (SelectedOption == 2)
                {
                    BezierCurve curve = new BezierCurve();
                    Point p;
                    p = new Point((int)(LastRoadX), (int)LastRoadY);
                    curve.SetControlPoint(p);
                    float xPull = ((CurrentX + LastRoadX) / 2) + XoutMouseDown - XinMouseDown;
                    float yPull = ((CurrentY + LastRoadY) / 2) + YoutMouseDown - YinMouseDown;
                    p = new Point((int)xPull, (int)yPull);
                    curve.SetControlPoint(p);
                    p = new Point((int)(XinMouseDown + StartDisplay), (int)YinMouseDown);
                    curve.SetControlPoint(p);
                    curve.SaveCurvePoints();
                    curve.DrawCurve(g, StartDisplay);
                }
            }
            g.FillEllipse(Brushes.Blue, LastRoadX - 15 - StartDisplay, LastRoadY - 15, 30, 30);
        }

        void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
    }
}