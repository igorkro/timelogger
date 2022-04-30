// Copyright (C) 2022  Igor Krushch
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//
// Email: dev@krushch.com

using System;
using System.Drawing;
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
