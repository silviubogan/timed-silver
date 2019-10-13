using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_timed_silver
{
    /// <summary>
    /// TODO: rename Form/s to Window/s in this class (including its name & file name).
    /// </summary>
    internal class TimeOutFormsManager
    {
        internal Dictionary<ClockM, TimeOutWindow> MyForms = null;

        internal MainWindow MyMainWindow
        {
            get
            {
                return System.Windows.Application.Current.MainWindow as MainWindow;
            }
        }

        internal TimeOutFormsManager()
        {
            MyForms = new Dictionary<ClockM, TimeOutWindow>();
        }

        internal void ShowForm(ClockM cd)
        {
            if (MyForms.ContainsKey(cd))
            {
                return;
            }

            MyMainWindow.VM.MultiAudioPlayer.AddClockData(cd);
            MyMainWindow.VM.MultiAudioPlayer.PlaySound();


            var MyTimeOutForm = new TimeOutWindow()
            {
                Clock = cd
            };
            MyTimeOutForm.IsVisibleChanged +=
                TimeOutForm_VisibleChanged;

            MyTimeOutForm.Show();

            MyTimeOutForm.Activate();

            MyForms[cd] = MyTimeOutForm;
        }

        /// <summary>
        /// Does smth only when the time-out form is invisible.
        /// The manager expects that TimeOutForm-s call Hide,
        /// not Close inside them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeOutForm_VisibleChanged(object sender,
            System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var s = sender as TimeOutWindow;
            if (!MyForms.ContainsValue(s))
            {
                return;
            }

            if (!s.IsVisible)
            {
                CloseForm(s.Clock);
                MyMainWindow.VM.MultiAudioPlayer.
                    RemoveClockMAndStopSoundIfNeeded
                        (s.Clock);
            }
        }

        internal void CloseForm(ClockM cd)
        {
            if (MyForms.ContainsKey(cd))
            {
                MyForms[cd].Close();
                MyForms.Remove(cd);
            }
        }

        public bool HasVisibleForm
        {
            get
            {
                foreach (KeyValuePair<ClockM, TimeOutWindow> p in MyForms)
                {
                    if (p.Value.IsVisible)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void HideForm(ClockM cd)
        {
            TimeOutWindow f = MyForms[cd];
            if (f != null)
            {
                f.Hide();
            }
        }
    }
}
