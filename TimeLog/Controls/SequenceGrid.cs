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
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;
using TimeLog.Models;
using TimeLog.Utils;
using System.Collections;

namespace TimeLog.Controls
{
    public partial class SequenceGrid : Control
    {
        private enum ScrollBarState
        {
            Invisible,
            Showing,
            Hiding,
            Visible
        }

        private enum EventAlteringMode
        {
            None,
            LeftSize,
            RightSize,
            Move
        }

        private List<TimeLogEntry> entries = new List<TimeLogEntry>();
        private HashSet<long> separatorTimes = new HashSet<long>();

        private int scrollX = 0;
        private int scrollXMax = 0;
        private int scrollXtotal = 0;

        private int scrollY = 0;
        private int scrollYMax = 0;
        private int scrollYtotal = 0;

        private int ticketNameWidth = 160;
        private int headerHeight = 28;
        private int ticketHeight = 24;

        private int visibilityAnimationDuration = 250;

        private ScrollBarState vertBarState = ScrollBarState.Invisible;
        private bool isVertScrolling = false;
        private int vertScrollingYStart = 0;
        private int vertScrollBarHeight = 0;
        private Color currentVertScrollBarColor;

        private ScrollBarState horzBarState = ScrollBarState.Invisible;
        private bool isHorzScrolling = false;
        private int horzScrollingXStart = 0;
        private int horzScrollBarWidth = 0;
        private Color currentHorzScrollBarColor;

        private OrderedDictionary orderedEntries = new OrderedDictionary();

        private Timer animationTimer = new Timer();

        private Timer currentTimeRefreshTimer = new Timer();

        private int clientAreaHeight;
        private int clientAreaWidth;

        private int totalTicketsHeight;
        private int totalMinutesWidth;

        private delegate bool Animator(int time);

        private Dictionary<string, Animator> animators = new Dictionary<string, Animator>();
        private Dictionary<string, int> animationTime = new Dictionary<string, int>();

        private Dictionary<Rectangle, TimeLogEntry> visibleEntryRect = new Dictionary<Rectangle, TimeLogEntry>(1024);
        private bool requireVisibleEntriesRepopulation = true;

        List<Tuple<int, int>> intersectionIntervals = new List<Tuple<int, int>>();

        private int startHour = 6;
        private int endHour = 21;

        private TimeLogEntry alteringEntry;
        private EventAlteringMode entryAlteringMode = EventAlteringMode.None;
        private DateTime alteringEntityInitialTimeReported;
        private TimeSpan alteringEntityInitialDuration;

        int alterationDelta = 0;
        int alterationStart = 0;
        int alterationDeltaTotal = 0;

        private bool allowModification = false;
        private bool isAltPressed = false;

        public delegate void TicketClickHandler(object sender, string ticket);

        public event TicketClickHandler TicketClick;

        private Pen separatorPen = new Pen(Color.LightGray, 1.0f);
        private Pen hourSeparatorPen = new Pen(Color.DarkGray, 1.0f);
        private Pen currentTimePen = new Pen(Color.DeepSkyBlue, 1.0f);
        private Pen hourSeparatorPen2 = new Pen(Color.FromArgb(90, Color.LightGray), 1.0f);
        private Brush fontBrush = new SolidBrush(Color.Black);
        private Pen breakPen = new Pen(Color.Crimson, 1.0f);
        private Brush headerGradientBrush = new SolidBrush(Color.FromArgb(15, Color.DarkGray));

        private Pen separatorPen2 = new Pen(Color.FromArgb(90, Color.LightGray), 1.0f);

        Pen notFlushedPen = new Pen(Color.Red);
        Brush notFlushedBrush = new SolidBrush(Color.IndianRed);

        Pen flushedPen = new Pen(Color.LawnGreen);
        Brush flushedBrush = new SolidBrush(Color.GreenYellow);



        public int ScrollX 
        { 
            get
            {
                return scrollX;
            } 

            set
            {
                scrollX = Math.Max(0, value);
            }
        }

        public int ScrollY
        {
            get
            {
                return scrollY;
            }

            set
            {
                scrollY = Math.Max(0, value);
            }
        }

