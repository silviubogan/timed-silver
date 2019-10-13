using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Windows.Forms;

namespace TimedSilverTests
{
    [TestClass]
    public class BeepDataGridViewTest
    {
        /// <summary>
        /// The code below does not test BeepDataGridView class.
        /// TODO: Use the HandleCreated event.
        /// </summary>
        //[TestMethod]
        //public void BeepDataGridViewTest_ctor()
        //{
        //    // Arrange
        //    var bdgv = new DataGridView();

        //    // Act
        //    var c1 = new DataGridViewTextBoxColumn()
        //    {
        //        Name = "MsBeforeRinging",
        //        DataPropertyName = "MsBeforeRinging",
        //        HeaderText = "Ms Before Ringing",
        //        SortMode = DataGridViewColumnSortMode.Programmatic
        //    };
        //    c1.ToolTipText = "Milliseconds before the clock starts ringing when the beep will start to sound.";

        //    var c2 = new DataGridViewTextBoxColumn()
        //    {
        //        Name = "BeepDuration",
        //        DataPropertyName = "BeepDuration",
        //        HeaderText = "Beep Duration Is Ms",
        //        SortMode = DataGridViewColumnSortMode.Programmatic
        //    };
        //    c2.ToolTipText = "The duration of the beep measured in milliseconds.";

        //    var c3 = new DataGridViewTextBoxColumn()
        //    {
        //        Name = "BeepFrequency",
        //        DataPropertyName = "BeepFrequency",
        //        HeaderText = "Beep Frequency",
        //        SortMode = DataGridViewColumnSortMode.Programmatic
        //    };
        //    c3.ToolTipText = "The frequency of the beep, ranging from 37 to 32767 hertz.";

        //    bdgv.Columns.Add(c1);
        //    bdgv.Columns.Add(c2);
        //    bdgv.Columns.Add(c3);

        //    bdgv.EditMode = DataGridViewEditMode.EditOnEnter;
        //    bdgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        //    bdgv.Rows.Add(100, 100, 100);
        //    bdgv.Rows.Add(200, 200, 200);
        //    bdgv.Rows.Add(300, 300, 300);

        //    // Assert
        //    Assert.AreEqual(4, bdgv.RowCount); // including the new-row row
        //    Assert.AreEqual(3, bdgv.ColumnCount);
        //}
    }
}
