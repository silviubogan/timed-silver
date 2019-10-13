using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace cs_timed_silver
{
    [ToolStripItemDesignerAvailability
      (ToolStripItemDesignerAvailability.ToolStrip |
       ToolStripItemDesignerAvailability.StatusStrip)]
    public partial class ToolStripTrackBarItem : ToolStripItem
    {
        public event EventHandler Scroll;

        internal Timer MyValueApplicationTimer;

        internal int _Minimum = 1;
        public int Minimum
        {
            get
            {
                return _Minimum;
            }
            set
            {
                if (_Minimum != value && value <= Maximum)
                {
                    _Minimum = value;
                    RecomputeButtonRectangle();
                }
            }
        }

        internal int _Maximum = 100;
        public int Maximum
        {
            get
            {
                return _Maximum;
            }
            set
            {
                if (_Maximum != value && value >= Minimum)
                {
                    _Maximum = value;
                    RecomputeButtonRectangle();
                }
            }
        }

        internal bool ChangeByUser { get; set; } = true;

        internal bool ValueTimerEnabled { get; set; } = true;

        internal int _Value = 100;
        public int Value
        {
            get
            {
                return _Value;
            }
            set
            {
                if (ValueTimerEnabled)
                {
                    DelayedSetValue(value);
                }
                else
                {
                    SyncSetValue(value);
                }
            }
        }

        internal void DelayedSetValue(int value)
        {
            if (_Value != value && value >= Minimum && value <= Maximum)
            {
                _Value = value;
                MyValueApplicationTimer.Stop();
                MyValueApplicationTimer.Start();
                RecomputeButtonRectangle();
                ShowPercentToolTip(value);
            }
        }

        internal Program Program { get; set; } = null;

        internal void ShowPercentToolTip(int percent)
        {
            // commented out because the current zoom percent is shown in the ComboBox
            //MyToolTip.Hide(Owner); // TODO: [VISUAL] w || w/o this line, while the ToolTip is visible, the user cannot
            //// click elsewhere (at least on the track bar) to change the zoom. This requires 2 clicks.
            //MyToolTip.Show(percent + "%", Owner,
            //    new Point(Bounds.Left + (int)ButtonRectangle.Left,
            //        Bounds.Top + (int)ButtonRectangle.Top - Font.Height),
            //    500);
        }

        internal void SyncSetValue(int value)
        {
            if (_Value != value && value >= Minimum && value <= Maximum)
            {
                _Value = value;
                RecomputeButtonRectangle();
                if (Owner != null)
                {
                    ShowPercentToolTip(value);
                }
                if (ChangeByUser)
                {
                    Scroll?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        internal ToolTip MyToolTip;

        public ToolStripTrackBarItem()
        {
            AutoSize = false;

            MyValueApplicationTimer = new Timer()
            {
                Interval = 100
            };
            MyValueApplicationTimer.Tick += MyValueApplicationTimer_Tick;

            MyToolTip = new ToolTip();
            
            //SyncSetValue(100);

            Width = 250;
            Height = 22;
        }

        private void MyValueApplicationTimer_Tick(object sender, EventArgs e)
        {
            if (ChangeByUser)
            {
                Scroll?.Invoke(this, EventArgs.Empty);
            }

            MyValueApplicationTimer.Stop();
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            proposedSize.Width = Math.Min(200, proposedSize.Width);
            proposedSize.Height = Math.Min(20, proposedSize.Height);
            return proposedSize;
        }

        internal RectangleF ButtonRectangle = RectangleF.Empty;

        internal bool ButtonHovered = false;

        internal bool Hovered = false;

        internal void RecomputeButtonRectangle()
        {
            // Without these comments the rectangle is a little too to the left/right.
            ButtonRectangle = new RectangleF(
                /*leftSpacing +*/ (float)(Width /*- leftSpacing */- rightSpacing) /
                    (float)(Maximum - Minimum) * (float)Value - rWidth / 2F,
                (Height - rHeight) / 2F,
                rWidth, rHeight);

            Invalidate();
        }

        internal const float leftSpacing = 2,
                rightSpacing = 3;
        internal const float rWidth = 6,
            rHeight = 12;

        internal float ResetToValue = 100F;

        internal Pen ResetToIndicatorPen = new Pen(Color.Blue, 2F);
        internal Pen borderPen;

        internal Pen activeZoomPen = new Pen(Color.Green, 2F);
        internal Pen inactiveZoomPen = new Pen(Color.Gray, 1F);

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            e.Graphics.DrawLine(activeZoomPen,
                leftSpacing, Height / 2F - activeZoomPen.Width / 2F,
               ButtonRectangle.Left, Height / 2F - activeZoomPen.Width / 2F
            );
            e.Graphics.DrawLine(inactiveZoomPen,
                ButtonRectangle.Right, Height / 2F - inactiveZoomPen.Width / 1F,
                Width - rightSpacing, Height / 2F - inactiveZoomPen.Width / 1F
            );

            float resetToX = ValueToX(ResetToValue);
            e.Graphics.DrawLine(ResetToIndicatorPen,
                resetToX - ResetToIndicatorPen.Width / 2, 0,
                resetToX - ResetToIndicatorPen.Width / 2, Height);
            
            e.Graphics.FillRectangle(Hovered ? Brushes.LightGray :
                Brushes.DarkGray, ButtonRectangle);

            if (Hovered)  // memoize (handle OnHoveredChanged)
            {
                borderPen = new Pen(Utils.MyDarkDarkGray); // memoize
            }
            else
            {
                borderPen = new Pen(Utils.MyDarkGray); // memoize
            }

            e.Graphics.DrawRectangle(borderPen,
                new Rectangle(0, 0, Width - 1, Height - 1)); // memoize Rectangle (recompute only on resize)
            e.Graphics.DrawRectangle(borderPen,
                ButtonRectangle.X,
                ButtonRectangle.Y,
                ButtonRectangle.Width,
                ButtonRectangle.Height); // memoize rectangle only if it is computed more than needed times
        }

        internal bool IsMouseDown = false;

        internal bool ScreenMouseMoveSubscribed = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            IsMouseDown = true;
            IsBeingDragged = false;

            if (Program != null)
            {
                if (!ScreenMouseMoveSubscribed)
                {
                    Program.ScreenMouseMove += Program_ScreenMouseMove;
                    Program.ScreenMouseUp += Program_ScreenMouseUp;
                    ScreenMouseMoveSubscribed = true;
                }
            }
        }

        private void Program_ScreenMouseUp(object sender, EventArgs e)
        {
            Program.ScreenMouseMove -= Program_ScreenMouseMove;
            Program.ScreenMouseUp -= Program_ScreenMouseUp;
            ScreenMouseMoveSubscribed = false;
        }

        private void Program_ScreenMouseMove(object sender, EventArgs e)
        {
            Point topLeftScreenLocation = Parent.PointToScreen(Bounds.Location);
            Point topRightScreenLocation = topLeftScreenLocation + new Size(Bounds.Width, 0);

            if (Cursor.Position.X < topLeftScreenLocation.X)
            {
                DelayedSetValue(Minimum);
            }
            else if (Cursor.Position.X > topRightScreenLocation.X)
            {
                DelayedSetValue(Maximum);
            }
            else
            {
                DelayedSetValue((int)ValueFromCursor());
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!IsBeingDragged)
            {
                // simple click done
                SyncSetValue((int)ValueFromCursor());
            }

            IsMouseDown = false;
            IsBeingDragged = false;
        }

        internal RectangleF btnRect;

        internal void UpdateHoveredAttribute()
        {
            btnRect = new RectangleF(Bounds.X + ButtonRectangle.X,
                Bounds.Y + ButtonRectangle.Y,
                ButtonRectangle.Width,
                ButtonRectangle.Height);
            if (btnRect.Contains(Owner.PointToClient(Cursor.Position)))
            {
                ButtonHovered = true;
            }
            else
            {
                ButtonHovered = false;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            Hovered = true;
            IsBeingDragged = false;

            UpdateHoveredAttribute();

            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            IsMouseDown = false;

            Hovered = false;
            IsBeingDragged = false;

            UpdateHoveredAttribute();

            Invalidate();
        }

        internal bool IsBeingDragged = false;

        internal float ValueFromCursor()
        {
            // interest: Left of the COntrol + Left of the cursor position - rectangle width / 2, both coordinates (left) relative to the owner ToolStrip.

            // interest: Left of the COntrol
            float leftOfControl = Bounds.Left;

            float leftOfCursor = Owner.PointToClient(Cursor.Position).X;

            return ((float)(leftOfCursor - leftOfControl) *
                (float)(Maximum - Minimum) / (float)Width);
        }

        internal float ValueToX(float val)
        {
            return (Width * val) / (Maximum - Minimum);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Hovered = true;

            if (IsBeingDragged || (IsMouseDown && Hovered))
            {
                IsBeingDragged = true;
                
                if (!ScreenMouseMoveSubscribed)
                {
                    DelayedSetValue((int)ValueFromCursor());
                }
            }
            else
            {
                // Incomplete or useless:
                //ButtonHovered = false;
            }

            UpdateHoveredAttribute();
        }

        //protected override void OnMouseWheel(MouseEventArgs e)
        //{
        //    base.OnMouseWheel(e);

        //    if (e.Delta > 0)
        //    {
        //        Value++;
        //    }
        //    else
        //    {
        //        Value--;
        //    }
        //    Scroll?.Invoke(this, e);
        //}
    }
}
