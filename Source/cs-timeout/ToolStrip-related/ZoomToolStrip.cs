using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace cs_timed_silver
{
    class ZoomToolStrip : ToolStrip
    {
        internal string Title
        {
            get
            {
                return tslTitle.Text;
            }
            set
            {
                tslTitle.Text = value;
            }
        }

        internal IClocksView TimersView = null;

        internal ToolStripLabel tslTitle;
        internal AutorepeatToolStripButton tsbZoomIn, tsbZoomOut;
        internal ToolStripButton tsbZoomReset;
        internal ToolStripTrackBarItem tstb;
        internal ToolStripComboBox tscb;

        internal event EventHandler ValuePropagationRequested,
            ZoomPercentChanged, SettingUpdateRequested;
        internal string PropertyName = "";

        internal MainForm _MyForm = null;
        internal MainForm MyForm
        {
            get
            {
                return _MyForm;
            }
            set
            {
                if (_MyForm != value)
                {
                    _MyForm = value;
                    OnMyFormChanged();
                }
            }
        }

        internal Program _Program = null;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Program Program
        {
            get
            {
                return _Program;
            }
            set
            {
                if (value != _Program)
                {
                    _Program = value;
                    OnProgramChanged();
                }
            }
        }

        private void OnProgramChanged()
        {
            tstb.Program = Program;
        }

        internal void OnMyFormChanged()
        {
            if (MyForm != null)
            {

            }
        }

        public ZoomToolStrip()
        {
            tslTitle = new ToolStripLabel();
            tslTitle.DisplayStyle = ToolStripItemDisplayStyle.Text;

            // TODO: [VISUAL] accelerator (here, "I") settable from MainForm, because I have 2
            // ZoomToolStrips. But maybe better would be middle mouse button + Ctrl scroll (also Ctrl++, Ctrl+-, Ctrl+= or Ctrl+0.
            // TODO: [VISUAL] Global Undo & Redo and local (in a single TimerControl) Undo & Redo & History panel

            tsbZoomIn = new AutorepeatToolStripButton("Zoom &In",
                Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_zoom_in, 128));
            tsbZoomOut = new AutorepeatToolStripButton("Zoom &Out",
                Utils.IconResourceVersionBySize(Properties.Resources.Oxygen_Icons_org_Oxygen_Actions_zoom_out, 128));

            tsbZoomReset = new ToolStripButton("&Reset",
                Utils.IconResourceVersionBySize(Properties.Resources.zoom_reset_icon_based_on_zoom_out_from_oxygen_icons, 128));
            tstb = new ToolStripTrackBarItem()
            {
                Minimum = 1,
                Maximum = 400,
                Value = 100,
                AutoSize = false,
                AutoToolTip = true
            };
            tscb = new ToolStripComboBox()
            {
                Available = true
            };
            tscb.Items.AddRange(new object[] { 25, 50, 75, 100, 125, 150, 175, 200, 300 });
            tscb.ComboBox.FormattingEnabled = true;
            tscb.ComboBox.Format += ComboBox_Format;
            tscb.AutoSize = false;
            tscb.Width = 65;
            tscb.SelectedIndex = 1;
            tscb.Enter += Tscb_Enter;
            tscb.KeyDown += Tscb_KeyDown;
            tscb.Leave += Tscb_Leave;
            tscb.TextChanged += Tscb_TextChanged;
            tscb.SelectedIndexChanged += Tscb_SelectedIndexChanged;

            Items.AddRange(new ToolStripItem[]
            {
                tslTitle, tsbZoomIn, tsbZoomOut, tsbZoomReset, tstb, tscb
            });

            string strZoom = "Move the rectangular button to the left to zoom out or to the right to zoom in. You can also use the three buttons in the left.";
            tstb.ToolTipText = strZoom;
            tstb.Available = true;

            tsbZoomReset.Click += TsbZoomReset_Click;

            tsbZoomIn.NormalClick += TsbZoomIn_NormalClick;
            tsbZoomIn.AutoClick += TsbZoomIn_AutoClick;

            tsbZoomOut.NormalClick += TsbZoomOut_NormalClick;
            tsbZoomOut.AutoClick += TsbZoomOut_AutoClick;

            tstb.Scroll += Tstb_Scroll;
            MouseWheel += ZoomToolStrip_MouseWheel;
        }

        private void Tscb_Enter(object sender, EventArgs e)
        {
            tscb.SelectAll();
        }

        private void ComboBox_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = Convert.ToInt32(e.ListItem).ToString() + "%";
        }

        private void Tscb_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnComboBoxEnter();
        }

        private void Tscb_Leave(object sender, EventArgs e)
        {
            OnComboBoxEnter();
        }

        private void Tscb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter ||
                e.KeyCode == Keys.Return)
            {
                OnComboBoxEnter();

                // to remove the beep:
                e.SuppressKeyPress = true;

                e.Handled = true;
            }
        }

        internal void OnComboBoxEnter()
        {
            return;
            // the combobox steals focus and the MainForm does not know the
            // previous focused control so I do this:
            if (PreviouslyFocusedControl != null)
            {
                MyForm.ApplyFocusedControlChange(PreviouslyFocusedControl);
                (MyForm.MyFocusedClocksViewProvider.FocusedClocksView as
                    System.Windows.Controls.Control).Focus();
            }

            if (tscb.SelectedItem != null)
            {
                tstb.SyncSetValue((int)tscb.SelectedItem);
                return;
            }

            int percent;
            try
            {
                try
                {
                    percent = int.Parse(tscb.Text);
                    tscb.Text = $"{percent}%";
                    tstb.SyncSetValue(percent);
                }
                catch (Exception /*ex*/)
                {
                    try
                    {
                        string p = tscb.Text.Substring(0, tscb.Text.Length - 1);
                        int per = int.Parse(p);
                        tscb.Text = $"{p}%";
                        tstb.SyncSetValue(per);
                    }
                    catch (Exception /*ex*/)
                    {
                        tscb.Text = "100%";
                    }
                }
            }
            catch (Exception /*ex*/)
            {
                tscb.Text = "100%";
            }

            // Only with these two calls the ComboBox remains focused:
            tscb.ComboBox.SelectAll();
            tscb.ComboBox.Select();
        }

        private void Tscb_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void TsbZoomOut_AutoClick(object sender, EventArgs e)
        {
            tstb.SyncSetValue(tstb.Value - 1);
        }

        private void TsbZoomOut_NormalClick(object sender, EventArgs e)
        {
            ChangeZoomOnButtonClick(-0.1M);
        }

        private void TsbZoomIn_AutoClick(object sender, EventArgs e)
        {
            tstb.SyncSetValue(tstb.Value + 1);
        }

        private void TsbZoomIn_NormalClick(object sender, EventArgs e)
        {
            ChangeZoomOnButtonClick(0.1M);
        }

        private void ZoomToolStrip_MouseWheel(object sender, MouseEventArgs e)
        {
            if (tstb.Hovered)
            {
                tstb.SyncSetValue(tstb.Value +
                    (e.Delta < 0 ? -5 : 5));
            }
        }

        private void Tstb_Scroll(object sender, EventArgs e)
        {
            //if (!ZoomManagedByButtonsOrImport)
            //{
                SuspendLayoutIfPossible();

                ZoomPercentChanged?.Invoke(this, EventArgs.Empty);
                ValuePropagationRequested?.Invoke(this, EventArgs.Empty);
                SettingUpdateRequested?.Invoke(this, EventArgs.Empty);

                tscb.Text = $"{Convert.ToInt32(tstb.Value)}%";

                ResumeLayoutIfPossible();
            //}
        }

        internal void SetZoomPercentAsSystemIfValid(decimal zoomPercent)
        {
            bool previous = tstb.ChangeByUser;
            tstb.ChangeByUser = false;

            SetZoomPercentIfValid(zoomPercent);

            tstb.ChangeByUser = previous;
        }

        // TODO: having the two tracepoints below, the problem is
        // that when Scroll-ing the tstb, the zoom is applied
        // to the control before the tracepoint of SuspendLayout is
        // printed to the Output window.
        // Understand, see the comment below with more lines of code,
        // eventually post on SO how to change the order of the
        // handlers of an event?
        // Eventually a new handler to the animation timer inside the
        // ToolStripTrackBarItem.

        internal void ResumeLayoutIfPossible()
        {
            if (TimersView != null)
            {
                Utils.ResumeLayoutRecursively(TimersView as Control, true);
            }
        }

        internal void SuspendLayoutIfPossible()
        {
            if (TimersView != null)
            {
                Utils.SuspendLayoutRecursively(TimersView as Control);
            }
        }

        internal static decimal RoundZoomFactor(decimal f)
        {
            return (decimal)Math.Round(f, 1);
        }

        internal bool ZoomManagedByButtonsOrImport = false;

        internal void ChangeZoomOnButtonClick(decimal v)
        {
            // this sentinel flag is not useful because inside the ToolStripTrackBarItem
            // a ChangeByUser flag is used
            //ZoomManagedByButtonsOrImport = true;

            decimal newFactor = RoundZoomFactor(tstb.Value / 100M) + v;
            if (ZoomPercentIsValid((int)(newFactor * 100)))
            {
                tstb.SyncSetValue((int)(newFactor * 100));
                //if (tstb.Value != (int)(newFactor * 100))
                //{
                //    EventHandler evt = delegate (object sender, EventArgs e)
                //    {
                //        ZoomManagedByButtonsOrImport = false;
                //    };
                //    tstb.Scroll += evt;
                //    tstb.Value = (int)(newFactor * 100);
                //    tstb.Scroll -= evt;
                //}

                SuspendLayoutIfPossible();

                ZoomPercentChanged?.Invoke(this, EventArgs.Empty);
                ValuePropagationRequested?.Invoke(this, EventArgs.Empty);
                SettingUpdateRequested?.Invoke(this, EventArgs.Empty);

                ResumeLayoutIfPossible();
            }

            //ZoomManagedByButtonsOrImport = false;
        }

        private void TsbZoomReset_Click(object sender, EventArgs e)
        {
            tstb.SyncSetValue(100);
        }

        internal bool ZoomPercentIsValid(decimal percent)
        {
            return percent <= tstb.Maximum &&
                percent >= tstb.Minimum;
        }

        internal void SetZoomPercentIfValid(decimal percent)
        {
            if (ZoomPercentIsValid(percent))
            {
                tstb.SyncSetValue((int)percent);
                tscb.Text = $"{(int)Math.Round(percent)}%";
            }
        }

        #region Remember previously focused control, and reset it on leave

        internal Control PreviouslyFocusedControl = null;

        protected override void OnEnter(EventArgs e)
        {
            PreviouslyFocusedControl = MyForm.toolStripContainer1.ActiveControl;

            base.OnEnter(e);
        }

        protected override void OnMouseDown(MouseEventArgs mea)
        {
            PreviouslyFocusedControl = MyForm.toolStripContainer1.ActiveControl;

            base.OnMouseDown(mea);
        }

        protected override void OnLeave(EventArgs e)
        {
            PreviouslyFocusedControl?.Select();

            base.OnLeave(e);
        }

        #endregion

        internal void SetItemsEnabled(bool enabled)
        {
            foreach (ToolStripItem tsi in Items)
            {
                tsi.Enabled = enabled;
            }
        }
    }
}
