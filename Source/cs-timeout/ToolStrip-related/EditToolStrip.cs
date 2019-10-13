using System;
using System.Windows.Forms;

namespace cs_timed_silver
{
    class EditToolStrip : ToolStrip
    {
        internal ToolStripLabel toolStripLabel;

        internal ToolStripSplitButton tsbNewTimer,
            tsbNewAlarm;
        internal ToolStripButton tsbSettings;

        internal MainForm MyMainForm;

        internal EditToolStrip(MainForm mf)
        {
            toolStripLabel = new ToolStripLabel();
            tsbNewTimer = new ToolStripSplitButton();
            tsbNewAlarm = new ToolStripSplitButton();
            tsbSettings = new ToolStripButton();

            ToolStripItem nt1 = tsbNewTimer.DropDownItems.Add("1");
            ToolStripItem nt5 = tsbNewTimer.DropDownItems.Add("5");
            ToolStripItem nt10 = tsbNewTimer.DropDownItems.Add("10");

            nt1.Click += Nt1_Click;
            nt5.Click += Nt5_Click;
            nt10.Click += Nt10_Click;

            ToolStripItem na1 = tsbNewAlarm.DropDownItems.Add("1");
            ToolStripItem na5 = tsbNewAlarm.DropDownItems.Add("5");
            ToolStripItem na10 = tsbNewAlarm.DropDownItems.Add("10");

            na1.Click += Na1_Click;
            na5.Click += Na5_Click;
            na10.Click += Na10_Click;

            // tsEdit
            Items.AddRange(new ToolStripItem[]
            {
                toolStripLabel,
                tsbNewTimer,
                tsbNewAlarm,
                tsbSettings
            });
            
            // toolStripLabel
            toolStripLabel.Text = "Edit";

            // tsbNewTimer
            tsbNewTimer.Image = Utils.IconResourceVersionBySize(
                Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_list_add, 128);
            tsbNewTimer.Text = "&New timer";
            tsbNewTimer.DefaultItem = nt1;

            // tsbNewAlarm
            tsbNewAlarm.Image = Utils.IconResourceVersionBySize(
                Properties.Resources.Apps_clock_icon, 128);
            tsbNewAlarm.Text = "New &alarm";
            tsbNewAlarm.DefaultItem = na1;

            // TODO: Update 'above' to 'below' and reverse,
            // when moving the tool strip in the UI.
            tsbNewTimer.ToolTipText = "Click here to add a" +
                " new timer at the end of the list above.";

            tsbSettings.Image = Utils.IconResourceVersionBySize(
                Properties.Resources.Oxygen_Icons_org_Oxygen_Categories_preferences_system, 128);
            tsbSettings.Text = "S&ettings";
            tsbSettings.Click += TsbSettings_Click;
            tsbSettings.ToolTipText = "Click this button" +
                " to open the settings dialog where you" +
                " can control the behavior of the program.";

            mf.toolStripContainer1.TopToolStripPanel.Controls.Add(this);
            this.SetBounds(0, 100, 100, 100); // just making sure that it is shown at the bottom of the top tool strip panel
            NewTimerRequested += mf.TsEdit_NewTimerRequested;
            SettingsRequested += mf.TsEdit_SettingsRequested;
            NewAlarmRequested += mf.TsEdit_NewAlarmRequested;

            MyMainForm = mf;
        }

        private void Na10_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; ++i)
            {
                NewAlarmRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Na5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; ++i)
            {
                NewAlarmRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Na1_Click(object sender, EventArgs e)
        {
            NewAlarmRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Nt10_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; ++i)
            {
                NewTimerRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Nt5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; ++i)
            {
                NewTimerRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Nt1_Click(object sender, EventArgs e)
        {
            NewTimerRequested?.Invoke(this, EventArgs.Empty);
        }

        private void TsbSettings_Click(object sender, EventArgs e)
        {
            SettingsRequested?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler NewTimerRequested,
            NewAlarmRequested,
            SettingsRequested;
    }
}
