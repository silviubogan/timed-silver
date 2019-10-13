using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace TimedSilverTests
{
    [TestClass]
    public class UtilsUnitTest
    {
        [TestMethod]
        public void UtilsTest_CreateCursorFromControl()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();
            
            // Act
            Application.DoEvents();
            var bmp = new Bitmap(1, 1);
            Cursor cur = Utils.CreateCursorFromControl(mf, 10, ref bmp);

            // Assert
            Assert.IsTrue(
                Utils.CursorsAreEqual(
                    new Cursor(bmp.GetHicon()),
                    cur
                )
            );
        }
        
        [TestMethod]
        public void UtilsTest_RemoveAllSelectedRows()
        {
            // Arrange
            var bdgv = new DataGridView();

            var c1 = new DataGridViewTextBoxColumn()
            {
                Name = "MsBeforeRinging",
                DataPropertyName = "MsBeforeRinging",
                HeaderText = "Ms Before Ringing",
                SortMode = DataGridViewColumnSortMode.Programmatic
            };
            c1.ToolTipText = "Milliseconds before the clock starts ringing when the beep will start to sound.";

            var c2 = new DataGridViewTextBoxColumn()
            {
                Name = "BeepDuration",
                DataPropertyName = "BeepDuration",
                HeaderText = "Beep Duration Is Ms",
                SortMode = DataGridViewColumnSortMode.Programmatic
            };
            c2.ToolTipText = "The duration of the beep measured in milliseconds.";

            var c3 = new DataGridViewTextBoxColumn()
            {
                Name = "BeepFrequency",
                DataPropertyName = "BeepFrequency",
                HeaderText = "Beep Frequency",
                SortMode = DataGridViewColumnSortMode.Programmatic
            };
            c3.ToolTipText = "The frequency of the beep, ranging from 37 to 32767 hertz.";

            bdgv.Columns.Add(c1);
            bdgv.Columns.Add(c2);
            bdgv.Columns.Add(c3);

            bdgv.EditMode = DataGridViewEditMode.EditOnEnter;
            bdgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            bdgv.Rows.Add(100, 100, 100);
            bdgv.Rows.Add(200, 200, 200);
            bdgv.SelectAll();

            // Act
            Utils.RemoveAllSelectedRows(bdgv);

            // Assert
            // there always is the new-row row
            Assert.AreEqual(1, bdgv.RowCount);
        }
        
        [TestMethod]
        public void UtilsTest_RemoveAllSelectedRows_2()
        {
            // Arrange
            var f = new Form();
            f.Show();

            Application.DoEvents();

            var bdgv = new DataGridView();

            var c1 = new DataGridViewTextBoxColumn()
            {
                Name = "MsBeforeRinging",
                DataPropertyName = "MsBeforeRinging",
                HeaderText = "Ms Before Ringing",
                SortMode = DataGridViewColumnSortMode.Programmatic
            };
            c1.ToolTipText = "Milliseconds before the clock starts ringing when the beep will start to sound.";

            var c2 = new DataGridViewTextBoxColumn()
            {
                Name = "BeepDuration",
                DataPropertyName = "BeepDuration",
                HeaderText = "Beep Duration Is Ms",
                SortMode = DataGridViewColumnSortMode.Programmatic
            };
            c2.ToolTipText = "The duration of the beep measured in milliseconds.";

            var c3 = new DataGridViewTextBoxColumn()
            {
                Name = "BeepFrequency",
                DataPropertyName = "BeepFrequency",
                HeaderText = "Beep Frequency",
                SortMode = DataGridViewColumnSortMode.Programmatic
            };
            c3.ToolTipText = "The frequency of the beep, ranging from 37 to 32767 hertz.";

            bdgv.Columns.Add(c1);
            bdgv.Columns.Add(c2);
            bdgv.Columns.Add(c3);

            bdgv.EditMode = DataGridViewEditMode.EditOnEnter;
            bdgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            bdgv.Rows.Add(new object[] {
                100, 100, 100
            });
            bdgv.Rows.Add(new object[] {
                200, 200, 200
            });

            f.Controls.Add(bdgv);

            // Act
            foreach (DataGridViewRow r in bdgv.Rows)
            {
                // skip new-row row & '100' row
                if (r.Cells[0].Value == null ||
                    (r.Cells[0].Value != null &&
                    (int)r.Cells[0].Value == 100))
                {
                    continue;
                }
                r.Selected = true;
            }

            Utils.RemoveAllSelectedRows(bdgv);

            // Assert
            Assert.AreEqual(1, bdgv.RowCount);
        }
    }
}