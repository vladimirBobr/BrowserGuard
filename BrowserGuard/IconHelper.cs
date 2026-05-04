using System;
using System.Drawing;

namespace BrowserGuard
{
    /// <summary>
    /// Helper class for creating application icons
    /// </summary>
    public static class IconHelper
    {
        /// <summary>
        /// Creates a colored icon with Chrome-like design
        /// </summary>
        public static Icon CreateColoredIcon()
        {
            using (var bitmap = new Bitmap(32, 32))
            {
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    
                    // Blue circle with gradient
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        new Point(0, 0), new Point(32, 32),
                        Color.FromArgb(66, 133, 244), Color.FromArgb(219, 68, 55)))
                    {
                        g.FillEllipse(brush, 2, 2, 28, 28);
                    }
                    
                    // Green stripe (Chrome-like design)
                    using (var brush = new SolidBrush(Color.FromArgb(52, 168, 83)))
                    {
                        g.FillRectangle(brush, 8, 12, 16, 8);
                    }
                }
                
                return Icon.FromHandle(bitmap.GetHicon());
            }
        }
    }
}