        public SequenceGrid()
        {
            InitializeComponent();
            MinimumSize = new Size(600, 320);
            DoubleBuffered = true;

            clientAreaHeight = Height - headerHeight;
            clientAreaWidth = Width - ticketNameWidth;
            totalTicketsHeight = orderedEntries.Count * ticketHeight + (clientAreaHeight / 4);
            totalMinutesWidth = (endHour - startHour) * 60 * 4;

            animationTimer.Interval = 25;
            animationTimer.Tick += AnimationTimer_Tick;
            animationTimer.Start();
        }

        private Color colorFromString(string value)
        {
            Int32 hash = 0;

            for (var i = 0; i < value.Length; i++)
            {
                hash = value[i] + ((hash << 5) - hash);
                hash &= hash;
            }

            int[] rgb = new int[3];
            for (var i = 0; i < 3; i++)
            {
                var v = (hash >> (i * 8)) & 255;
                rgb[i] = v;
            }
            return generateColor(Color.FromArgb(255, rgb[0], rgb[1], rgb[2]), Color.DimGray);
        }

        private Color generateColor(Color start, Color mix)
        {
            int red = start.R;
            int green = start.G;
            int blue = start.B;
            // mix the color
            if (mix != null)
            {
                red = (red + mix.R) / 2;
                green = (green + mix.G) / 2;
                blue = (blue + mix.B) / 2;
            }

            return Color.FromArgb(255, red, green, blue);
        }

        private bool isSame(int value, int a, int epsilon)
        {
            return isBetween(value, a - epsilon, a + epsilon);
        }

        private bool isBetween(int value, int a, int b)
        {
            return value >= a && value <= b;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (animators.Count == 0)
                return;

            var keys = new List<string>(animators.Keys);
            foreach (var key in keys)
            {
                if (animators[key](animationTime[key]))
                {
                    animators.Remove(key);
                }
                else
                {
                    animationTime[key] = animationTime[key] + animationTimer.Interval;
                }
            }

            Refresh();
        }

        private void StartAnimation(string key, Animator animator)
        {
            animators[key] = animator;
            animationTime[key] = 0;
        }

        public void SetHours(int start, int end)
        {
            startHour = start;
            endHour = end;
            totalMinutesWidth = (endHour - startHour) * 60 * 4;
            Refresh();
        }

        public void SetAllowModifications(bool value)
        {
            allowModification = value;
        }

        public void SetTimeLogEntries(List<TimeLogEntry> entries)
        {
            this.entries = entries;

            updateEntries();
        }

        public void SetSeparatorTimes(List<DateTime> dateTimes)
        {
            separatorTimes.Clear();
            foreach (DateTime dateTime in dateTimes)
            {
                separatorTimes.Add((dateTime.Hour - startHour) * 60 + dateTime.Minute);
            }
        }

        private void updateEntries()
        {
            orderedEntries.Clear();

            foreach (TimeLogEntry entry in entries)
            {
                string ticketId = entry.TicketId.ToLower();
                if (!orderedEntries.Contains(ticketId))
                    orderedEntries.Add(ticketId, new List<TimeLogEntry>());

                ((List<TimeLogEntry>)orderedEntries[ticketId]).Add(entry);
            }

            clientAreaHeight = Height - headerHeight;
            totalTicketsHeight = orderedEntries.Count * ticketHeight + (clientAreaHeight / 4);
        }

        public void ScrollToTime(DateTime time)
        {
            recalculateScrollLimits();
            int minute = (time.Hour - startHour) * 60 + time.Minute;
            scrollX = (int)((float)minute / (float)((endHour - startHour) * 60) * scrollXMax);
            scrollX = Math.Max(0, scrollX);
            scrollX = Math.Min(scrollXMax, scrollX);
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics graphics = pe.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            graphics.Clear(Color.White);

            drawTickets(graphics);
            drawHeaders(graphics);

            drawEvents(graphics);

            drawScrollBars(graphics);
        }

