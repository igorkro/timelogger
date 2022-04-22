using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeLog.Controls
{
    public partial class InfiniteProgress : Control
    {
        public Color ItemColor { get; set; }
        public int NodesCount { get; set; }
        public int StepInterval 
        { 
            get => stepInterval;
            set => setStepInterval(value);
        }

        private Timer timer = new Timer();
        private int currentTime = 0;
        private int stepInterval;

        public InfiniteProgress()
        {
            InitializeComponent();

            ItemColor = Color.Red;
            NodesCount = 12;
            StepInterval = 20;

            DoubleBuffered = true;

            timer.Tick += Timer_Tick;
            timer.Interval = StepInterval;
            timer.Enabled = Visible;
        }

        private void setStepInterval(int value)
        {
            stepInterval = value;
            timer.Interval = stepInterval;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            currentTime++;

            Refresh();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (NodesCount <= 12)
                return;

            if (!Visible)
                return;

            if (StepInterval <= 10)
                return;

            DrawAnimation(pe);
        }

        private void DrawAnimation(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int minDimension = Math.Min(Size.Width, Size.Height);
            int px = Size.Width / 2;
            int py = Size.Height / 2;

            float radius = (float)minDimension * 0.5f - 4.0f;
            float nodeRadius = 1.2f;

            double delta = Math.PI * 2.0 / (float)NodesCount;

            for (int i = 0; i < NodesCount; i += 2)
            {
                if (i <= currentTime && currentTime <= i + NodesCount)
                {
                    double x = px + Math.Cos(delta * i) * radius;
                    double y = py + Math.Sin(delta * i) * radius;

                    Brush solid = new SolidBrush(Color.FromArgb(255 - (200 / NodesCount * (currentTime - i)), ItemColor.R, ItemColor.G, ItemColor.B));
                    e.Graphics.FillEllipse(solid, (float)x - nodeRadius, (float)y - nodeRadius, nodeRadius * 2, nodeRadius * 2);
                }
            }

            if (currentTime >= 2 * NodesCount)
                currentTime = 0;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
        }
    }
}
