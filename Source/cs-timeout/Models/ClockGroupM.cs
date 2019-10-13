using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace cs_timed_silver
{
    public class ClockGroupM : BindableBaseExtended
    {
        private string _Name = "";
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        private bool _IsSelected = false;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { SetProperty(ref _IsSelected, value); }
        }

        private ImageSource _Icon = null;
        public ImageSource Icon
        {
            get { return _Icon; }
            set { SetProperty(ref _Icon, value); }
//                if (_Icon != value)
//                {
//                    if (value != null)
//                    {
//                        // resize while switching to new aspect ratio (square 200 x 200)

//                        float maxHeight = 200;
//        float maxWidth = 200;

//        var r = new Rectangle(0,
//            0,
//            (int)Math.Round(maxWidth),
//            (int)Math.Round(maxHeight)
//        );

//        Bitmap bmp = Utils.ResizeToFitBoundingBox(value, r);

//        _Icon = value;// bmp;
//                    }
//                    else
//                    {
//                        _Icon = null;
//                    }

////TODO: inspiration: * efficiently* resize image:
//                    ico = Utils.ResizeImage(ico, ico.Width, ico.Height);
//                }
//            }
        }

        public override bool Equals(object obj)
        {
            var o = obj as ClockGroupM;

            if (ReferenceEquals(o, null))
            {
                return false;
            }

            return base.Equals(obj) &&
                Name == o.Name &&
                Icon == o.Icon;
        }

        public static bool operator ==(ClockGroupM m1,
            ClockGroupM m2)
        {
            if (ReferenceEquals(m1, null))
            {
                return ReferenceEquals(m2, null);
            }
            return m1.Equals(m2);
        }

        public static bool operator !=(ClockGroupM m1,
            ClockGroupM m2)
        {
            return !(m1 == m2);
        }
    }
}