        private void recalculateScrollLimits()
        {
            if (clientAreaHeight >= totalTicketsHeight)
            {
                scrollYMax = 0;
                scrollY = 0;
            }
            else
            {
                vertScrollBarHeight = (int)(clientAreaHeight * clientAreaHeight / totalTicketsHeight);
                scrollYMax = clientAreaHeight - vertScrollBarHeight;
                scrollY = Math.Min(scrollYMax, scrollY);
            }

            if (clientAreaWidth >= totalMinutesWidth)
            {
                scrollXMax = 0;
                scrollX = 0;
            }
            else
            {
                horzScrollBarWidth = clientAreaWidth * clientAreaWidth / totalMinutesWidth;

                scrollXMax = clientAreaWidth - horzScrollBarWidth;
                scrollX = Math.Min(scrollXMax, scrollX);
            }
        }

        private void drawScrollBars(Graphics graphics)
        {
            if (vertBarState != ScrollBarState.Invisible)
            {
                Brush barBrush = new SolidBrush(currentVertScrollBarColor);

                if (clientAreaHeight >= totalTicketsHeight)
                {
                    scrollYMax = 0;
                    scrollY = 0;
                }
                else
                {
                    vertScrollBarHeight = (int)(clientAreaHeight * clientAreaHeight / totalTicketsHeight);

                    scrollYMax = clientAreaHeight - vertScrollBarHeight;

                    GraphicsExt.FillRoundedRectangle(graphics, barBrush, new Rectangle(Width - 8, headerHeight + scrollY, 8, vertScrollBarHeight), 2);
                }
            }

            if (horzBarState != ScrollBarState.Invisible)
            {
                Brush barBrush = new SolidBrush(currentHorzScrollBarColor);

                if (clientAreaWidth >= totalMinutesWidth)
                {
                    scrollXMax = 0;
                    scrollX = 0;
                }
                else
                {
                    horzScrollBarWidth = clientAreaWidth * clientAreaWidth / totalMinutesWidth;

                    scrollXMax = clientAreaWidth - horzScrollBarWidth;

                    GraphicsExt.FillRoundedRectangle(graphics, barBrush, new Rectangle(ticketNameWidth + scrollX, Height - 8, horzScrollBarWidth, 8), 2);
                }
            }
        }

        private void drawHeaders(Graphics graphics)
        {
            graphics.SetClip(Rectangle.FromLTRB(ticketNameWidth, 0, Width, Height));

            Rectangle gradientRect = new Rectangle(ticketNameWidth, 0, Width - ticketNameWidth, headerHeight);
            graphics.FillRectangle(headerGradientBrush, gradientRect);

            int totalMinutes = (endHour - startHour) * 60;

            int startMinute = scrollXMax > 0 ? (int)((scrollX / (float)scrollXMax * (totalMinutesWidth - clientAreaWidth * 1.5f)) / 4) : 0;

            DateTime now = DateTime.Now;
            int currentMinute = (now.Hour - startHour) * 60 + now.Minute;

            int posX = ticketNameWidth - scrollX;
            for (int i = startMinute; i <= totalMinutes; i++)
            {
                if (posX > Width)
                    break;

                else if (i % 60 == 0)
                {
                    graphics.DrawLine(hourSeparatorPen, posX, headerHeight - 12, posX, headerHeight);
                    graphics.DrawLine(hourSeparatorPen2, posX, headerHeight + 1, posX, Height);
                    graphics.DrawString((startHour + i / 60).ToString(), Font, fontBrush, posX, 2, StringFormat.GenericDefault);
                }
                else if (i % 30 == 0)
                    graphics.DrawLine(hourSeparatorPen, posX, headerHeight - 8, posX, headerHeight);
                else if (i % 5 == 0)
                    graphics.DrawLine(separatorPen, posX, headerHeight - 6, posX, headerHeight);
                else
                    graphics.DrawLine(separatorPen, posX, headerHeight - 2, posX, headerHeight);

                if (i == totalMinutes)
                {
                    graphics.DrawLine(hourSeparatorPen, posX, headerHeight, posX, Height);
                    break;
                }

                if (separatorTimes.Contains(i))
                {
                    graphics.DrawLine(breakPen, posX, headerHeight, posX, Height);
                }
                else if (currentMinute == i)
                {
                    graphics.DrawLine(currentTimePen, posX, headerHeight, posX, Height);
                }

                posX += 4;
            }
            graphics.ResetClip();
        }

