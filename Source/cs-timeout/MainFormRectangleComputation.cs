using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    public class MainFormRectangleComputation : Computation<SettingDataM>
    {
        public MainWindow MyMainWindow = null;

        public MainFormRectangleComputation()
        {
            MyMainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
        }

        public override object Compute(SettingDataM arg)
        {
            System.Windows.Rect r = MyMainWindow.RestoreBounds;

            return System.Drawing.Rectangle.FromLTRB(
                (int)Math.Round(r.Left),
                (int)Math.Round(r.Top),
                (int)Math.Round(r.Right),
                (int)Math.Round(r.Bottom));
        }
    }
}
