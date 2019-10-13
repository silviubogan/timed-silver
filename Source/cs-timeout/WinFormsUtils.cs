using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cs_timed_silver
{
    internal class WinFormsUtils
    {
        internal static Icon IconResourceVersionBySizeAsIcon(Icon icon, int width, int height = -1)
        {
            if (height == -1)
            {
                height = width;
            }
            return new Icon(icon, width, height);
        }

        internal static SolidBrush BrushFromArgb(int r, int g, int b,
            int a = 255)
        {
            return new SolidBrush(Color.FromArgb(a, r, g, b));
        }

        internal static void DoRightMouseButtonSelect(DataGridView dgv, MouseEventArgs e)
        {
            // If the user pressed something else than mouse right click, return
            if (e.Button != MouseButtons.Right) { return; }

            // Use HitTest to resolve the row under the cursor
            int rowIndex = dgv.HitTest(e.X, e.Y).RowIndex;

            // If there was no DataGridViewRow under the cursor, return
            if (rowIndex == -1)
            {
                // Clear all selections
                dgv.ClearSelection();

                return;
            }

            if (dgv.SelectedRows.Count <= 1)
            {
                // Clear all other selections before making a new selection
                dgv.ClearSelection();

                // Select the found DataGridViewRow
                dgv.Rows[rowIndex].Selected = true;
            }
            else
            {
                bool rUnderCursorSelected = dgv.Rows[rowIndex].Selected;

                if (rUnderCursorSelected)
                {
                    // Keep the selection
                }
                else
                {
                    // Clear all other selections before making a new selection
                    dgv.ClearSelection();

                    // Select the found DataGridViewRow
                    dgv.Rows[rowIndex].Selected = true;
                }
            }
        }

        internal static Color GetOppositeColor(in Color c)
        {
            const int RGBMAX = 255;

            return Color.FromArgb(RGBMAX - c.A,
                RGBMAX - c.R,
                RGBMAX - c.G,
                RGBMAX - c.B);
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect,
                        0, 0,
                        image.Width, image.Height,
                        GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        internal static string GetFolderPathOfFile(string fileName)
        {
            int index = fileName.LastIndexOf("\\");

            if (index <= 0)
            {
                return fileName;
            }

            return fileName.Substring(0, index);
        }

        internal static Rectangle GetControlVisibleRectangle(Control c)
        {
            var parentRect = new WinAPI.RECT();
            var intersectRect = new WinAPI.RECT();

            var parentRectangle = new Rectangle();
            var intersectRectangle = new Rectangle();

            // Get the current Handle.
            IntPtr currentHandle = c.Handle;

            // Get next Parent Handle.
            IntPtr parentHandle = c.Parent == null ? IntPtr.Zero : c.Parent.Handle;

            // Get the Rect for the current Window.
            WinAPI.GetWindowRect(new HandleRef(c, c.Handle), out intersectRect);

            // Load Current Window Rect into a System.Drawing.Rectangle
            intersectRectangle = new Rectangle(
                intersectRect.Left,
                intersectRect.Top,
                intersectRect.Right - intersectRect.Left,
                intersectRect.Bottom - intersectRect.Top);

            // Itterate through all parent windows and get the overlap of the visible areas to find out what's actually visible.
            while (parentHandle != IntPtr.Zero)
            {
                // Get the Rect for the Parent Window.
                WinAPI.GetWindowRect(
                    new HandleRef(c, parentHandle),
                    out parentRect);

                parentRectangle = new Rectangle(
                    parentRect.Left,
                    parentRect.Top,
                    parentRect.Right - parentRect.Left,
                    parentRect.Bottom - parentRect.Top);

                // Get the intersection between the current and parent window.
                intersectRectangle.Intersect(parentRectangle);

                // Set up for next loop.
                currentHandle = parentHandle;
                parentHandle = WinAPI.GetParent(currentHandle);
            }

            return intersectRectangle;
        }

        internal static Bitmap CropImage(Image img, Rectangle cropArea)
        {
            var bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        internal static Bitmap PrintClientRectangleToImage(Form f)
        {
            var bmp = new Bitmap(f.ClientSize.Width, f.ClientSize.Height);
            using (Graphics bmpGraphics = Graphics.FromImage(bmp))
            {
                IntPtr bmpDC = bmpGraphics.GetHdc();
                using (Graphics formGraphics = Graphics.FromHwnd(f.Handle))
                {
                    IntPtr formDC = formGraphics.GetHdc();
                    WinAPI.BitBlt(bmpDC, 0, 0, f.ClientSize.Width, f.ClientSize.Height, formDC, 0, 0, WinAPI.SRCCOPY);
                    formGraphics.ReleaseHdc(formDC);
                }
                bmpGraphics.ReleaseHdc(bmpDC);
            }
            return bmp;
        }

        internal static GraphicsPath RoundedCornerRectangle(Rectangle r)
        {
            var path = new GraphicsPath();
            float radius = 10;
            float size = radius * 2F;

            path.StartFigure();

            path.AddArc(r.X, r.Y,
                size, size, 180, 90);

            //GraphicsPathWithBorder.AddLine(new Point((int)CornerRadius, 0),
            //    new Point((int)(c.Width - CornerRadius), 0));
            //GraphicsPathWithBorder.AddLine(new Point((int)CornerRadius, 2),
            //    new Point((int)(c.Width - CornerRadius), 2));

            //NormalGraphicsPath.AddLine(new Point((int)newRectangle.X + (int)radius, newRectangle.Y),
            //    new Point((int)(newRectangle.X + newRectangle.Width - radius), newRectangle.Y));

            //gp.AddArc(newRectangle.X, newRectangle.Y,
            //    size, size, 180, 90);
            //gp.AddArc(newRectangle.X + (newRectangle.Width - size),
            //    newRectangle.Y,
            //    size, size, 270, 90);
            //gp.AddArc(newRectangle.X + (newRectangle.Width - size),
            //    newRectangle.Y + (newRectangle.Height - size),
            //    size, size, 0, 90);
            //gp.AddArc(newRectangle.X,
            //    newRectangle.Y + (newRectangle.Height - size),
            //    size, size, 90, 90);

            path.AddArc(r.Right - size, r.Y,
                size, size, 270, 90);
            path.AddArc(r.Right - size, r.Bottom - size,
                size, size, 0, 90);
            path.AddArc(r.X, r.Bottom - size,
                size, size, 90, 90);

            path.CloseFigure();

            return path;
        }

        internal static void UpdateRoundedCornersUserControl(UserControl c,
            GraphicsPath GraphicsPathWithBorder,
            bool roundedCorners)
        {
            if (roundedCorners)
            {
                c.Region = new Region(GraphicsPathWithBorder);
                c.BorderStyle = BorderStyle.None;
            }
            else
            {
                Rectangle r = c.ClientRectangle;
                r.Inflate(5, 5); // w/o this the FixedSingle border is shown only on top and on left, not on the sides right & bottom.
                c.Region = new Region(r);

                c.BorderStyle = BorderStyle.FixedSingle;
            }
        }
    }
}