        private void drawTickets(Graphics graphics)
        {
            graphics.DrawLine(separatorPen, ticketNameWidth - 1, 0, ticketNameWidth - 1, Height);
            graphics.DrawLine(separatorPen, 0, headerHeight, Width, headerHeight);

            graphics.SetClip(Rectangle.FromLTRB(0, headerHeight, Width, Height));

            int normalizedScrollY = 0;
            if (scrollYMax > 0)
                normalizedScrollY = (int)((float)scrollY / scrollYMax * (totalTicketsHeight - clientAreaHeight));

            int currentY = headerHeight - (normalizedScrollY % ticketHeight);

            int firstTicketToPaint = normalizedScrollY / ticketHeight;
            int currentTicket = 0;
            int maxTicketToPaint = (Height - headerHeight) / ticketHeight + firstTicketToPaint + 1;
            foreach (string key in orderedEntries.Keys)
            {
                if (currentTicket < firstTicketToPaint)
                {
                    currentTicket++;
                    continue;
                }

                if (currentTicket > maxTicketToPaint)
                    break;

                graphics.DrawLine(separatorPen, 0, currentY + ticketHeight, ticketNameWidth, currentY + ticketHeight);
                graphics.DrawLine(separatorPen2, ticketNameWidth + 1, currentY + ticketHeight, Width, currentY + ticketHeight);
                graphics.DrawString(key.ToUpper(), Font, fontBrush, 4, currentY + (ticketHeight - Font.Height) * 0.5f);

                currentY += ticketHeight;
                currentTicket++;
            }

            graphics.ResetClip();
        }

        private bool overlap(int a1, int b1, int a2, int b2)
        {
            return (a2 <= a1 && a1 <= b2) || (a2 <= b1 && b1 <= b2) || (a1 <= a2 && a2 <= b1) || (a1 <= b2 && b2 <= b1);
        }

        private void drawEvents(Graphics graphics)
        {
            int startMinute = scrollXMax > 0 ? (int)((scrollX / (float)scrollXMax * (totalMinutesWidth - clientAreaWidth * 1.5f)) / 4) : 0;
            int endMinute = startMinute + (clientAreaWidth + scrollX) / 4;
            int posX = ticketNameWidth - scrollX;

            int normalizedScrollY = 0;
            if (scrollYMax > 0)
                normalizedScrollY = (int)((float)scrollY / scrollYMax * (totalTicketsHeight - clientAreaHeight));

            int currentY = headerHeight - (normalizedScrollY % ticketHeight);

            int firstTicketToPaint = normalizedScrollY / ticketHeight;

            int currentTicket = 0;
            int maxTicketToPaint = (Height - headerHeight) / ticketHeight + firstTicketToPaint + 1;

            graphics.SetClip(Rectangle.FromLTRB(ticketNameWidth, headerHeight, Width, Height));

            if (requireVisibleEntriesRepopulation)
            {
                visibleEntryRect.Clear();
                intersectionIntervals.Clear();
            }

            foreach (string key in orderedEntries.Keys)
            {
                if (currentTicket < firstTicketToPaint)
                {
                    currentTicket++;
                    continue;
                }

                if (currentTicket > maxTicketToPaint)
                    break;

                List<TimeLogEntry> logEntries = orderedEntries[key] as List<TimeLogEntry>;

                Color ticketColor = colorFromString(key);
                Brush eventBrush = new SolidBrush(Color.FromArgb(128, ticketColor));
                Pen eventPen = new Pen(ticketColor, 1);

                foreach (TimeLogEntry entry in logEntries)
                {
                    int hour = entry.TimeReported.Value.Hour;
                    int minute = entry.TimeReported.Value.Minute;

                    int entryDuration = (int)entry.Duration.Value.TotalMinutes;
                    int entryEndMinute = (hour - startHour) * 60 + minute;
                    int entryStartMinute = entryEndMinute - entryDuration;

                    if (overlap(startMinute, endMinute, entryStartMinute, entryEndMinute))
                    {
                        Rectangle rect = new Rectangle(posX + (entryStartMinute - startMinute) * 4, currentY, entryDuration * 4, ticketHeight);
                        graphics.FillRectangle(eventBrush, rect);
                        graphics.DrawRectangle(eventPen, rect);

                        Rectangle flushingRect = new Rectangle(rect.Right - 3, rect.Bottom - 3, 3, 3);
                        if (entry.Flushed)
                        {
                            graphics.FillRectangle(flushedBrush, flushingRect);
                            graphics.DrawRectangle(flushedPen, flushingRect);
                        }
                        else
                        {
                            graphics.FillRectangle(notFlushedBrush, flushingRect);
                            graphics.DrawRectangle(notFlushedPen, flushingRect);
                        }

                        if (requireVisibleEntriesRepopulation)
                            visibleEntryRect[rect] = entry;
                    }
                }

                currentY += ticketHeight;
                currentTicket++;
            }

            Brush intersectionBrush = new SolidBrush(Color.FromArgb(35, Color.LightPink));
            List<Rectangle> visibleRects = new List<Rectangle>(visibleEntryRect.Keys);

            if (requireVisibleEntriesRepopulation)
            {
                for (int i = 0; i < visibleRects.Count - 1; ++i)
                {
                    for (int j = i + 1; j < visibleRects.Count; ++j)
                    {
                        if (overlap(visibleRects[i].Left, visibleRects[i].Right, visibleRects[j].Left, visibleRects[j].Right))
                        {
                            Rectangle intersection = Rectangle.Intersect(visibleRects[i], visibleRects[j]);
                            Tuple<int, int> newIntersection = new Tuple<int, int>(intersection.Left, intersection.Right);

                            if (!processIntersection(newIntersection))
                                intersectionIntervals.Add(newIntersection);
                        }
                    }
                }

                postprocessIntersections();
            }

            for (int i = 0; i < intersectionIntervals.Count; ++i)
            {
                graphics.FillRectangle(intersectionBrush, intersectionIntervals[i].Item1, headerHeight, intersectionIntervals[i].Item2 - intersectionIntervals[i].Item1, Height - headerHeight);
            }

            graphics.ResetClip();
            requireVisibleEntriesRepopulation = false;
        }

