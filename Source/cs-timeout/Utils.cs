using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;

namespace cs_timed_silver
{
    internal class Utils
    {
        internal static Color MyDarkGray =
            GrayByPercent(28);
        internal static Color MyDarkDarkGray =
            GrayByPercent(59);

        /// <summary>
        /// Percent is bigger, results in lighter gray.
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        internal static Color GrayByPercent(int percent)
        {
            return Color.FromArgb((int)(percent / 100F * 255),
                (int)(percent / 100F * 255),
                (int)(percent / 100F * 255));
        }

        internal static bool TypeImplements(Type t, Type intface)
        {
            return t.GetInterfaces().Contains(intface);
        }

        internal static Image IconResourceVersionBySize(Icon icon, int width, int height = -1)
        {
            if (height == -1)
            {
                height = width;
            }
            var icon2 = new Icon(icon, width, height);
            Bitmap bmp = icon2.ToBitmap();
            return bmp;
        }

        internal static Bitmap ResizeToFitBoundingBox(Image image, in Rectangle box)
        {
            float maxHeight = box.Height;
            float maxWidth = box.Width;

            float x = Math.Min(maxWidth / image.Width,
                maxHeight / image.Height);

            float newW = (float)image.Width * x;
            float newH = (float)image.Height * x;

            var bmp = new Bitmap((int)Math.Ceiling(maxWidth),
                (int)Math.Ceiling(maxHeight));
            bmp.MakeTransparent(Color.Empty);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(image, (bmp.Width - newW) / 2,
                    (bmp.Height - newH) / 2, newW, newH);
            }

            return bmp;
        }

#if DEBUG
        internal static bool PrettyXML = true;
#else
        internal static bool PrettyXML = false;
#endif

        internal static string XmlDocumentToString(XmlDocument doc)
        {
            XmlWriter xw = null;
            try
            {
                var sb = new StringBuilder();
                var s = new XmlWriterSettings();
                s.Encoding = Encoding.Unicode;

                if (PrettyXML)
                {
                    s.Indent = true;
                    s.IndentChars = "\t";
                }

                xw = XmlWriter.Create(sb, s);
                doc.WriteTo(xw);
                xw.Flush();
                return sb.ToString();
            }
            finally
            {
                if (xw != null)
                {
                    xw.Close();
                }
            }
        }

        /// <summary>
        /// Returns the installation folder without the '\' at its end.
        /// </summary>
        /// <returns></returns>
        internal static string GetInstallationFolder()
        {
            string result = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // there always is a '\' char at the end
            int index = result.LastIndexOf("\\");

            return result.Substring(0, index);
        }

