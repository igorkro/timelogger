using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimeLog.Utils
{
    class WindowUtils
    {
        public static string GeometryToString(Form form)
        {
            return form.Location.X.ToString() + "|" +
                form.Location.Y.ToString() + "|" +
                form.Size.Width.ToString() + "|" +
                form.Size.Height.ToString() + "|" +
                form.WindowState.ToString();
        }

        public static void GeometryFromString(string geometry, Form form)
        {
            if (string.IsNullOrEmpty(geometry) == true)
            {
                return;
            }
            string[] numbers = geometry.Split('|');
            string windowString = numbers[4];
            if (windowString == "Normal")
            {
                Point windowPoint = new Point(int.Parse(numbers[0]),
                    int.Parse(numbers[1]));
                Size windowSize = new Size(int.Parse(numbers[2]),
                    int.Parse(numbers[3]));

                bool locOkay = GeometryIsBizarreLocation(windowPoint, windowSize);
                bool sizeOkay = GeometryIsBizarreSize(windowSize);

                if (locOkay == true && sizeOkay == true)
                {
                    form.Location = windowPoint;
                    form.Size = windowSize;
                    form.StartPosition = FormStartPosition.Manual;
                    form.WindowState = FormWindowState.Normal;
                }
                else if (sizeOkay == true)
                {
                    form.Size = windowSize;
                }
            }
            else if (windowString == "Maximized")
            {
                form.Location = new Point(100, 100);
                form.StartPosition = FormStartPosition.Manual;
                form.WindowState = FormWindowState.Maximized;
            }
        }

        private static bool isPointWithinScreens(Point pt)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains(pt))
                    return true;
            }
            return false;
        }

        private static bool GeometryIsBizarreLocation(Point loc, Size size)
        {
            bool locOkay;
            if (loc.X < 0 || loc.Y < 0)
            {
                locOkay = false;
            }
            else if (!isPointWithinScreens(new Point(loc.X + size.Width, loc.Y + size.Height)))
            {
                locOkay = false;
            }
            else
            {
                locOkay = true;
            }
            return locOkay;
        }

        private static bool GeometryIsBizarreSize(Size size)
        {
            return (size.Height <= Screen.PrimaryScreen.WorkingArea.Height &&
                size.Width <= Screen.PrimaryScreen.WorkingArea.Width);
        }
    }
}