        private void postprocessIntersections()
        {
            bool wereAnyIntersections;
            do
            {
                wereAnyIntersections = false;
                for (int i = 0; i < intersectionIntervals.Count - 1; ++i)
                {
                    for (int j = i + 1; j < intersectionIntervals.Count; )
                    {
                        if (overlap(intersectionIntervals[i].Item1, intersectionIntervals[i].Item2, intersectionIntervals[j].Item1, intersectionIntervals[j].Item2))
                        {
                            intersectionIntervals[i] = new Tuple<int, int>(Math.Min(intersectionIntervals[i].Item1, intersectionIntervals[j].Item1), Math.Max(intersectionIntervals[i].Item1, intersectionIntervals[j].Item1));
                            intersectionIntervals.RemoveAt(j);
                            wereAnyIntersections = true;
                        }
                        else
                            j++;
                    }
                }
            } while (wereAnyIntersections);
        }

        private bool processIntersection(Tuple<int, int> newIntersection)
        {
            for (int i = 0; i < intersectionIntervals.Count; ++i)
            {
                if (overlap(intersectionIntervals[i].Item1, intersectionIntervals[i].Item2, newIntersection.Item1, newIntersection.Item2))
                {
                    intersectionIntervals[i] = new Tuple<int, int>(Math.Min(intersectionIntervals[i].Item1, newIntersection.Item1), Math.Max(intersectionIntervals[i].Item1, newIntersection.Item1));
                    return true;
                }
            }
            return false;
        }

