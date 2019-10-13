using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Xml;
using System.Windows.Threading;

namespace cs_timed_silver
{
    internal class BeepCollection : ObservableCollection<Beep>
    {
        public BeepCollection() : base()
        {
        }

        public string ToXMLString()
        {
            using (var sw = new StringWriter())
            using (var w = new XmlTextWriter(sw))
            {
                // TODO:
                // DefaultView.Sort = "MsBeforeRinging DESC";

                var d = new XmlDocument();
                XmlElement r = d.CreateElement("Beeps");

                foreach (Beep b in this)
                {
                    XmlElement el = d.CreateElement("Beep");
                    el.SetAttribute("MsBeforeRinging", b.MsBeforeRinging.ToString());
                    el.SetAttribute("BeepDuration", b.BeepDuration.ToString());
                    el.SetAttribute("BeepFrequency", b.BeepFrequency.ToString());

                    r.AppendChild(el);
                }

                d.AppendChild(r);
                d.WriteTo(w);
                return sw.ToString();
            }
        }

        internal BeepTimerCollection CreateBeepTimerCollection(TimerData td = null, bool startNow = false)
        {
            var btc = new BeepTimerCollection();

            btc.BeepTimers = new DispatcherTimer[Count];
            btc.BeepDurations = new int[Count];
            btc.BeepMsBeforeRinging = new int[Count];
            btc.BeepFrequecies = new int[Count];

            int i = 0;
            foreach (Beep r in this)
            {
                var t = new DispatcherTimer(DispatcherPriority.Send);
                t.Tick += btc.TickHandler;

                btc.BeepTimers[i] = t;
                btc.BeepMsBeforeRinging[i] = (int)r.MsBeforeRinging;
                btc.BeepDurations[i] = (int)r.BeepDuration;
                btc.BeepFrequecies[i] = (int)r.BeepFrequency;

                int interval = startNow ? 1 :
                                (int)td.CurrentTimeSpan.TotalMilliseconds -
                                btc.BeepMsBeforeRinging[i];

                if (interval > 0)
                {
                    t.Interval = TimeSpan.FromMilliseconds(interval);
                    t.Start();
                }

                ++i;
            }

            return btc;
        }

        internal void LoadFromString(string v)
        {
            Clear();

            var xml = new XmlDocument();
            xml.LoadXml(v);

            LoadFromXML(xml.SelectNodes("/Beeps/Beep"));
        }

        internal void LoadFromXML(XmlNodeList e)
        {
            foreach (XmlElement b in e)
            {
                Add(new Beep()
                {
                    MsBeforeRinging = int.Parse(b.GetAttribute("MsBeforeRinging")),
                    BeepFrequency = int.Parse(b.GetAttribute("BeepFrequency")),
                    BeepDuration = int.Parse(b.GetAttribute("BeepDuration"))
                });
            }
        }

        internal BeepCollection Clone()
        {
            var bc = new BeepCollection();
            foreach (Beep b in this)
            {
                bc.Add(b);
            }
            return bc;
        }
    }
}