        internal static void OpenUrlInDefaultApp(string url,
            IWin32Window messageBoxParent = null)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                {
                    if (messageBoxParent == null)
                    {
                        MessageBox.Show(noBrowser.Message);
                    }
                    else
                    {
                        MessageBox.Show(messageBoxParent, noBrowser.Message);
                    }
                }
            }
            catch (Exception other)
            {
                if (messageBoxParent == null)
                {
                    MessageBox.Show(other.Message);
                }
                else
                {
                    MessageBox.Show(messageBoxParent, other.Message);
                }
            }
        }

        /// <summary>
        /// If the filePath's file does not exist, it tries to open its folder
        /// if it exists. It returns true the function either opened the
        /// folder, or also did the selection in the folder, false otherwise.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal static bool SelectFileInItsFolderInExplorer(string filePath, IWin32Window win = null)
        {
            if (!File.Exists(filePath))
            {
                string folderPath = Path.GetDirectoryName(Path.GetFullPath(filePath));

                // TODO: use WPF MessageBox class instead
                if (MessageBox.Show(win, "File does not exist anymore. Do you wish to open its folder?", "Confirmation request", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (Directory.Exists(folderPath))
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(
                                "explorer.exe", QuoteFilePathProcessArgument(folderPath));
                            return true;
                        }
                        catch (Exception other)
                        {
                            if (win == null)
                            {
                                MessageBox.Show(other.Message);
                            }
                            else
                            {
                                MessageBox.Show(win, other.Message);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show(win, $"Neither the directory nor the file do exist. For your information, the directory is the one containing this file: {filePath}", "Information");
                        return false;
                    }
                }
                return false;
            }

            try
            {
                System.Diagnostics.Process.Start(
                    "explorer.exe", "/select, " + QuoteFilePathProcessArgument(filePath));
                return true;
            }
            catch (Exception other)
            {
                if (win == null)
                {
                    MessageBox.Show(other.Message);
                }
                else
                {
                    MessageBox.Show(win, other.Message);
                }
                return false;
            }
        }

        internal static void WPFDoRightMouseButtonSelect(System.Windows.Controls.DataGrid dg, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.RightButton != System.Windows.Input.MouseButtonState.Pressed) { return; }

            // Use HitTest to resolve the row under the cursor
            System.Windows.Media.HitTestResult hitTestResult =
                System.Windows.Media.VisualTreeHelper.HitTest(dg, e.GetPosition(dg));
            System.Windows.Controls.DataGridRow dataGridRow = hitTestResult.VisualHit.GetParentOfType<System.Windows.Controls.DataGridRow>();

            if (dataGridRow == null)
            {
                // case: right click not on a row, but on a column header
                // (even the first column header in the corner, sometimes hidden)
                return;
            }

            // If there was no DataGridViewRow under the cursor, return
            if (hitTestResult == null)
            {
                // Clear all selections
                dg.UnselectAllCells();

                return;
            }

            if (dg.SelectedItems.Count <= 1)
            {
                // Clear all other selections before making a new selection
                dg.UnselectAllCells();

                // Select the found row
                dataGridRow.IsSelected = true;
            }
            else
            {
                bool rUnderCursorSelected = dataGridRow.IsSelected;

                if (rUnderCursorSelected)
                {
                    // Keep the selection
                }
                else
                {
                    // Clear all other selections before making a new selection
                    dg.UnselectAllCells();

                    // Select the found row
                    dataGridRow.IsSelected = true;
                }
            }
        }

        internal static bool HasAsParent<T>(System.Windows.DependencyObject dependencyObject)
        {
            System.Windows.DependencyObject parent = System.Windows.Media.VisualTreeHelper.GetParent(dependencyObject);

            while (!(parent is T) && parent != null)
            {
                parent = System.Windows.Media.VisualTreeHelper.GetParent(parent);
            }

            return parent != null;
        }

        internal static void RemoveAllSelectedRows(DataGridView dgv, Action<DataGridViewRow> afterRemoval = null)
        {
            var rc = new DataGridViewRow[dgv.SelectedRows.Count];
            dgv.SelectedRows.CopyTo(rc, 0);
            for (int i = 0; i < rc.Length; ++i)
            {
                if (rc[i].IsNewRow)
                {

                }
                else
                {
                    dgv.Rows.Remove(rc[i]);
                    if (afterRemoval != null)
                    {
                        afterRemoval(rc[i]);
                    }
                }
            }
        }

        // inspiration from https://stackoverflow.com/q/2393384/258462
        private static string QuoteFilePathProcessArgument(string filePath)
        {
            string s = filePath;
            s = s.Replace("^", "^^");
            s = s.Replace("&", "^&");
            s = s.Replace("|", "^|");
            s = s.Replace("(", "^(");
            s = s.Replace(")", "^)");
            s = s.Replace("<", "^<");
            s = s.Replace(">", "^>");
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        }

        internal static string BaseFileNameInPath(string fileName)
        {
            int index = fileName.LastIndexOf("\\");

            // if there is no '\' in the fileName string
            if (index < 0)
            {
                return fileName;
            }

            // else
            return fileName.Substring(index + 1);
        }

        internal static string ColorToString(in Color color)
        {
            byte a = color.A,
                r = color.R,
                g = color.G,
                b = color.B;
            var arr = new byte[] { a, r, g, b };
            string s = BitConverter.ToString(arr).Replace("-", "");
            return $"#{s}";
        }

        internal static Color StringToColor(string str)
        {
            try
            {
                if (str[0] == '#')
                {
                    int a = int.Parse(str.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                    int r = int.Parse(str.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                    int g = int.Parse(str.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                    int b = int.Parse(str.Substring(7, 2), System.Globalization.NumberStyles.HexNumber);

                    return Color.FromArgb(a, r, g, b);
                }
                else
                {
                    return Color.FromName(str);
                }
            }
            catch (Exception)
            {
                return Color.Empty;
            }
        }

#region Find if a Point is inside a polygon described using an array of Point-s.
        // Original code
        // from http://csharphelper.com/blog/2014/07/determine-whether-a-point-is-inside-a-polygon-in-c/.

        /// <summary>
        /// Returns true if the point (x, y) is in the polygon's surface.
        /// Does not support an empty `polygon` array.
        /// </summary>
        /// <param name="polygon">An unidimensional array containing the points
        /// that, when connected, form the polygon.</param>
        internal static bool PointInPolygon(PointF[] polygon,
            float x, float y)
        {
            // Get the angle between the (x, y) point and the
            // first and last points in the `polygon` array.
            int lastPointIndex = polygon.Length - 1;
            float totalAngle = GetAngle(
                polygon[lastPointIndex].X, polygon[lastPointIndex].Y,
                x, y,
                polygon[0].X, polygon[0].Y);

            // Add the angles from the point (x, y)
            // to each other pair of points next to
            // each other.
            for (int i = 0; i < lastPointIndex; ++i)
            {
                totalAngle += GetAngle(
                    polygon[i].X, polygon[i].Y,
                    x, y,
                    polygon[i + 1].X, polygon[i + 1].Y);
            }

            // Theoretically the total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon, but checking the
            // equality of the absolute value of the angle with
            // 2 * Math.PI does not function, it is always false.
            return Math.Abs(totalAngle) > 0.000001;
        }

        /// <summary>
        /// Return the angle ABC.
        /// Return a value between PI and -PI.
        /// Note that the value is the opposite of what you might
        /// expect because Y coordinates increase downward.
        /// 
        /// Theory:
        /// http://mathworld.wolfram.com/DotProduct.html &
        ///  https://en.wikipedia.org/wiki/Dot_product &
        ///  https://www.mathsisfun.com/algebra/vectors-dot-product.html
        /// https://www.mathsisfun.com/algebra/vectors-cross-product.html
        /// 
        /// <c>crossProduct</c> is expressed in function of <c>sin</c>.
        /// <c>dotProduct</c> is expressed in function of <c>cos</c>.
        /// <c>crossProduct / dotProduct = sin(B) / cos(B) = tan(B)</c>.
        /// <c>atan2(crossProduct, dotProduct) =
        ///     = atan(crossProduct / dotProduct) = atan(tan(B)) = B (in radians)</c>
        /// </summary>
        internal static float GetAngle(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the dot product.
            float dotProduct = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            float crossProduct = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.

            // **What angle has tan the same as the sin of angle ABC (angle B)?**
            float angle = (float)Math.Atan2(crossProduct, dotProduct);
            //    wrong when (crossProduct == 0 || dotProduct < 0):
            //float angle2 = (float)Math.Atan(crossProduct / dotProduct);
            //if (Convert.ToInt32(angle * 1000) != Convert.ToInt32(angle2 * 1000))
            //{
            //    this is how to test the cases when Atan does function as Atan2
            //    by producing bugs
            //}

#region Solution using Law of sines with small bug
            // BUG: does not return negative/exterior angles when it should,
            // but the absolute value is always correct.
            /*
            float AB = (float)Math.Sqrt((Ax - Bx) * (Ax - Bx) +
                (Ay - By) * (Ay - By));
            float BC = (float)Math.Sqrt((Bx - Cx) * (Bx - Cx) +
                (By - Cy) * (By - Cy));
            float AC = (float)Math.Sqrt((Ax - Cx) * (Ax - Cx) +
                (Ay - Cy) * (Ay - Cy));

            // Triangle area through the Heron's formula
            float p = (AB + AC + BC) / 2; // half of perimeter
            // S - triangle area:
            float S = (float) Math.Sqrt(p * (p - AB) * (p - AC) * (p - BC));
            // using Law of sines as a proportion
            float sinB = (2 * AC * S) / (AB * BC * AC); // simplificable by AC

            return (float)Math.Asin(sinB); // in radians
            */
#endregion

#region [gives wrong value] Solution using the Law of cosines
            // needs variables from the previous code region:
            //float angle2 = (float)Math.Acos((AB * AB + AC * AC - BC * BC) /
            //    (2 * AC * BC));
#endregion

#region Solution with 7+ pozitive votes from SO, gives wrong value
            // https://stackoverflow.com/a/9023595/258462
            //float v1x = Bx - Cx;
            //float v1y = By - Cy;
            //float v2x = Ax - Cx;
            //float v2y = Ay - Cy;

            //angle = (float)Math.Atan(v1x / v1y) - (float)Math.Atan(v2x / v2y);
#endregion

            return angle;
        }

        // Return the cross product AB x BC.
        // The cross product is a vector perpendicular to AB
        // and BC having length |AB| * |BC| * Sin(theta) and
        // with direction given by the right-hand rule.
        // For two vectors in the X-Y plane, the result is a
        // vector with X and Y components 0 so the Z component
        // gives the vector's length and direction.
        internal static float CrossProductLength(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return BAx * BCy - BAy * BCx;
        }

        // Return the dot product AB · BC.
        // Note that AB · BC = |AB| * |BC| * Cos(theta).
        internal static float DotProduct(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the dot product.
            return BAx * BCx + BAy * BCy;
        }
#endregion
        
        internal static bool ColorsAreTheSame(in Color a, in Color b)
        {
            return a.A == b.A &&
                a.R == b.R &&
                a.G == b.G &&
                a.B == b.B;
        }

        /// <summary>
        /// Converts the given string to the corresponding value inside the
        /// given enumeration.
        /// </summary>
        /// <typeparam name="EnumT">typeof an Enum (enumeration)</typeparam>
        /// <param name="nameInsideEnumT">string of name inside the given EnumT</param>
        /// <exception cref="ArgumentNullException">
        /// nameInsideEnumT is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// EnumT is not an Enum. 
        /// nameInsideEnumT is either an empty string or only contains white space. 
        /// nameInsideEnumT is a name, but not one of the named constants defined for the enumeration.
        /// </exception>
        /// <exception cref="OverflowException">
        /// nameInsideEnumT is outside the range of the underlying type of enumType.
        /// </exception>
        /// <returns></returns>
        internal static EnumT StrToEnum<EnumT>(string nameInsideEnumT,
            bool caseInsensitive = true)
        {
            EnumT p;
            
            p = (EnumT)Enum.Parse(typeof(EnumT),
                nameInsideEnumT, caseInsensitive);

            return p;
        }

        internal static Image Base64StringToImage(string b64)
        {
            byte[] imageBytes = Convert.FromBase64String(b64);
            using (var ms = new MemoryStream(imageBytes,
                0, imageBytes.Length))
            {
                var img = Image.FromStream(ms, true);
                return img;
            }
        }

        internal static string ImageToBase64String(Image img)
        {
            if (img == null)
            {
                throw new ArgumentNullException("img");
            }

            using (var m = new MemoryStream())
            {
                var bmp = img as Bitmap;
                bmp.Save(m, ImageFormat.Bmp);
                byte[] imageBytes = m.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static string ImageToBase64(BitmapSource bitmap)
        {
            var encoder = new PngBitmapEncoder();
            var frame = BitmapFrame.Create(bitmap);
            encoder.Frames.Add(frame);
            using (var stream = new MemoryStream())
            {
                encoder.Save(stream);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static BitmapSource Base64ToImage(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            using (var stream = new MemoryStream(bytes))
            {
                return BitmapFrame.Create(stream);
            }
        }

        internal static Bitmap RoundCorners(Image startImage,
            int cornerRadius, in Color backgroundColor)
        {
            cornerRadius *= 2;
            var roundedImage = new Bitmap(startImage.Width, startImage.Height);
            using (Graphics g = Graphics.FromImage(roundedImage))
            {
                g.Clear(backgroundColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Brush brush = new TextureBrush(startImage);

                using (var gp = new GraphicsPath())
                {
                    gp.AddArc(0, 0, cornerRadius, cornerRadius, 180, 90);
                    gp.AddArc(0 + roundedImage.Width - cornerRadius, 0, cornerRadius, cornerRadius, 270, 90);
                    gp.AddArc(0 + roundedImage.Width - cornerRadius, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 0, 90);
                    gp.AddArc(0, 0 + roundedImage.Height - cornerRadius, cornerRadius, cornerRadius, 90, 90);

                    g.FillPath(brush, gp);
                }

                return roundedImage;
            }
        }

        public static Control FindControlWithFocus(Control parent)
        {
            if (parent.Focused)
            {
                return parent;
            }

            foreach (Control ctl in parent.Controls)
            {
                if (ctl.Focused)
                {
                    return ctl;
                }

                Control child = FindControlWithFocus(ctl);

                if (child != null)
                {
                    return child;
                }
            }

            return null;
        }

        public static Control FindControlWithType(Control parent, Type t)
        {
            if (parent.GetType() == t)
            {
                return parent;
            }

            foreach (Control ctl in parent.Controls)
            {
                if (ctl.GetType() == t)
                {
                    return ctl;
                }

                Control child = FindControlWithType(ctl, t);

                if (child != null)
                {
                    return child;
                }
            }

            return null;
        }

        public static Control FindControlWithInterface(Control parent, Type t)
        {
            if (TypeImplements(parent.GetType(), t))
            {
                return parent;
            }

            foreach (Control ctl in parent.Controls)
            {
                if (TypeImplements(ctl.GetType(), t))
                {
                    return ctl;
                }

                Control child = FindControlWithInterface(ctl, t);

                if (child != null)
                {
                    return child;
                }
            }

            return null;
        }

        internal static void SetStartProgramWithWindows(bool yes)
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
            if (yes)
            {
                key.SetValue(Application.ProductName,
                    Application.ExecutablePath.ToString());
            }
            else
            {
                key.DeleteValue(Application.ProductName, false);
            }
        }

        internal static bool ProgramStartsWithWindows()
        {
            var path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
            object val = key.GetValue(Application.ProductName);
            return val != null &&
                val.ToString() == Application.ExecutablePath.ToString();
        }

        internal static System.Windows.Forms.OpenFileDialog OnlyImageChooser = null;
        internal static System.Windows.Forms.SaveFileDialog OnlyImageSaveChooser = null;
        internal static System.Windows.Forms.OpenFileDialog OnlyAudioChooserDialog = null;
        
        // TODO: [VISUAL] Save the last used folder.
        internal static System.Windows.Forms.OpenFileDialog GetImageChooser()
        {
            if (OnlyImageChooser == null)
            {
                var fd = new System.Windows.Forms.OpenFileDialog();
                fd.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp, *.gif) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp; *.gif|All files (*.*)|*.*";
                fd.ReadOnlyChecked = true;
                fd.FileName = "";
                fd.InitialDirectory = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
                fd.RestoreDirectory = true;
                OnlyImageChooser = fd;
            }

            return OnlyImageChooser;
        }

        internal static System.Windows.Forms.SaveFileDialog GetImageSaveChooser()
        {
            if (OnlyImageSaveChooser == null)
            {
                var fd = new System.Windows.Forms.SaveFileDialog();
                fd.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|Emf Image|*.emf|Icon Image|*.ico|Png Image|*.png|Tiff Image|*.tiff";
                fd.FileName = "";
                fd.InitialDirectory = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
                fd.RestoreDirectory = true;

                OnlyImageSaveChooser = fd;
            }

            return OnlyImageSaveChooser;
        }

        internal static string[] SupportedImageFileExtensions()
        {
            return new string[]
            {
                "jpg",
                "jpeg",
                "jpe",
                "jfif",
                "png",
                "bmp",
                "gif"
            };
        }

        internal static string[] SupportedAudioFileExtensions()
        {
            return new string[]
            {
                "3g2",
                "3gp",
                "3gp2",
                "3gpp",

                "aac",
                "adts",

                "aiff",
                "aif",
                "aifc",

                "asf",
                "wma",

                "flac",
                "mp3",

                "m4a",

                "wav"
            };
        }

        internal static System.Windows.Forms.OpenFileDialog GetAudioChooserDialog()
        {
            if (OnlyAudioChooserDialog == null)
            {
                var openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
                openFileDialog1.DefaultExt = "mp3";

                // keep the filters ordered alphabetically:
                openFileDialog1.Filter =
                    "3GP files (*.3g2; *.3gp; *.3gp2; *.3gpp)|*.3g2;*.3gp;*.3gp2;*.3gpp|" +
                    "ADTS files (*.aac; *.adts)|*.aac;*.adts|" +
                    "AIFF files (*.aiff; *.aif; *.aifc)|*.aiff;*.aif;*.aifc|" +
                    "ASF files (*.asf; *.wma)|*.asf;*.wma|" +
                    "FLAC files (*.flac)|*.flac|" +
                    "MP3 files (*.mp3)|*.mp3|" +
                    "MPEG-4 files (*.m4a)|*.m4a|" +
                    //"OGG files (*.ogg)|*.ogg|" + // needs a separate library
                    "WAVE files (*.wav)|*.wav|" +
                    "All files (*.*)|*.*";

                openFileDialog1.FilterIndex = 6;
                openFileDialog1.ReadOnlyChecked = true;
                openFileDialog1.RestoreDirectory = true;
                OnlyAudioChooserDialog = openFileDialog1;
            }

            return OnlyAudioChooserDialog;
        }

        internal static ColorDialog OnlyColorChooser = null;
        internal static ColorDialog GetColorChooser(Color initialColor)
        {
            if (OnlyColorChooser == null)
            {
                var cd = new ColorDialog();
                cd.FullOpen = true;
                cd.SolidColorOnly = false;
                cd.Color = initialColor;
                OnlyColorChooser = cd;
            }

            return OnlyColorChooser;
        }

        internal static Microsoft.Win32.OpenFileDialog OnlyDataFileChooserWPF = null;

        internal static Microsoft.Win32.OpenFileDialog GetDataFileOpenerWPF()
        {
            if (OnlyDataFileChooserWPF == null)
            {
                var fd = new Microsoft.Win32.OpenFileDialog();
                fd.Filter = "XML files|*.xml|All files (*.*)|*.*";
                fd.FilterIndex = 1;
                fd.DefaultExt = "xml";
                fd.ReadOnlyChecked = true;
                fd.FileName = "";
                fd.InitialDirectory = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
                fd.RestoreDirectory = true;

                OnlyDataFileChooserWPF = fd;
            }

            return OnlyDataFileChooserWPF;
        }

        internal static System.Windows.Forms.OpenFileDialog OnlyDataFileChooser = null;

        internal static System.Windows.Forms.OpenFileDialog GetDataFileOpener()
        {
            if (OnlyDataFileChooser == null)
            {
                var fd = new System.Windows.Forms.OpenFileDialog();
                fd.Filter = "XML files|*.xml|All files (*.*)|*.*";
                fd.FilterIndex = 1;
                fd.DefaultExt = "xml";
                fd.ReadOnlyChecked = true;
                fd.FileName = "";
                fd.InitialDirectory = Environment.GetFolderPath(
                    Environment.SpecialFolder.Personal);
                fd.RestoreDirectory = true;
                OnlyDataFileChooser = fd;
            }

            return OnlyDataFileChooser;
        }

        internal static Rectangle TruncateRectF(RectangleF r)
        {
            return new Rectangle((int)Math.Round(r.X),
                (int)Math.Round(r.Y),
                (int)Math.Round(r.Width),
                (int)Math.Round(r.Height));
        }

        internal static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        
        internal static bool ContainsReference<T>(IEnumerable<T> l, object o)
        {
            foreach (T obj in l)
            {
                if (ReferenceEquals(obj, o))
                {
                    return true;
                }
            }
            return false;
        }

        internal static Cursor CreateCursorFromControl(Control c, float radius, ref Bitmap cursorBitmap)
        {
            var bmp = new Bitmap(c.Width, c.Height);

            c.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size));
            if (radius > 0)
            {
                var bmp2 = Utils.RoundCorners(bmp, (int)radius,
                    Color.Empty) as Bitmap;
                bmp.Dispose();
                bmp = bmp2;
            }

            cursorBitmap = bmp;

            return new Cursor(bmp.GetHicon());
        }

        internal static bool CursorsAreEqual(Cursor cur1, Cursor cur2)
        {
            byte[] bytes1, bytes2;

            using (var ico = Icon.FromHandle(cur1.Handle))
            {
                using (var fs = new MemoryStream())
                {
                    ico.Save(fs);
                    bytes1 = fs.ToArray();
                }
            }

            using (Icon ico = Icon.FromHandle(cur2.Handle))
            {
                using (var fs = new MemoryStream())
                {
                    ico.Save(fs);
                    bytes2 = fs.ToArray();
                }
            }
            
            return bytes1.SequenceEqual(bytes2);
        }
        
        internal static IEnumerable<T> AddEnumerablesDistinct<T>(IEnumerable<T> l1, IEnumerable<T> l2)
        {
            var l = new List<T>(l1);
            l.AddRange(l2);
            return l.Distinct();
        }

        internal static bool IsNullOrDisposed(Control c)
        {
            return c == null || c.IsDisposed;
        }

        internal static void SuspendLayoutRecursively(Control c)
        {
            c.SuspendLayout();
            foreach (Control cc in c.Controls)
            {
                SuspendLayoutRecursively(cc);
            }
        }

        internal static void ResumeLayoutRecursively(Control c, bool performLayout = true)
        {
            c.ResumeLayout(performLayout);
            foreach (Control cc in c.Controls)
            {
                ResumeLayoutRecursively(cc, performLayout);
            }
        }

        internal static bool NearlyEqual(double a, double b, double epsilon)
        {
            const double MinNormal = 2.2250738585072014E-308d;
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if (a.Equals(b))
            {
                // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || diff < MinNormal)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * MinNormal);
            }
            else
            {
                // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }

        internal static string RemoveWhitespaceFromXML(string s)
        {
            var regex = new Regex(@">\s*<");
            return regex.Replace(s, "><");
        }

        // Icon.FromHandle(ico.GetHicon()) not working
        /// <summary>
        /// Use with `using`.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        internal static Icon IconFromImage(Image img)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            // Header
            bw.Write((short)0);   // 0 : reserved
            bw.Write((short)1);   // 2 : 1=ico, 2=cur
            bw.Write((short)1);   // 4 : number of images
                                  // Image directory
            int w = img.Width;
            if (w >= 256) w = 0;
            bw.Write((byte)w);    // 0 : width of image
            int h = img.Height;
            if (h >= 256) h = 0;
            bw.Write((byte)h);    // 1 : height of image
            bw.Write((byte)0);    // 2 : number of colors in palette
            bw.Write((byte)0);    // 3 : reserved
            bw.Write((short)0);   // 4 : number of color planes
            bw.Write((short)0);   // 6 : bits per pixel
            long sizeHere = ms.Position;
            bw.Write((int)0);     // 8 : image size
            int start = (int)ms.Position + 4;
            bw.Write(start);      // 12: offset of image data
                                  // Image data
            img.Save(ms, ImageFormat.Png);
            int imageSize = (int)ms.Position - start;
            ms.Seek(sizeHere, SeekOrigin.Begin);
            bw.Write(imageSize);
            ms.Seek(0, SeekOrigin.Begin);

            // And load it
            return new Icon(ms);
        }

        public static T GetVisualParent<T>(object childObject) where T : System.Windows.Media.Visual
        {
            var child = childObject as System.Windows.DependencyObject;
            // iteratively traverse the visual tree
            while ((child != null) && !(child is T))
            {
                if (!(child is System.Windows.Media.Visual ||
                    child is System.Windows.Media.Media3D.Visual3D))
                {
                    return null;
                }
                // GetParent accepts only Visual || Visual3D:
                child = System.Windows.Media.VisualTreeHelper.GetParent(child);
            }
            return child as T;
        }

        public static T GetLogicalParent<T>(object childObject) where T : System.Windows.Media.Visual
        {
            var child = childObject as System.Windows.DependencyObject;
            // iteratively traverse the visual tree
            while ((child != null) && !(child is T))
            {
                child = System.Windows.LogicalTreeHelper.GetParent(child);
            }
            return child as T;
        }

        public static List<T> GetVisualChildCollection<T>(object parent) where T : System.Windows.Media.Visual
        {
            var visualCollection = new List<T>();
            GetVisualChildCollection(parent as System.Windows.DependencyObject, visualCollection);
            return visualCollection;
        }

        public static void GetVisualChildCollection<T>(System.Windows.DependencyObject parent, List<T> visualCollection) where T : System.Windows.Media.Visual
        {
            int count = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                System.Windows.DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                else if (child != null)
                {
                    GetVisualChildCollection(child, visualCollection);
                }
            }
        }

        internal static T GetVisualChild<T>(System.Windows.DependencyObject parent) where T : System.Windows.Media.Visual
        {
            T child = default(T);

            int numVisuals = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                var v = (System.Windows.Media.Visual)System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static Point GetMousePositionWindowsForms()
        {
            System.Drawing.Point point = Control.MousePosition;
            return new Point(point.X, point.Y);
        }

        public static System.Windows.Media.ImageSource GetBitmapImageFromBitmap(
            Bitmap ico)
        {
            if (ico == null) return null;

            var stream = new MemoryStream();
            ico.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            return System.Windows.Media.Imaging.BitmapFrame.Create(stream);
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(System.Windows.DependencyObject parent, string childName)
            where T : System.Windows.DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as System.Windows.FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        internal static void SynchronizeCollectionChange<T, U>(
            ObservableCollection<T> source,
            NotifyCollectionChangedEventArgs e,
            ObservableCollection<U> target,
            Action<U> afterAddition = null,
            Action<T> prepareDeletion = null,
            Func<T, U, bool> equalsWithinTargetTo = null,
            Func<T, U> toTarget = null,
            int startingIndexInSource = 0,
            int startingIndexInTarget = 0)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (T item in e.NewItems)
                    {
                        var mItem = (T)item;
                        int idx = source.IndexOf(mItem);

                        // if it doesn't exist in target, put it in target at idx, not with Add method
                        //int uIdx = target.IndexOf(toTarget(mItem));

                        //int newIndex = idx;
                        //if (startingIndexInTarget != -1) // only from this index will the addition take place
                        //{
                        //    if (idx <startingIndexInTarget) // if the index in sourceis lower than the starting index in source that you  take care of
                        //    {
                        //        continue; // go to next added item, ignore this one
                        //    }
                        //    //// else 
                        //    //newIndex = startingIndexInSource + idx;
                        //}
                        //else
                        //{
                        //    newIndex = idx;
                        //}


                        // if the index in target is... it does not matter... the new index is computed by adding starting
                        //if(uIdx < startingIndexInTarget)
                        //{

                        //}

                        // the operation could have been either Insert or Add
                        //if (0 <= uIdx && uIdx <= source.Count - 1)
                        //{
                        //    target.Insert(newIndex, toTarget(mItem)); // move
                        //}
                        //else
                        //{
                        int ni = idx - startingIndexInSource + startingIndexInTarget;

                        if (ni < 0 || ni > target.Count)
                        {
                            // the item must not be copied to target, it is a basic (auto) filter
                            break;
                        }

                        var mInTarget = toTarget(mItem);

                        // TODO: what if mItem is null?:
                            target.Insert(ni, mInTarget); // add

                            //target.Add(toTarget(mItem));

                            afterAddition?.Invoke(mInTarget);
                        //}
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (T item in e.OldItems)
                    {
                        // here the T item is already deleted from source

                        // this should cvt T item to an U , but not when deleting it!
                        //U mItem = toTarget(item); // does not know that and that

                        //if (mItem == null)
                        //{
                        //    return;
                        //}

                        prepareDeletion?.Invoke(item);

                        // here the T item is already deleted from source

                        // find VM objects that wrap the relevant model object and remove them
                        IEnumerable<U> query;
                        while ((query = from it in target
                                        where equalsWithinTargetTo(item, it) // based on fact that U : T
                                        select it).Count() > 0)
                        {
                            U m = query.First();

                            //if (startingIndexInTarget != -1)
                            //{
                            //    if (target.IndexOf(m) < startingIndexInTarget)
                            //    {
                            //        continue;
                            //    }
                            //}

                            //int index = target.IndexOf(m);
                            target.Remove(x => ReferenceEquals(x, m));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    if (startingIndexInTarget != 0 || startingIndexInSource != 0)
                    {
                        for (int i = target.Count - 1; i >= 0; --i)
                        {
                            int ni = i - startingIndexInSource + startingIndexInTarget;

                            if (ni < 0 || ni >= target.Count)
                            {
                                // the item must not be cleared from target on reset, it is a basic (auto) filter
                                continue;
                            }

                            target.RemoveAt(i);
                        }
                    }
                    else
                    {
                        target.Clear();
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    {
                        //if (startingIndexInTarget != -1 &&
                        //    (e.OldStartingIndex < startingIndexInTarget ||
                        //    e.NewStartingIndex < startingIndexInTarget))
                        //{
                        //    break;
                        //}

                        // TODO: handle multiple items

                        int oi = e.OldStartingIndex - startingIndexInSource + startingIndexInTarget;
                        int ni = e.NewStartingIndex - startingIndexInSource + startingIndexInTarget;

                        if (ni < 0 || ni > target.Count)
                        {
                            break;
                        }

                        if (oi < 0 || oi > target.Count)
                        {
                            break;
                        }

                        target.Move(oi, ni);
                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        //if (startingIndexInTarget != -1 &&
                        //    (e.OldStartingIndex < startingIndexInTarget ||
                        //    e.NewStartingIndex < startingIndexInTarget))
                        //{
                        //    break;
                        //}

                        int oi = e.OldStartingIndex - startingIndexInSource + startingIndexInTarget;

                        if (oi < 0 || oi > target.Count)
                        {
                            break;
                        }

                        // TODO: handle multiple items
                        target[oi] = toTarget((T)e.NewItems[0]);
                        break;
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