        private void checkAndProcessVertScrollBarVisibility(MouseEventArgs e)
        {
            if (clientAreaHeight >= totalTicketsHeight)
                return;

            var newVisibility = e.X > (Width - 10) && e.X < Width && (e.Y < Height - 10);
            switch (vertBarState)
            {
                case ScrollBarState.Invisible:
                case ScrollBarState.Hiding:
                    if (newVisibility)
                    {
                        float fullTime = (float)visibilityAnimationDuration / 255.0f * (float)(255.0f - currentVertScrollBarColor.A);
                        float step = (255.0f - currentVertScrollBarColor.A) / fullTime;
                        vertBarState = ScrollBarState.Showing;
                        StartAnimation("vertsb-visibility", (int time) =>
                        {
                            if (time >= fullTime)
                            {
                                currentVertScrollBarColor = Color.FromArgb(255, Color.DarkGray);
                                vertBarState = ScrollBarState.Visible;
                                return true;
                            }
                            else
                            {
                                currentVertScrollBarColor = Color.FromArgb((int)(time * step), Color.DarkGray);
                            }
                            return false;
                        });
                    }
                    break;
                case ScrollBarState.Showing:
                case ScrollBarState.Visible:
                    if (!newVisibility)
                    {
                        float fullTime = (float)visibilityAnimationDuration / 255.0f * currentVertScrollBarColor.A;
                        float step = (0.0f - currentVertScrollBarColor.A) / fullTime;
                        int startAlpha = currentVertScrollBarColor.A;
                        vertBarState = ScrollBarState.Hiding;
                        StartAnimation("vertsb-visibility", (int time) =>
                        {
                            if (time >= fullTime)
                            {
                                currentVertScrollBarColor = Color.FromArgb(0, Color.DarkGray);
                                vertBarState = ScrollBarState.Invisible;
                                return true;
                            }
                            else
                            {
                                currentVertScrollBarColor = Color.FromArgb(startAlpha + (int)(time * step), Color.DarkGray);
                            }
                            return false;
                        });
                    }
                    break;
            }
        }


        private void checkAndProcessHorzScrollBarVisibility(MouseEventArgs e)
        {
            if (clientAreaWidth >= totalMinutesWidth)
                return;

            var newVisibility = e.Y > (Height - 10) && e.Y < Height && (e.X < Width - 10);
            switch (horzBarState)
            {
                case ScrollBarState.Invisible:
                case ScrollBarState.Hiding:
                    if (newVisibility)
                    {
                        float fullTime = (float)visibilityAnimationDuration / 255.0f * (float)(255.0f - currentHorzScrollBarColor.A);
                        float step = fullTime > 0.0f ? (255.0f - currentHorzScrollBarColor.A) / fullTime : 0.0f;
                        horzBarState = ScrollBarState.Showing;
                        StartAnimation("horzsb-visibility", (int time) =>
                        {
                            if (time >= fullTime)
                            {
                                currentHorzScrollBarColor = Color.FromArgb(255, Color.DarkGray);
                                horzBarState = ScrollBarState.Visible;
                                return true;
                            }
                            else
                            {
                                currentHorzScrollBarColor = Color.FromArgb((int)(time * step), Color.DarkGray);
                            }
                            return false;
                        });
                    }
                    break;
                case ScrollBarState.Showing:
                case ScrollBarState.Visible:
                    if (!newVisibility)
                    {
                        float fullTime = (float)visibilityAnimationDuration / 255.0f * currentHorzScrollBarColor.A;
                        float step = (0.0f - currentHorzScrollBarColor.A) / fullTime;
                        int startAlpha = currentHorzScrollBarColor.A;
                        horzBarState = ScrollBarState.Hiding;
                        StartAnimation("horzsb-visibility", (int time) =>
                        {
                            if (time >= fullTime)
                            {
                                currentHorzScrollBarColor = Color.FromArgb(0, Color.DarkGray);
                                horzBarState = ScrollBarState.Invisible;
                                return true;
                            }
                            else
                            {
                                currentHorzScrollBarColor = Color.FromArgb(startAlpha + (int)(time * step), Color.DarkGray);
                            }
                            return false;
                        });
                    }
                    break;
            }
        }


