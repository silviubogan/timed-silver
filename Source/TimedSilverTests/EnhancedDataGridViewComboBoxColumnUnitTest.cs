using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TimedSilverTests
{
    [TestClass]
    public class EnhancedDataGridViewComboBoxColumnUnitTest
    {
        [TestMethod]
        public void EnhancedDataGridViewComboBoxColumnTest_DataSourceContainsDisplayString()
        {
            // Arrange
            var dgv = new DataGridView();

            var col = new EnhancedDataGridViewComboBoxColumn();
            dgv.Columns.Add(col);

            var source = new List<ListControlItem>();
            source.Add(new ListControlItem()
            {
                Tag = "test tag",
                DisplayString = "test display string"
            });
            col.DataSource = source;

            // Assert
            Assert.IsTrue(col.DataSourceContainsDisplayString("test display string"));
        }

        [TestMethod]
        public void EnhancedDataGridViewComboBoxColumnTest_IsNewEntryValid()
        {
            // Arrange
            var dgv = new DataGridView();

            var col = new EnhancedDataGridViewComboBoxColumn();
            dgv.Columns.Add(col);

            var source = new List<ListControlItem>();
            source.Add(new ListControlItem()
            {
                Tag = "test tag",
                DisplayString = "test display string"
            });
            col.DataSource = source;

            // Assert
            Assert.IsFalse(col.IsNewEntryValid("test display string"), "duplicate");

            col.AllowDuplicates = true;

            Assert.IsTrue(col.IsNewEntryValid("test display string"), "allowing duplicates");

            Assert.IsFalse(col.IsNewEntryValid(""), "empty string");
            Assert.IsTrue(col.IsNewEntryValid("test 2"), "does not already exist");
        }

        [TestMethod]
        public void EnhancedDataGridViewComboBoxColumnTest_AddNewEntry()
        {
            // Arrange
            var dgv = new DataGridView();

            var col = new EnhancedDataGridViewComboBoxColumn();
            dgv.Columns.Add(col);

            var source = new List<ListControlItem>();
            source.Add(new ListControlItem()
            {
                Tag = "test tag",
                DisplayString = "test display string"
            });
            col.DataSource = source;

            ListControlItem newItem = null;
            col.NewItemAdded += delegate (object sender, ListControlItemEventArgs e)
            {
                newItem = e.Item;
            };
            ListControlItem returnedItem = col.AddNewEntry("my little test");

            // Assert
            Assert.AreEqual(newItem, returnedItem);
            Assert.IsNotNull(newItem);
        }
    }
}
