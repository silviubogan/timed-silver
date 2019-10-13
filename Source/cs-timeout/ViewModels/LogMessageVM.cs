using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace cs_timed_silver
{
    public class LogMessageVM : LogMessageM
    {
        internal DateToDayDateConverter Converter = new DateToDayDateConverter();

        protected ImageSource _Image = null;
        public ImageSource Image
        {
            get { return _Image; }
            set {
                SetProperty(ref _Image, value, new Action(() =>
                {
                    RaisePropertyChanged("Image");
                }));
            }
        }

        public string DateString
        {
            get
            {
                return ((DateTime)Converter.Convert(DateTime, typeof(DateTime), null, null)).
                    ToShortDateString();
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Category")
            {
                if (Category == LogCategory.Information)
                {
                    Image = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                else if (Category == LogCategory.Error)
                {
                    Image = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Error.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else if (e.PropertyName == "DateTime")
            {
                RaisePropertyChanged("DateString");
            }

            base.OnPropertyChanged(e);
        }
    }
}