        private void checkEventResizers(MouseEventArgs e)
        {
            if (e.X < ticketNameWidth || e.Y < headerHeight || e.X > Width - 10 || e.Y > Height)
                return;

            foreach (Rectangle rect in visibleEntryRect.Keys)
            {
                if (isSame(e.X, rect.Left, 1) && isBetween(e.Y, rect.Top, rect.Bottom))
                {
                    Cursor = Cursors.VSplit;
                    alteringEntry = visibleEntryRect[rect];
                    entryAlteringMode = EventAlteringMode.LeftSize;
                    return;
                }
                else if (isSame(e.X, rect.Right, 1) && isBetween(e.Y, rect.Top, rect.Bottom))
                {
                    Cursor = Cursors.VSplit;
                    alteringEntry = visibleEntryRect[rect];
                    entryAlteringMode = EventAlteringMode.RightSize;
                    return;
                }
                else if (isBetween(e.X, rect.Left, rect.Right) && isBetween(e.Y, rect.Top, rect.Bottom))
                {
                    Cursor = Cursors.SizeAll;
                    alteringEntry = visibleEntryRect[rect];
                    entryAlteringMode = EventAlteringMode.Move;
                    return;
                }
            }
            Cursor = Cursors.Default;
            alteringEntry = null;
            entryAlteringMode = EventAlteringMode.None;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                if (e.X > (Width - 10) && e.X < Width && e.Y > headerHeight)
                {
                    int scrollMin = headerHeight + scrollY;
                    int scrollMax = scrollMin + vertScrollBarHeight;
                    if (e.Y >= scrollMin && e.Y <= scrollMax)
                    {
                        isVertScrolling = true;
                        vertScrollingYStart = e.Y;
                        scrollYtotal = scrollY;
                    }
                    else if (e.Y < scrollMin)
                    {
                        scrollY -= scrollY / 3;
                        scrollY = Math.Max(0, scrollY);
                    }
                    else if (e.Y > scrollY)
                    {
                        scrollY += (scrollYMax - scrollY) / 3;
                    }
                    requireVisibleEntriesRepopulation = true;
                    Refresh();
                }
                else if (e.Y > (Height - 10) && e.Y < Height && e.X > ticketNameWidth)
                {
                    int scrollMin = ticketNameWidth + scrollX;
                    int scrollMax = scrollMin + horzScrollBarWidth;
                    if (e.X >= scrollMin && e.X <= scrollMax)
                    {
                        isHorzScrolling = true;
                        horzScrollingXStart = e.X;
                        scrollXtotal = scrollX;
                    }
                    else if (e.X < scrollMin)
                    {
                        scrollX -= scrollX / 3;
                        scrollX = Math.Max(0, scrollX);
                    }
                    else if (e.X > scrollX)
                    {
                        scrollX += (scrollXMax - scrollX) / 3;
                    }
                    requireVisibleEntriesRepopulation = true;
                    Refresh();
                }
                else if (e.Y > headerHeight && e.Y < (Height - 10) && e.X <= ticketNameWidth)
                {
                    int normalizedScrollY = 0;
                    if (scrollYMax > 0)
                        normalizedScrollY = (int)((float)scrollY / scrollYMax * (totalTicketsHeight - clientAreaHeight));
                    int ticketIndex = (e.Y + normalizedScrollY - headerHeight) / ticketHeight;
                    if (ticketIndex < 0 || ticketIndex >= orderedEntries.Count)
                        return;

                    string ticketName = (string)orderedEntries.Cast<DictionaryEntry>().ElementAt(ticketIndex).Key;

                    TicketClick.Invoke(this, ticketName);
                }
                else if (allowModification && entryAlteringMode != EventAlteringMode.None && alteringEntry != null)
                {
                    alteringEntityInitialTimeReported = alteringEntry.TimeReported.Value;
                    alteringEntityInitialDuration = alteringEntry.Duration.Value;

                    alterationStart = e.X;
                    alterationDelta = 0;
                    alterationDeltaTotal = 0;
                }
                else if (!allowModification)
                {
                    checkWorkLogTooltip(e);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.None)
            {
                checkAndProcessVertScrollBarVisibility(e);
                checkAndProcessHorzScrollBarVisibility(e);

                if (allowModification)
                    checkEventResizers(e);

                //checkWorkLogTooltip(e);
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (isVertScrolling)
                {
                    scrollYtotal += e.Y - vertScrollingYStart;
                    vertScrollingYStart = e.Y;

                    scrollY = Math.Min(scrollYMax, Math.Max(0, scrollYtotal));
                    requireVisibleEntriesRepopulation = true;
                    Refresh();
                }
                else if (isHorzScrolling)
                {
                    scrollXtotal += e.X - horzScrollingXStart;
                    horzScrollingXStart = e.X;

                    scrollX = Math.Min(scrollXMax, Math.Max(0, scrollXtotal));
                    requireVisibleEntriesRepopulation = true;
                    Refresh();
                }
                else if (allowModification)
                {
                    if (entryAlteringMode != EventAlteringMode.None && alteringEntry != null)
                    {
                        alterationDeltaTotal += e.X - alterationStart;
                        alterationStart = e.X;

                        alterationDelta = alterationDeltaTotal / 4;
                        if (alterationDelta == 0)
                            return;

                        TimeSpan offset = new TimeSpan(0, alterationDelta, 0);
                        if (entryAlteringMode == EventAlteringMode.Move)
                        {
                            alteringEntry.TimeReported = alteringEntityInitialTimeReported.AddMinutes(alterationDelta);
                            if ((alteringEntry.TimeReported.Value - alteringEntry.Duration.Value).Hour < startHour)
                            {
                                alteringEntry.TimeReported = new DateTime(alteringEntityInitialTimeReported.Year, alteringEntityInitialTimeReported.Month, alteringEntityInitialTimeReported.Day, startHour, 0, 0) + alteringEntry.Duration.Value;
                            }
                            else if (alteringEntry.TimeReported.Value.Hour >= endHour)
                            {
                                alteringEntry.TimeReported = new DateTime(alteringEntityInitialTimeReported.Year, alteringEntityInitialTimeReported.Month, alteringEntityInitialTimeReported.Day, endHour, 0, 0);
                            }
                        }
                        else if (entryAlteringMode == EventAlteringMode.LeftSize)
                        {
                            alteringEntry.Duration = alteringEntityInitialDuration.Add(-offset);
                        }
                        else if (entryAlteringMode == EventAlteringMode.RightSize)
                        {
                            alteringEntry.TimeReported = alteringEntityInitialTimeReported.AddMinutes(alterationDelta);
                            alteringEntry.Duration = alteringEntityInitialDuration.Add(offset);
                        }
                        Refresh();
                    }
                }
            }
        }

