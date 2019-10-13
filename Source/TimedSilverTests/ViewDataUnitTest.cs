using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows.Forms;

namespace TimedSilverTests
{
    // TODO: remake "advanced views" using XAML, Grid & GridSplitter. Most of the below tested methods
    // are replaced by XAML and its Visual/LogicalTreeHelper.
    //[TestClass]
    public class ViewDataUnitTest
    {
        /// <summary>
        /// ViewData instances require an existing MainForm.
        /// </summary>
        [TestMethod]
        public void ViewDataTest_ViewType_change()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();

            Application.DoEvents();

            var vd = new ViewData(mf.MyDataFile);
            bool correct = false;
            vd.ViewTypeChanged += delegate (object sender, EventArgs e)
            {
                correct = vd.ViewType == ViewDataType.DataGrid;
            };
            vd.ViewType = ViewDataType.DataGrid;

            // Act
            //Application.DoEvents();

            // Assert
            Assert.IsTrue(correct, "ViewTypeChanged event called when ViewType is not the new value.");
        }

        [TestMethod]
        public void ViewDataTest_ViewType_default_value()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();

            Application.DoEvents();

            var vd = new ViewData(mf.MyDataFile);

            // Act
            //Application.DoEvents();

            // Assert
            Assert.AreEqual(ViewDataType.List, vd.ViewType, "ViewType default value is wrong.");
        }

        [TestMethod]
        public void ViewDataTest_ZoomPercentChanged()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();

            Application.DoEvents();

            var vd = new ViewData(mf.MyDataFile);
            bool correct = false;
            vd.ZoomPercentChanged += delegate (object sender, EventArgs e)
            {
                correct = vd.ZoomPercent == 120;
            };
            vd.ZoomPercent = 120;

            // Act
            //Application.DoEvents();

            // Assert
            Assert.IsTrue(correct, "ZoomPercentChanged event called when ZoomPercent is not the new value.");
        }

        [TestMethod]
        public void ViewDataTest_ZoomPercent_default_value()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();

            Application.DoEvents();

            var vd = new ViewData(mf.MyDataFile);

            // Act
            //Application.DoEvents();

            // Assert
            Assert.AreEqual(100, vd.ZoomPercent, "ZoomPercent default value is wrong.");
        }

        [TestMethod]
        public void ViewDataTest_ParentViewChanged()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();

            Application.DoEvents();

            var futureParent = new ViewData(mf.MyDataFile);
            var vd = new ViewData(mf.MyDataFile);
            bool correct = false;
            vd.ParentViewChanged += delegate (object sender, EventArgs e)
            {
                correct = vd.ParentViewData == futureParent;
            };
            vd.ParentViewData = futureParent;

            // Act
            //Application.DoEvents();

            // Assert
            Assert.IsTrue(correct, "ParentViewChanged called with the new parent not set.");
        }

        [TestMethod]
        public void ViewDataTest_PreviousParentViewData_default_value_etc()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();

            Application.DoEvents();

            var vd = new ViewData(mf.MyDataFile);

            // Act
            //Application.DoEvents();

            // Assert
            Assert.IsNull(vd.PreviousParentViewData, "PreviousParentViewData default value should be null.");
            Assert.IsNull(vd.ParentViewData, "ParentViewData default value should be null.");
            Assert.AreEqual(Orientation.Horizontal, vd.Orientation, "Orientation default value should be Horizontal.");
        }

        [TestMethod]
        public void ViewDataTest_SetViewPointer_and_View()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            var p = new Pointer();

            var vd = new ViewData(mf.MyDataFile);
            vd.SetViewPointer(p);
            SplitterView.BuildFromViewData(mf, vd);

            vd.View = new ClockListView(mf);

            // Act
            Application.DoEvents();

            // Assert
            Assert.AreEqual(p.Value, vd.View, "Pointer value should be a reference to the ViewData's view.");
        }

        [TestMethod]
        public void ViewDataTest_OrientationChanged()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            var vd = new ViewData(mf.MyDataFile);
            bool correct = false;
            vd.OrientationChanged += delegate (object sender, EventArgs e)
            {
                correct = vd.Orientation == Orientation.Vertical;
            };
            vd.Orientation = Orientation.Vertical;

            // Act
            //Application.DoEvents();

            // Assert
            Assert.IsTrue(correct, "OrientationChanged called with the new orientation not set.");
        }

        [TestMethod]
        public void ViewDataTest_ViewChanged()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            var vd = new ViewData(mf.MyDataFile);
            var newView = new ClockListView(mf);

            Application.DoEvents();

            bool correct = false;
            vd.ViewChanged += delegate (object sender, EventArgs e)
            {
                correct = vd.View == newView;
            };
            vd.View = newView;

            // Act
            Application.DoEvents();

            // Assert
            Assert.IsTrue(correct, "ViewChanged called with the new view not set.");
        }

        [TestMethod]
        public void ViewDataTest_GetSetHasFirstChild_and_Child1Changed()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();
            //var mf = new MainForm();
            //mf.Show();

            var vd = new ViewData(mf.MyDataFile);
            var vd2 = new ViewData(mf.MyDataFile);
            bool correct = false;
            vd.Child1Changed += delegate (object sender, EventArgs e)
            {
                correct = vd.GetFirstChild() == vd2;
            };
            vd.SetFirstChild(vd2);

            //var newView = new ClockListView(mf);

            // Act
            //Application.DoEvents();

            // Assert
            Assert.IsTrue(correct, "GetFirstChild and SetFirstChild do not work well together.");
            Assert.IsTrue(vd.HasFirstChild(), "HasFirstChild should return true.");
            Assert.IsFalse(vd.HasSecondChild(), "HasFirstChild should return false.");
        }

        [TestMethod]
        public void ViewDataTest_GetSecondChild()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            var vd = new ViewData(mf.MyDataFile);

            //var newView = new ClockListView(mf);

            // Act
            //Application.DoEvents();

            // Assert
            Assert.IsNull(vd.GetFirstChild(), "GetFirstChild should return null.");
            Assert.IsNull(vd.GetSecondChild(), "GetSecondChild should return null.");
        }

        [TestMethod]
        public void ViewDataTest_GetSetHasSecondChild_and_Child2Changed()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            var vd = new ViewData(mf.MyDataFile);
            var vd2 = new ViewData(mf.MyDataFile);
            bool correct = false;
            vd.Child2Changed += delegate (object sender, EventArgs e)
            {
                correct = vd.GetSecondChild() == vd2;
            };
            vd.SetSecondChild(vd2);

            //var newView = new ClockListView(mf);

            // Act
            //Application.DoEvents();

            // Assert
            Assert.IsTrue(correct, "GetSecondChild and SetSecondChild do not work well together.");
            Assert.IsFalse(vd.HasFirstChild(), "HasFirstChild should return false.");
            Assert.IsTrue(vd.HasSecondChild(), "HasSecondChild should return true.");
        }

        [TestMethod]
        public void ViewDataTest_SwitchChildren()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            var vd = new ViewData(mf.MyDataFile);
            var vd2 = new ViewData(mf.MyDataFile);
            bool correct = true;
            vd.ChildrenSwitched += delegate (object sender, EventArgs e)
            {
                correct = false;
            };
            vd.SetSecondChild(vd2);
            vd.SwitchChildren();

            vd.SetSecondChild(vd2);
            vd.SwitchChildren();

            //var newView = new ClockListView(mf);

            // Act
            //Application.DoEvents();

            // Assert
            Assert.AreEqual(vd2, vd.GetFirstChild(), "Deparenting first child, reparenting it as second child results in wrong first child.");
            Assert.AreEqual(null, vd.GetSecondChild(), "Deparenting first child, reparenting it as second child results in wrong second child which should be null.");
            Assert.IsTrue(correct, "ChildrenSwitched triggered mistakenly.");
        }

        [TestMethod]
        public void ViewDataTest_SetViewPointer()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            var p = new Pointer();
            p.Value = new ClockListView(mf);

            var vd = new ViewData(mf.MyDataFile);
            vd.SetViewPointer(p);
            SplitterView.BuildFromViewData(mf, vd);

            // Act
            Application.DoEvents();

            // Assert
            Assert.AreEqual(vd.View, p.Value, "Pointer passed to ViewData c-tor should set its View to the pointer's current value.");
        }

        [TestMethod]
        public void ViewDataTest_SetViewPointer_2()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            var p = new Pointer();
            p.Value = new ClockListView(mf);

            var vd = new ViewData(mf.MyDataFile);
            //SplitterView.BuildFromViewData(mf, vd);
            vd.SetViewPointer(p);

            // Act
            Application.DoEvents();
            p.Value = new ClockListView(mf);
            Application.DoEvents();

            // Assert
            Assert.AreEqual(vd.View, p.Value, "Pointer passed to ViewData c-tor should set its View to the pointer's current value when the value of the pointer changes.");
        }

        [TestMethod]
        public void ViewDataTest_SetViewPointer_3()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            var p = new Pointer();
            p.Value = new ClockListView(mf);

            var vd = new ViewData(mf.MyDataFile);
            vd.SetViewPointer(p);

            // Act
            Application.DoEvents();
            p.Value = null;
            Application.DoEvents();

            // Assert
            Assert.IsNotNull(vd.View, "When the pointer associated with the ViewData has a new null value, it should be ignored by the ViewData so the View is the preceding view.");
        }

        [TestMethod]
        public void ViewDataTest_MergeFrom()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();
            



            var vdParent = new ViewData(mf.MyDataFile)
            {
                ViewType = ViewDataType.Splitter
            };
            ISplitterPanel view = new ClockListView(mf);

            var vd = new ViewData(mf.MyDataFile)
            {
                Orientation = Orientation.Vertical,
                ParentViewData = null,
                View = null,
                ViewType = ViewDataType.DataGrid,
                ZoomPercent = 150
            };
            var vd2 = new ViewData(mf.MyDataFile)
            {
                Orientation = Orientation.Horizontal,
                ParentViewData = vdParent,
                View = view,
                ViewType = ViewDataType.List,
                ZoomPercent = 170
            };
            vd.MergeFrom(vd2);

            // Act
            Application.DoEvents();

            // Assert
            Assert.AreEqual(Orientation.Horizontal, vd.Orientation);
            Assert.AreEqual(vdParent, vd.ParentViewData);
            Assert.AreEqual(view, vd.View);
            Assert.AreEqual(ViewDataType.List, vd.ViewType);
            Assert.AreEqual(170, vd.ZoomPercent);
            Assert.IsNull(vd2.View, "View passed with MergeFrom method but remains in the original ViewData.");
        }

        [TestMethod]
        public void ViewDataTest_RecreateView_CloneWithoutChildren()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            var vdParent = new ViewData(mf.MyDataFile)
            {
                ViewType = ViewDataType.Splitter
            };

            var vd = new ViewData(mf.MyDataFile)
            {
                Orientation = Orientation.Horizontal,
                ParentViewData = vdParent,
                ViewType = ViewDataType.List,
                ZoomPercent = 170
            };
            vd.RecreateView(mf);

            ViewData vd2 = vd.CloneWithoutChildren();
            vd2.RecreateView(mf);

            // Act
            Application.DoEvents();

            // Assert
            Assert.AreEqual(Orientation.Horizontal, vd2.Orientation);
            Assert.AreEqual(vdParent, vd2.ParentViewData);
            Assert.AreEqual(vd.View.GetType(), vd2.View.GetType());
            Assert.AreEqual(ViewDataType.List, vd2.ViewType);
            Assert.AreEqual(170, vd2.ZoomPercent);
            Assert.AreNotSame(vd.View, vd2.View, "View passed with CloneWithoutChildren method different than the new view created at that moment.");
        }

        [TestMethod]
        public void ViewDataTest_MainForm_Load_InitializationCode()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();


            MainForm_Load_InitializationCode(mf);

            MainForm_ForceUpdateViewType_List(mf, false);

            MainForm_GroupListView_Checked_ListOrDataGrid(mf);

            MainForm_ForceUpdateViewType_Split(mf, Orientation.Vertical, true);

            mf.UpdateRootSplitterViewControl();

            // Act
            Application.DoEvents();

            // Assert
            Assert.IsInstanceOfType(mf.RootViewData.GetFirstChild().View, typeof(TimerGroupListView));
            Assert.IsInstanceOfType(mf.RootViewData.GetSecondChild().GetFirstChild().View, typeof(ClockListView));
            Assert.IsInstanceOfType(mf.RootViewData.GetSecondChild().GetSecondChild().View, typeof(ClockDataGridView));
        }

        [TestMethod]
        public void ViewDataTest_MainForm_ForceUpdateViewType_List()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;
            mf.Show();
            
            Application.DoEvents();

            MainForm_Load_InitializationCode(mf);

            MainForm_ForceUpdateViewType_List(mf, false);

            mf.UpdateRootSplitterViewControl();

            mf.MyClocksViewProvider.ShowGroupList_ListOrDataGrid();

            mf.RootSplitterView.Refresh();

            mf.UpdateRootSplitterViewControl();

            // Act
            Application.DoEvents();

            // Assert
            Assert.IsInstanceOfType(mf.RootSplitterView.c1,
                typeof(TimerGroupListView));
            Assert.IsInstanceOfType(mf.RootSplitterView.c2,
                typeof(ClockListView));
        }

        [TestMethod]
        public void ViewDataTest_RootSplitterView_c1_c2()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            MainForm_Load_InitializationCode(mf);

            mf.MyClocksViewProvider.ForceUpdateViewType_List(false);

            // Act
            Application.DoEvents();


            // Assert
            Assert.IsInstanceOfType(mf.RootSplitterView.c1,
                typeof(ClockListView));
            Assert.IsNull(mf.RootSplitterView.c2);
        }

        [TestMethod]
        public void ViewDataTest_ParentViewData()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();


            MainForm_Load_InitializationCode(mf);

            MainForm_ForceUpdateViewType_DataGrid(mf, false);


            // Act
            Application.DoEvents();


            // Assert
            Assert.IsInstanceOfType(mf.RootSplitterView.c1,
                typeof(ClockDataGridView));
            Assert.IsNull(mf.RootSplitterView.c2);
            Assert.AreEqual(mf.RootViewData,
                mf.RootViewData.GetFirstChild().ParentViewData);
        }

        /// <summary>
        /// Load the form, show the form,
        /// switch from list to datagrid,
        /// check to show group list,
        /// then switch back to list,
        /// w/o hiding grouplist.
        /// </summary>
        [TestMethod]
        public void ViewDataTest_MainForm_ForceUpdateViewType_DataGrid()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();


            MainForm_Load_InitializationCode(mf);

            MainForm_ForceUpdateViewType_DataGrid(mf, false);

            mf.UpdateRootSplitterViewControl();

            mf.MyClocksViewProvider.ShowGroupList_ListOrDataGrid();

            MainForm_ForceUpdateViewType_List(mf, true);

            // Act
            Application.DoEvents();


            // Assert
            Assert.IsInstanceOfType(mf.RootSplitterView.c1,
                typeof(TimerGroupListView));
            Assert.IsInstanceOfType(mf.RootSplitterView.c2,
                typeof(ClockListView));
            Assert.AreEqual(1, mf.RootSplitterView.Panel1.Controls.Count);
            Assert.AreEqual(1, mf.RootSplitterView.Panel2.Controls.Count);
            Assert.AreEqual(mf.RootViewData,
                mf.RootViewData.GetFirstChild().ParentViewData);
        }

        public void MainForm_Load_InitializationCode(MainForm mf)
        {
            mf.MyViewDataFactory.RootViewData = new ViewData(mf.MyDataFile)
            {
                ViewType = ViewDataType.Splitter
            };
            mf.RootViewData.SetViewPointer(mf.RootSplitterViewPointer);
            SplitterView.BuildFromViewData(mf, mf.RootViewData);
            (mf.RootViewData.View as Control).Name = "RootSplitterView";
            ClockListView clv = mf.MyClocksViewProvider.GetExistingOrNewClockListView();

            mf.RootViewData.SetFirstChild(new ViewData(mf.MyDataFile)
            {
                ViewType = ViewDataType.List,
                View = clv
            });
            clv.MyViewData = mf.RootViewData.GetFirstChild();

            mf.InitializeLastFocusedTimersView();

            //RootSplitterViewPointer.Value = SplitterView.BuildFromViewData(this, RootViewData) as SplitterView;

            //SplitterView.SubscribeViewData(RootViewData, RootViewData.View);

            //mf.UpdateRootSplitterViewControl();
        }

        public void MainForm_ForceUpdateViewType_List(MainForm mf, bool withTglv)
        {
            mf.MyClocksViewProvider.ForceUpdateViewType_List(withTglv);

            Assert.IsNotNull(mf.LastFocusedTimersView.MyViewData);

            Assert.IsTrue(mf.RootViewData.Contains(mf.LastFocusedTimersView.MyViewData));

            mf.UpdateRootSplitterViewControl();
        }

        /// <summary>
        /// Load the form, show the form.
        /// The second panel of the root SplitterView
        /// should be collapsed.
        /// </summary>
        [TestMethod]
        public void ViewDataTest_MainForm_ForceUpdateViewType_DataGrid_2()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            MainForm_Load_InitializationCode(mf);

            MainForm_ForceUpdateViewType_DataGrid(mf, false);

            // Act
            Application.DoEvents();


            // Assert
            Assert.IsInstanceOfType(mf.RootSplitterView.c1,
                typeof(ClockDataGridView));
            Assert.IsNull(mf.RootSplitterView.c2);

            Assert.AreEqual(1, mf.RootSplitterView.Panel1.Controls.Count);
            Assert.AreEqual(0, mf.RootSplitterView.Panel2.Controls.Count);

            Assert.IsFalse(mf.RootSplitterView.Panel1Collapsed);
            Assert.IsTrue(mf.RootSplitterView.Panel2Collapsed);
            Assert.AreEqual(mf.RootViewData,
                mf.RootViewData.GetFirstChild().ParentViewData);
        }

        private void MainForm_ForceUpdateViewType_DataGrid(MainForm mf, bool withTglv)
        {
            mf.MyClocksViewProvider.ForceUpdateViewType_DataGrid(withTglv);

            mf.UpdateRootSplitterViewControl();
        }

        private void MainForm_ForceUpdateViewType_Split(MainForm mf, Orientation o, bool withTglv)
        {
            mf.MyClocksViewProvider.ForceUpdateViewType_Split(o, withTglv);

            mf.UpdateRootSplitterViewControl();
        }

        [TestMethod]
        public void ViewDataTest_MainForm_UpdateRootSplitterViewControl()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            //Application.DoEvents();

            MainForm_Load_InitializationCode(mf);

            MainForm_ForceUpdateViewType_List(mf, false);

            MainForm_ForceUpdateViewType_DataGrid(mf, false);

            mf.UpdateRootSplitterViewControl(); // TODO: the place where this method is currently called is very well suited, the bug is below where I do not check some variables.

            // Act
            Application.DoEvents();


            // Assert
            Assert.IsInstanceOfType(mf.RootSplitterView.c1,
                typeof(ClockDataGridView));
            Assert.IsNull(mf.RootSplitterView.c2);

            Assert.AreEqual(1, mf.RootSplitterView.Panel1.Controls.Count);
            Assert.AreEqual(0, mf.RootSplitterView.Panel2.Controls.Count);

            Assert.IsFalse(mf.RootSplitterView.Panel1Collapsed);
            Assert.IsTrue(mf.RootSplitterView.Panel2Collapsed);

            Assert.AreEqual(mf.RootViewData,
                mf.RootViewData.GetFirstChild().ParentViewData);

            Assert.AreEqual(mf.tableLayoutPanel1, mf.RootSplitterView.Parent);

            Assert.AreEqual(3, mf.tableLayoutPanel1.Controls.Count);
        }

        /// <summary>
        /// show the default List view,
        /// show the GroupList view alongside,
        /// switch to the horizontally split view,
        /// then the GroupList is wrongly hidden.
        /// </summary>
        [TestMethod]
        public void ViewDataTest_show_hide_views()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            //Application.DoEvents();

            MainForm_Load_InitializationCode(mf);

            MainForm_ForceUpdateViewType_List(mf, false);

            MainForm_GroupListView_Checked_ListOrDataGrid(mf);

            MainForm_ForceUpdateViewType_Split(mf, Orientation.Vertical, true);

            mf.UpdateRootSplitterViewControl();

            // Act
            Application.DoEvents();


            // Assert
            Assert.IsInstanceOfType(mf.RootSplitterView.c1,
                    typeof(TimerGroupListView));
            Assert.IsInstanceOfType(mf.RootSplitterView.c2,
                    typeof(SplitterView));
            Assert.IsInstanceOfType((mf.RootSplitterView.c2 as
                SplitterView).c1, typeof(ClockListView));
            Assert.IsInstanceOfType((mf.RootSplitterView.c2 as
                SplitterView).c2, typeof(ClockDataGridView));

            Assert.AreEqual(1, mf.RootSplitterView.Panel1.Controls.Count);
            Assert.AreEqual(1, mf.RootSplitterView.Panel2.Controls.Count);

            Assert.IsFalse(mf.RootSplitterView.Panel1Collapsed);
            Assert.IsFalse(mf.RootSplitterView.Panel2Collapsed);

            Assert.AreEqual(mf.RootViewData,
                mf.RootViewData.GetFirstChild().ParentViewData);

            Assert.AreEqual(mf.tableLayoutPanel1, mf.RootSplitterView.Parent);

            Assert.AreEqual(3, mf.tableLayoutPanel1.Controls.Count);
        }

        private void MainForm_GroupListView_Checked_ListOrDataGrid(MainForm mf)
        {
            mf.MyClocksViewProvider.ShowGroupList_ListOrDataGrid();
        }

        /// <summary>
        /// show the default List view w/o GroupList,
        /// then split the view
        /// </summary>
        [TestMethod]
        public void ViewDataTest_DoSplit()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            Application.DoEvents();

            MainForm_Load_InitializationCode(mf);

            Application.DoEvents();
            
            ViewData vd = mf.LastFocusedTimersView.MyViewData;

            mf.UpdateRootSplitterViewControl();

            // TODO: HERE, mf.LastFocusedTimersView.MyViewData is still not null?

            MainForm_ForceUpdateViewType_List(mf, false);

            //mf.RootViewData.SetFirstChild(vd.GetFirstChild());


            mf.UpdateRootSplitterViewControl();

            mf.DoSplit(Orientation.Horizontal);

            //MainForm_GroupListView_Checked_ListOrDataGrid(mf);

            // MainForm_ForceUpdateViewType_Split(mf, Orientation.Vertical, true);

            mf.UpdateRootSplitterViewControl();

            // Act
            Application.DoEvents();


            // Assert
            Assert.IsInstanceOfType(mf.RootSplitterView.c1,
                    typeof(ClockListView));
            Assert.IsInstanceOfType(mf.RootSplitterView.c2,
                    typeof(ClockListView));

            Assert.AreEqual(1, mf.RootSplitterView.Panel1.Controls.Count);
            Assert.AreEqual(1, mf.RootSplitterView.Panel2.Controls.Count);

            Assert.IsFalse(mf.RootSplitterView.Panel1Collapsed);
            Assert.IsFalse(mf.RootSplitterView.Panel2Collapsed);

            Assert.AreEqual(mf.RootViewData,
                mf.RootViewData.GetFirstChild().ParentViewData);

            Assert.AreEqual(mf.tableLayoutPanel1, mf.RootSplitterView.Parent);

            Assert.AreEqual(3, mf.tableLayoutPanel1.Controls.Count);
        }

        /// <summary>
        /// show the default List view w/o GroupList
        /// </summary>
        [TestMethod]
        public void ViewDataTest_MainForm_ForceUpdateViewType_List_2()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            MainForm_Load_InitializationCode(mf);

            Application.DoEvents();

            MainForm_ForceUpdateViewType_List(mf, false);

            mf.UpdateRootSplitterViewControl();

            // Act
            Application.DoEvents();

            // Assert
            Assert.IsNotNull(mf.LastFocusedTimersView.MyViewData.View);
        }

        // TODO: use ExpectedException attribute
        // test that an exception is raised
        // when there are more than 1 ViewData-s with the same View property value or
        // when there are more than 2 ViewData-s with the same ParentViewData property value
        //[TestMethod]
        public void ViewDataTest29()
        {
            throw new NotImplementedException();

            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.Show();

            ViewData vdParent = new ViewData(mf.MyDataFile)
            {
                ViewType = ViewDataType.Splitter
            };
            ISplitterPanel view = new ClockListView(mf);

            var vd1 = new ViewData(mf.MyDataFile)
            {
                Orientation = Orientation.Horizontal,
                ParentViewData = vdParent,
                View = view,
                ViewType = ViewDataType.List,
                ZoomPercent = 170
            };
            var vd2 = new ViewData(mf.MyDataFile)
            {
                Orientation = Orientation.Horizontal,
                ParentViewData = vdParent,
                View = view,
                ViewType = ViewDataType.DataGrid,
                ZoomPercent = 170
            };
            var vd3 = new ViewData(mf.MyDataFile)
            {
                Orientation = Orientation.Horizontal,
                ParentViewData = vdParent,
                View = view,
                ViewType = ViewDataType.GroupList,
                ZoomPercent = 170
            };

            // Act
            Application.DoEvents();

            // Assert
            Assert.AreEqual(Orientation.Horizontal, vd2.Orientation);
            Assert.AreEqual(vdParent, vd2.ParentViewData);
            Assert.AreEqual(view, vd2.View);
            Assert.AreEqual(ViewDataType.List, vd2.ViewType);
            Assert.AreEqual(170, vd2.ZoomPercent);
            //Assert.IsNull(vd.View, "View passed with CloneWithoutChildren method but remains in the original ViewData.");
        }

        [TestMethod]
        public void ViewDataTest_HideGroupList()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();


            MainForm_Load_InitializationCode(mf);

            MainForm_ForceUpdateViewType_List(mf, false);

            MainForm_ForceUpdateViewType_Split(mf, Orientation.Vertical, true);

            mf.MyClocksViewProvider.HideGroupList(true);
            
            //mf.RootViewData.RecreateThisView();

            mf.UpdateRootSplitterViewControl();

            // Act
            Application.DoEvents();

            // Assert
            Assert.IsInstanceOfType(mf.RootViewData.GetFirstChild().View, typeof(ClockListView));
            Assert.IsInstanceOfType(mf.RootViewData.GetSecondChild().View, typeof(ClockDataGridView));
        }

        [TestMethod]
        public void ViewDataTest_UpdateRootSplitterViewControl_2()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            //Application.DoEvents();

            MainForm_Load_InitializationCode(mf);

            MainForm_ForceUpdateViewType_List(mf, false);

            mf.UpdateRootSplitterViewControl();

            // Act
            Application.DoEvents();


            // Assert
            Assert.IsInstanceOfType(mf.RootSplitterView.c1,
                typeof(ClockListView));
            Assert.IsNull(mf.RootSplitterView.c2);

            Assert.AreEqual(1, mf.RootSplitterView.Panel1.Controls.Count);
            Assert.AreEqual(0, mf.RootSplitterView.Panel2.Controls.Count);

            Assert.IsFalse(mf.RootSplitterView.Panel1Collapsed);
            Assert.IsTrue(mf.RootSplitterView.Panel2Collapsed);

            Assert.AreEqual(mf.RootViewData,
                mf.RootViewData.GetFirstChild().ParentViewData);

            Assert.AreEqual(mf.tableLayoutPanel1, mf.RootSplitterView.Parent);

            Assert.AreEqual(3, mf.tableLayoutPanel1.Controls.Count);
        }

        /// <summary>
        /// 1. open normally w/o existing file.
        /// 2. the state is: clock list view.
        /// 3. select to show the GroupList.
        /// 4. select vertical split view easy view.
        /// </summary>
        [TestMethod]
        public void ViewDataTest_ForceUpdateViewType_Split()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            //Application.DoEvents();

            MainForm_Load_InitializationCode(mf);

            MainForm_ForceUpdateViewType_List(mf, false);

            mf.MyClocksViewProvider.ShowGroupList_ListOrDataGrid();

            mf.MyClocksViewProvider.ForceUpdateViewType_Split(Orientation.Vertical, true);

            mf.UpdateRootSplitterViewControl();

            // Act
            Application.DoEvents();


            // Assert
            Assert.IsInstanceOfType(mf.RootSplitterView.c1,
                typeof(TimerGroupListView));
            Assert.IsInstanceOfType(mf.RootSplitterView.c2,
                typeof(SplitterView));
            Assert.IsInstanceOfType((mf.RootSplitterView.c2 as SplitterView).c1,
                typeof(ClockListView));
            Assert.IsInstanceOfType((mf.RootSplitterView.c2 as SplitterView).c2,
                typeof(ClockDataGridView));

            Assert.AreEqual(1, mf.RootSplitterView.Panel1.Controls.Count);
            Assert.AreEqual(1, mf.RootSplitterView.Panel2.Controls.Count);

            Assert.IsFalse(mf.RootSplitterView.Panel1Collapsed);
            Assert.IsFalse(mf.RootSplitterView.Panel2Collapsed);

            Assert.AreEqual(mf.RootViewData,
                mf.RootViewData.GetFirstChild().ParentViewData);

            Assert.AreEqual(mf.tableLayoutPanel1, mf.RootSplitterView.Parent);

            Assert.AreEqual(3, mf.tableLayoutPanel1.Controls.Count);
        }

        /// <summary>
        /// 1. open normally w/o existing file.
        /// 2. the state is: clock list view.
        /// 3. select to show the GroupList.
        /// 4. hide the GroupList.
        /// </summary>
        [TestMethod]
        public void ViewDataTest_HideGroupList_2()
        {
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();
            mf.OnLoadCreateBasicViewDataWithView = false;
            mf.OnLoadUpdateViewType = false;

            mf.Show();

            //Application.DoEvents();

            MainForm_Load_InitializationCode(mf);

            MainForm_ForceUpdateViewType_List(mf, false);

            mf.MyClocksViewProvider.ShowGroupList_ListOrDataGrid();

            mf.MyClocksViewProvider.HideGroupList(false);

            mf.UpdateRootSplitterViewControl();

            // Act
            Application.DoEvents();


            // Assert
            Assert.IsInstanceOfType(mf.RootSplitterView.c1,
                typeof(ClockListView));
            Assert.IsNull(mf.RootSplitterView.c2);

            Assert.AreEqual(1, mf.RootSplitterView.Panel1.Controls.Count);
            Assert.AreEqual(0, mf.RootSplitterView.Panel2.Controls.Count);

            Assert.IsFalse(mf.RootSplitterView.Panel1Collapsed);
            Assert.IsTrue(mf.RootSplitterView.Panel2Collapsed);

            Assert.AreEqual(mf.RootViewData,
                mf.RootViewData.GetFirstChild().ParentViewData);

            Assert.AreEqual(mf.tableLayoutPanel1, mf.RootSplitterView.Parent);

            Assert.AreEqual(3, mf.tableLayoutPanel1.Controls.Count);
        }
    }
}