        private void checkWorkLogTooltip(MouseEventArgs e)
        {
            List<Rectangle> visibleRects = new List<Rectangle>(visibleEntryRect.Keys);
            foreach (Rectangle rect in visibleRects)
            {
                if (isBetween(e.X, rect.Left, rect.Right) && isBetween(e.Y, rect.Top, rect.Bottom))
                {
                    TimeLogEntry entry = visibleEntryRect[rect];
                    DateTime timeStarted = entry.TimeReported.Value - entry.Duration.Value;
                    string comment = string.Format("{0}-{1}\r\n{2}", timeStarted.ToString("HH:mm"), entry.TimeReported.Value.ToString("HH:mm"), entry.Comment);
                    toolTip1.Show(comment, this, e.X, e.Y + 10, 5000);
                    return;
                }
            }
            toolTip1.Hide(this);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isVertScrolling = false;
            isHorzScrolling = false;
            if (entryAlteringMode != EventAlteringMode.None)
            {
                requireVisibleEntriesRepopulation = true;
                entryAlteringMode = EventAlteringMode.None;
                alteringEntry = null;

                Refresh();
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Button == MouseButtons.None)
            {
                if (isAltPressed)
                {
                    scrollX += e.Delta;
                    scrollX = Math.Min(scrollXMax, scrollX);
                    scrollX = Math.Max(scrollX, 0);
                }
                else
                {
                    scrollY += e.Delta;
                    scrollY = Math.Min(scrollYMax, scrollY);
                    scrollY = Math.Max(scrollY, 0);
                }
                Refresh();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            clientAreaHeight = Height - headerHeight;
            clientAreaWidth = Width - ticketNameWidth;

            requireVisibleEntriesRepopulation = true;
            recalculateScrollLimits();
            Refresh();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Menu)
                isAltPressed = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            isAltPressed = false;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (!isVertScrolling)
                checkAndProcessVertScrollBarVisibility(new MouseEventArgs(MouseButtons.None, 0, -1, -1, 0));

            if (!isHorzScrolling)
                checkAndProcessHorzScrollBarVisibility(new MouseEventArgs(MouseButtons.None, 0, -1, -1, 0));

        }
    }
}
