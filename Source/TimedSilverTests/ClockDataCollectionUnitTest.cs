using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimedSilverTests
{
    [TestClass]
    public class ClockDataCollectionUnitTest
    {
        [TestMethod]
        public void ClockDataCollectionTest_IsUnsaved_change()
        {
            // Arrange
            var f = new MainForm();

            var td = new TimerData(f.MyDataFile, f.MultiAudioPlayer);
            td.IsUnsaved = true;

            f.MyDataFile.ClockMCollection.AddClock(td);
            f.MyDataFile.ClockMCollection.IsUnsaved = false;
            
            // Assert
            Assert.IsFalse(td.IsUnsaved);
        }

        [TestMethod]
        public void ClockDataCollectionTest_AddClock()
        {
            // Arrange
            var f = new MainForm();

            var td = new TimerData(f.MyDataFile, f.MultiAudioPlayer);

            // Act
            var eventCalled = false;

            f.MyDataFile.ClockMCollection.ClockAdded += delegate (object s, ClockEventArgs e)
            {
                eventCalled = true;
            };
            
            f.MyDataFile.ClockMCollection.AddClock(td);

            // Assert
            Assert.IsTrue(eventCalled);
            Assert.IsTrue(f.MyDataFile.
                ClockMCollection.Contains(td));
            Assert.IsTrue(f.MyDataFile.
                ClockMCollection.IsUnsaved);
        }

        [TestMethod]
        public void ClockDataCollectionTest_AppliedFilter_change()
        {
            // Arrange
            var f = new MainForm();

            var c0 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                GroupName = ""
            };
            var cTest = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
            {
                GroupName = "test"
            };

            f.MyDataFile.ClockMCollection.AddClocks(c0, cTest);

            f.MyDataFile.ClockMCollection.AppliedFilter = new FilterM(f.MyDataFile.ClockMCollection, "");

            // Assert
            Assert.IsFalse(f.MyDataFile.ClockMCollection.AppliedFilter.Autocorrected);
            Assert.IsFalse(c0.FilteredOut); // the clock outside of any group should be shown only on "" filter
            Assert.IsFalse(cTest.FilteredOut); // the clock in group "test" should be shown only in the "" filter and "test" filter

            f.MyDataFile.ClockMCollection.AppliedFilter = new FilterM(f.MyDataFile.ClockMCollection, "1");

            Assert.IsFalse(f.MyDataFile.ClockMCollection.AppliedFilter.Autocorrected);
            Assert.IsTrue(c0.FilteredOut);
            Assert.IsFalse(cTest.FilteredOut);

            f.MyDataFile.ClockMCollection.AppliedFilter = new FilterM(f.MyDataFile.ClockMCollection, "alarms");

            Assert.IsFalse(f.MyDataFile.ClockVMCollection.Model.AppliedFilter.Autocorrected);
            Assert.IsTrue(c0.FilteredOut);
            Assert.IsFalse(cTest.FilteredOut);

            f.MyDataFile.ClockMCollection.AppliedFilter = new FilterM(f.MyDataFile.ClockMCollection, "2");

            Assert.IsTrue(f.MyDataFile.ClockMCollection.AppliedFilter.Autocorrected);
            Assert.IsFalse(c0.FilteredOut);
            Assert.IsFalse(cTest.FilteredOut);
        }

        [TestMethod]
        public void ClockDataCollectionTest_ClockSatisfiesFilter()
        {
            // Arrange
            var f = new MainForm();

            var c0 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                GroupName = ""
            };
            var cTest = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
            {
                GroupName = "test"
            };

            // Act
            f.MyDataFile.ClockMCollection.AddClocks(c0, cTest);

            var f1 = new FilterM(f.MyDataFile.ClockMCollection, "");
            Assert.IsFalse(f1.Autocorrected);
            Assert.IsTrue(ClockMCollection.ClockSatisfiesFilter(f1, c0));
            Assert.IsTrue(ClockMCollection.ClockSatisfiesFilter(f1, cTest));

            var f2 = new FilterM(f.MyDataFile.ClockMCollection, "1");
            Assert.IsFalse(f2.Autocorrected);
            Assert.IsFalse(ClockMCollection.ClockSatisfiesFilter(f2, c0));
            Assert.IsTrue(ClockMCollection.ClockSatisfiesFilter(f2, cTest));

            var f3 = new FilterM(f.MyDataFile.ClockMCollection, "alarms");
            Assert.IsFalse(f3.Autocorrected);
            Assert.IsFalse(ClockMCollection.ClockSatisfiesFilter(f3, c0));
            Assert.IsTrue(ClockMCollection.ClockSatisfiesFilter(f3, cTest));

            var f4 = new FilterM(f.MyDataFile.ClockMCollection, "2");
            Assert.IsTrue(f4.Autocorrected);
            Assert.IsTrue(ClockMCollection.ClockSatisfiesFilter(f4, c0));
            Assert.IsTrue(ClockMCollection.ClockSatisfiesFilter(f4, cTest));
        }

        [TestMethod]
        public void ClockDataCollectionTest_OneTimeFilter()
        {
            // Arrange
            var f = new MainForm();

            var c0 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                GroupName = ""
            };
            var cTest = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
            {
                GroupName = "test"
            };

            // Act
            f.MyDataFile.ClockMCollection.AddClocks(c0, cTest);

            var f1 = new FilterM(f.MyDataFile.ClockMCollection, "");
            Assert.AreEqual(
                f.MyDataFile.ClockMCollection.OneTimeFilter(f1).Count(),
                2);

            var f2 = new FilterM(f.MyDataFile.ClockMCollection, "1");
            Assert.AreEqual(
                f.MyDataFile.ClockMCollection.OneTimeFilter(f2).Count(),
                1);

            var f3 = new FilterM(f.MyDataFile.ClockMCollection, "alarms");
            Assert.AreEqual(
                f.MyDataFile.ClockMCollection.OneTimeFilter(f3).Count(),
                1);

            var f4 = new FilterM(f.MyDataFile.ClockMCollection, "2");
            Assert.AreEqual(
                f.MyDataFile.ClockMCollection.OneTimeFilter(f4).Count(),
                2);
        }

        [TestMethod]
        public void ClockDataCollectionTest_RingPassedAlarms()
        {
            // Arrange
            var f = new MainForm();

            // Act
            var ad = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
            {
                CurrentDateTime = new DateTime(DateTime.Now.Ticks) - TimeSpan.FromSeconds(1),
                Enabled = true
            };
            f.MyDataFile.ClockMCollection.AddClock(ad);
            f.MyDataFile.ClockMCollection.RingPassedAlarms();

            // Assert
            Assert.IsFalse(ad.Enabled);
            Assert.IsTrue(f.MyTimeOutFormsManager.HasVisibleForm);
        }

        [TestMethod]
        public void ClockDataCollectionTest_UpdateTaskBarProgress()
        {
            // Arrange
            var f = new MainForm();

            // Act
            var ad = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                CurrentTimeSpan = TimeSpan.FromSeconds(10)
            };
            
            f.MyDataFile.ClockMCollection.AddClock(ad);

            Application.DoEvents();

            f.MyDataFile.ClockMCollection.ReportGlobalProgress();
            
            // Assert
            Assert.IsTrue(f.LastTaskBarProgressPercent == -1D);
            
            ad.StartOrStop();

            Application.DoEvents();

            f.MyDataFile.ClockMCollection.ReportGlobalProgress();

            // Assert
            Assert.IsTrue(f.LastTaskBarProgressPercent > 50D);
        }

        [TestMethod]
        public void ClockDataCollectionTest_RemoveClock()
        {
            // Arrange
            var f = new MainForm();

            var td = new TimerData(f.MyDataFile, f.MultiAudioPlayer);

            // Act
            var eventCalled = false;

            f.MyDataFile.ClockMCollection.ClockRemoved += delegate (object s, ClockEventArgs e)
            {
                eventCalled = true;
            };

            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClock(td);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;
            
            f.MyDataFile.ClockMCollection.RemoveClock(td);

            // Assert
            Assert.IsTrue(eventCalled);
            Assert.IsFalse(f.MyDataFile.
                ClockMCollection.Contains(td));
            Assert.IsTrue(f.MyDataFile.
                ClockMCollection.IsUnsaved);
        }

        [TestMethod]
        public void ClockDataCollectionTest_RemoveAllClocks()
        {
            // Arrange
            var f = new MainForm();

            var td = new TimerData(f.MyDataFile, f.MultiAudioPlayer);

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClock(td);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            f.MyDataFile.ClockMCollection.RemoveAllClocks();

            // Assert
            Assert.AreEqual(0, f.MyDataFile.
                ClockMCollection.Count());
            Assert.IsTrue(f.MyDataFile.
                ClockMCollection.IsUnsaved);
        }

        [TestMethod]
        public void ClockDataCollectionTest_SwapClocksByIndices()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1"
            };
            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t2"
            };

            var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t3"
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1, td2, td3);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            bool triggered = false;
            f.MyDataFile.ClockMCollection.ClocksSwapped += delegate (object sender, ClocksSwappedEventArgs e)
            {
                triggered = true;

                Assert.AreEqual("t3", e.FirstClockInNewOrder.Tag);
                Assert.AreEqual("t1", e.SecondClockInNewOrder.Tag);
            };
            f.MyDataFile.ClockMCollection.SwapClocksByIndices(0, 2);

            // Assert
            Assert.IsTrue(triggered);
            Assert.AreEqual("t3", f.MyDataFile.
                ClockMCollection.Ms[0].Tag);
            Assert.AreEqual("t1", f.MyDataFile.
                ClockMCollection.Ms[2].Tag);
        }

        [TestMethod]
        public void ClockDataCollectionTest_SwapClocksByIndices_2()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1"
            };
            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t2"
            };

            var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t3"
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1, td2, td3);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            bool triggered = false;
            f.MyDataFile.ClockMCollection.ClocksSwapped += delegate (object sender, ClocksSwappedEventArgs e)
            {
                triggered = true;

                Assert.AreEqual("t3", e.FirstClockInNewOrder.Tag);
                Assert.AreEqual("t1", e.SecondClockInNewOrder.Tag);
            };
            f.MyDataFile.ClockMCollection.SwapClocksByIndices(2, 0);

            // Assert
            Assert.IsTrue(triggered);
            Assert.AreEqual("t3", f.MyDataFile.
                ClockMCollection.Ms[0].Tag);
            Assert.AreEqual("t1", f.MyDataFile.
                ClockMCollection.Ms[2].Tag);
        }

        [TestMethod]
        public void ClockDataCollectionTest_MoveClockFromIndexToIndex()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1"
            };

            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t2"
            };

            var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t3"
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1, td2, td3);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            bool triggered = false;
            f.MyDataFile.ClockMCollection.ClockMoved += delegate (object sender, ClockMovedEventArgs e)
            {
                triggered = true;

                Assert.AreEqual(td1, e.Clock);
                Assert.AreEqual(2, e.NewIndex);
                Assert.AreEqual(0, e.OldIndex);
            };
            f.MyDataFile.ClockMCollection.MoveClockFromIndexToIndex(0, 2);

            // Assert
            Assert.IsTrue(triggered);
            Assert.AreEqual("t2", f.MyDataFile.
                ClockMCollection.Ms[0].Tag);
            Assert.AreEqual("t3", f.MyDataFile.
                ClockMCollection.Ms[1].Tag);
            Assert.AreEqual("t1", f.MyDataFile.
                ClockMCollection.Ms[2].Tag);
            Assert.IsTrue(f.MyDataFile.ClockMCollection.IsUnsaved);
            Assert.IsTrue(f.MyDataFile.IsUnsaved);
        }

        [TestMethod]
        public void ClockDataCollectionTest_ChangeTypeOfClock()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1"
            };

            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t2"
            };

            var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t3"
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1, td2, td3);
            f.MyDataFile.ClockMCollection.AppliedFilter = new FilterM(f.MyDataFile.ClockMCollection, "");
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            string tag = td2.Tag;

            bool triggered = false;
            f.MyDataFile.ClockMCollection.ClockChangedType += delegate (object sender, ClockEventArgs e)
            {
                triggered = true;
            };

            ClockM newClock = f.MyDataFile.ClockMCollection.
                ChangeTypeOfClock(td2, typeof(AlarmData));

            Assert.IsTrue(triggered);
            Assert.AreEqual(f.MyDataFile.ClockMCollection.Ms[1], newClock);
            Assert.IsInstanceOfType(newClock, typeof(AlarmData));
            Assert.AreEqual(tag, newClock.Tag);
            Assert.IsTrue(f.MyDataFile.ClockMCollection.IsUnsaved);
            Assert.IsTrue(f.MyDataFile.IsUnsaved);
        }

        [TestMethod]
        public void ClockDataCollectionTest_Td_GroupNameChanged()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1",
                GroupName = "test"
            };

            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t2"
            };

            var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t3"
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1, td2, td3);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            f.MyDataFile.ClockMCollection.AppliedFilter = new FilterM(f.MyDataFile.ClockMCollection, "1");

            td3.GroupName = "test";

            Assert.IsFalse(td1.FilteredOut);
            Assert.IsTrue(td2.FilteredOut);
            Assert.IsFalse(td3.FilteredOut);
        }

        [TestMethod]
        public void ClockDataCollectionTest_Td_PropertyChanged()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1",
                GroupName = "test"
            };

            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t2"
            };

            var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t3"
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1, td2, td3);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            f.MyDataFile.ClockMCollection.AppliedFilter = new FilterM(f.MyDataFile.ClockMCollection, "");

            bool triggered = false;
            f.MyDataFile.ClockMCollection.ClockPropertyChanged += delegate (object sender,
                PropertyChangedEventArgs e)
            {
                triggered = true;
            };

            f.MyDataFile.ClockMCollection.AutosortMode = AutosortMode.Alphabetically;

            td2.Tag = "20"; // If the Tag property is changed, the clock collection is sorted
                            // alphabetically.
                            
            Assert.IsTrue(triggered);

            Assert.AreEqual(td2, f.MyDataFile.ClockMCollection.Ms[0]);
            Assert.AreEqual(td1, f.MyDataFile.ClockMCollection.Ms[1]);
            Assert.AreEqual(td3, f.MyDataFile.ClockMCollection.Ms[2]);
        }

        [TestMethod]
        public void ClockDataCollectionTest_Td_FilteredOutChanged()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1",
                GroupName = "test",
                FilteredOut = true // this is not kept, there is already an applied filter
            };

            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t2"
            };

            var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t3"
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1, td2, td3);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            int triggeredIn = 0;
            f.MyDataFile.ClockMCollection.ClockFilteredIn += delegate (object sender,
                ClockEventArgs e)
            {
                Assert.AreEqual(e.Clock, td1);

                ++triggeredIn;
            };

            int triggeredOut = 0;
            f.MyDataFile.ClockMCollection.ClockFilteredOut += delegate (object sender,
    ClockEventArgs e)
            {
                Assert.IsTrue(e.Clock == td2 ||
                    e.Clock == td3);

                ++triggeredOut;
            };

            f.MyDataFile.ClockMCollection.AppliedFilter = new FilterM(f.MyDataFile.ClockMCollection, "1");

            Assert.IsFalse(td1.FilteredOut);
            Assert.IsTrue(td2.FilteredOut);
            Assert.IsTrue(td3.FilteredOut);
            Assert.AreEqual(0, triggeredIn);
            Assert.AreEqual(2, triggeredOut);
        }

        [TestMethod]
        public void ClockDataCollectionTest_Td_TimerStoppedByUser()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1",
                GroupName = "test",
                CurrentTimeSpan = TimeSpan.FromSeconds(10)
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            int count = 0;

            f.MyDataFile.ClockMCollection.TimerStoppedByUser += delegate (object sender, ClockEventArgs e)
            {
                ++count;
            };

            td1.StartOrStop();
            td1.StartOrStop();

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void ClockDataCollectionTest_DoSortByClosestRingingMoment()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1",
                GroupName = "test",
                CurrentTimeSpan = TimeSpan.FromSeconds(10)
            };

            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t2",
                CurrentTimeSpan = TimeSpan.FromSeconds(5)
            };

            var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t3",
                CurrentTimeSpan = TimeSpan.FromSeconds(2)
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1, td2, td3);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            td1.StartOrStop();
            td2.StartOrStop();
            td3.StartOrStop();

            f.MyDataFile.ClockMCollection.AutosortMode = AutosortMode.None;
            f.MyDataFile.ClockMCollection.DoSortByClosestRingingMoment();

            // order remains the same
            Assert.AreEqual(td1, f.MyDataFile.ClockMCollection.Ms[0]);
            Assert.AreEqual(td2, f.MyDataFile.ClockMCollection.Ms[1]);
            Assert.AreEqual(td3, f.MyDataFile.ClockMCollection.Ms[2]);

            f.MyDataFile.ClockMCollection.AutosortMode = AutosortMode.Alphabetically;
            f.MyDataFile.ClockMCollection.DoSortByClosestRingingMoment();

            // order remains the same
            Assert.AreEqual(td1, f.MyDataFile.ClockMCollection.Ms[0]);
            Assert.AreEqual(td2, f.MyDataFile.ClockMCollection.Ms[1]);
            Assert.AreEqual(td3, f.MyDataFile.ClockMCollection.Ms[2]);

            f.MyDataFile.ClockMCollection.AutosortMode = AutosortMode.ClosestRingingMoment;
            f.MyDataFile.ClockMCollection.DoSortByClosestRingingMoment();

            // order changed
            Assert.AreEqual(td1, f.MyDataFile.ClockMCollection.Ms[2]);
            Assert.AreEqual(td2, f.MyDataFile.ClockMCollection.Ms[1]);
            Assert.AreEqual(td3, f.MyDataFile.ClockMCollection.Ms[0]);
        }

        [TestMethod]
        public void ClockDataCollectionTest_SortByClosestRingingMoment()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1",
                GroupName = "test",
                CurrentTimeSpan = TimeSpan.FromMinutes(60)
            };

            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t2",
                CurrentTimeSpan = TimeSpan.FromMinutes(50)
            };

            var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t3",
                CurrentTimeSpan = TimeSpan.FromMinutes(20)
            };

            var ad4 = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "a4",
                CurrentDateTime = DateTime.Now + TimeSpan.FromMinutes(40)
            };

            var ad5 = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "a5",
                CurrentDateTime = DateTime.Now + TimeSpan.FromMinutes(1000)
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1, td2, td3, ad4, ad5);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            td1.StartOrStop();
            td2.StartOrStop();
            td3.StartOrStop();
            ad4.ActivateOrDeactivate();

            f.MyDataFile.ClockMCollection.AutosortMode = AutosortMode.ClosestRingingMoment;
            f.MyDataFile.ClockMCollection.SortByClosestRingingMoment();

            // order changed
            Assert.AreEqual(td3, f.MyDataFile.ClockMCollection.Ms[0]);
            Assert.AreEqual(ad4, f.MyDataFile.ClockMCollection.Ms[1]);
            Assert.AreEqual(td2, f.MyDataFile.ClockMCollection.Ms[2]);
            Assert.AreEqual(td1, f.MyDataFile.ClockMCollection.Ms[3]);
            Assert.AreEqual(ad5, f.MyDataFile.ClockMCollection.Ms[4]);
            Assert.IsTrue(f.MyDataFile.ClockMCollection.IsUnsaved);
        }

        // NOTE: this test is not needed because I will be using data binding in XAML.
        //[TestMethod]
        //public void ClockDataCollectionTest_FindIndexOfClockWithView()
        //{
        //    // Arrange
        //    var f = new MainForm();
        //    //f.OnLoadCreateBasicViewDataWithView = false;
        //    //f.OnLoadUpdateViewType = false;
        //    f.Show();

        //    var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
        //    {
        //        Tag = "t1",
        //        GroupName = "test",
        //        CurrentTimeSpan = TimeSpan.FromMinutes(60)
        //    };

        //    var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
        //    {
        //        Tag = "t2",
        //        CurrentTimeSpan = TimeSpan.FromMinutes(50)
        //    };

        //    var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
        //    {
        //        Tag = "t3",
        //        CurrentTimeSpan = TimeSpan.FromMinutes(20)
        //    };

        //    var ad4 = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
        //    {
        //        Tag = "a4",
        //        CurrentDateTime = DateTime.Now + TimeSpan.FromMinutes(40)
        //    };

        //    var ad5 = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
        //    {
        //        Tag = "a5",
        //        CurrentDateTime = DateTime.Now + TimeSpan.FromMinutes(1000)
        //    };

        //    f.MyDataFile.ClockCollection.RemoveAllClocks();

        //    // Act
        //    f.MyDataFile.ClockCollection.IsUnsavedLocked = true;
        //    f.MyDataFile.ClockCollection.AddClocks(td1, td2, td3, ad4, ad5);
        //    f.MyDataFile.ClockCollection.IsUnsavedLocked = false;

        //    Assert.AreEqual(td1.MyTimerViews[0],
        //        f.MyClocksViewProvider.GetExistingOrNewClockListView().
        //            MyClockFlowLayoutPanel.Controls[0]);

        //    Assert.AreEqual(0,
        //        f.MyDataFile.ClockCollection.
        //            FindIndexOfClockWithView(td1.MyTimerViews[0]));
        //    Assert.AreEqual(1,
        //        f.MyDataFile.ClockCollection.
        //            FindIndexOfClockWithView(td2.MyTimerViews[0]));
        //    Assert.AreEqual(2,
        //        f.MyDataFile.ClockCollection.
        //            FindIndexOfClockWithView(td3.MyTimerViews[0]));
        //    Assert.AreEqual(3,
        //        f.MyDataFile.ClockCollection.
        //            FindIndexOfClockWithView(ad4.MyTimerViews[0]));
        //    Assert.AreEqual(4,
        //        f.MyDataFile.ClockCollection.
        //            FindIndexOfClockWithView(ad5.MyTimerViews[0]));

        //    Assert.AreEqual(td1.MyTimerViews[0], f.MyClocksViewProvider.GetExistingOrNewClockListView().MyClockFlowLayoutPanel.Controls[0]);
        //    Assert.AreEqual(0,
        //        f.MyDataFile.ClockCollection.
        //            FindIndexOfClockWithView(td1.MyTimerViews[1]));
        //    Assert.AreEqual(1,
        //        f.MyDataFile.ClockCollection.
        //            FindIndexOfClockWithView(td2.MyTimerViews[1]));
        //    Assert.AreEqual(2,
        //        f.MyDataFile.ClockCollection.
        //            FindIndexOfClockWithView(td3.MyTimerViews[1]));
        //    Assert.AreEqual(3,
        //        f.MyDataFile.ClockCollection.
        //            FindIndexOfClockWithView(ad4.MyTimerViews[1]));
        //    Assert.AreEqual(4,
        //        f.MyDataFile.ClockCollection.
        //            FindIndexOfClockWithView(ad5.MyTimerViews[1]));
        //}

        [TestMethod]
        public void ClockDataCollectionTest_SortAlphabetically()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1",
                GroupName = "test",
                CurrentTimeSpan = TimeSpan.FromMinutes(60)
            };

            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t2",
                CurrentTimeSpan = TimeSpan.FromMinutes(50)
            };

            var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t3",
                CurrentTimeSpan = TimeSpan.FromMinutes(20)
            };

            var ad4 = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "a4",
                CurrentDateTime = DateTime.Now + TimeSpan.FromMinutes(40)
            };

            var ad5 = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "a5",
                CurrentDateTime = DateTime.Now + TimeSpan.FromMinutes(1000)
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1, td2, td3, ad4, ad5);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;
            
            f.MyDataFile.ClockMCollection.SortAlphabetically();

            // order changed
            Assert.AreEqual(ad4, f.MyDataFile.ClockMCollection.Ms[0]);
            Assert.AreEqual(ad5, f.MyDataFile.ClockMCollection.Ms[1]);
            Assert.AreEqual(td1, f.MyDataFile.ClockMCollection.Ms[2]);
            Assert.AreEqual(td2, f.MyDataFile.ClockMCollection.Ms[3]);
            Assert.AreEqual(td3, f.MyDataFile.ClockMCollection.Ms[4]);
            Assert.IsTrue(f.MyDataFile.ClockMCollection.IsUnsaved);
        }

        // NOTE: this test is not needed because I will use data binding in XAML
        // also in a DataGrid.
        //[TestMethod]
        //public void ClockDataCollectionTest_SortClockViewsByClocksData()
        //{
        //    // Arrange
        //    var f = new MainForm();

        //    var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
        //    {
        //        Tag = "t1",
        //        GroupName = "test",
        //        CurrentTimeSpan = TimeSpan.FromMinutes(60)
        //    };

        //    var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
        //    {
        //        Tag = "t2",
        //        CurrentTimeSpan = TimeSpan.FromMinutes(50)
        //    };

        //    var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
        //    {
        //        Tag = "t3",
        //        CurrentTimeSpan = TimeSpan.FromMinutes(20)
        //    };

        //    var ad4 = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
        //    {
        //        Tag = "a4",
        //        CurrentDateTime = DateTime.Now + TimeSpan.FromMinutes(40)
        //    };

        //    var ad5 = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
        //    {
        //        Tag = "a5",
        //        CurrentDateTime = DateTime.Now + TimeSpan.FromMinutes(1000)
        //    };

        //    // Act
        //    f.MyDataFile.ClockCollection.IsUnsavedLocked = true;
        //    f.MyDataFile.ClockCollection.AddClocks(td1, td2, td3, ad4, ad5);
        //    f.MyDataFile.ClockCollection.IsUnsavedLocked = false;

        //    ClockData aux = f.MyDataFile.ClockCollection.ClocksData[1];
        //    f.MyDataFile.ClockCollection.ClocksData[1] = f.MyDataFile.ClockCollection.ClocksData[3];
        //    f.MyDataFile.ClockCollection.ClocksData[3] = aux;

        //    // note: this call does not change the IsUnsaved properties of the ClockCollection and DataFile
        //    f.MyDataFile.ClockCollection.SortClockViewsByClocksData();
            
        //    // order changed
            
        //    ClockFlowLayoutPanelOld cfp = f.MyClocksViewProvider.GetExistingOrNewClockListView
        //        ().MyClockFlowLayoutPanel;

        //    Assert.AreEqual(0, f.MyDataFile.ClockCollection.FindIndexOfClockWithView(cfp.Controls[0] as ClockControl));
        //    Assert.AreEqual(1, f.MyDataFile.ClockCollection.FindIndexOfClockWithView(cfp.Controls[1] as ClockControl));
        //    Assert.AreEqual(2, f.MyDataFile.ClockCollection.FindIndexOfClockWithView(cfp.Controls[2] as ClockControl));
        //    Assert.AreEqual(3, f.MyDataFile.ClockCollection.FindIndexOfClockWithView(cfp.Controls[3] as ClockControl));
        //    Assert.AreEqual(4, f.MyDataFile.ClockCollection.FindIndexOfClockWithView(cfp.Controls[4] as ClockControl));
        //}


        [TestMethod]
        public void ClockDataCollectionTest_Equals()
        {
            // Arrange
            var f = new MainForm();

            var td1 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t1",
                GroupName = "test",
                CurrentTimeSpan = TimeSpan.FromMinutes(60)
            };

            var td2 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t2",
                CurrentTimeSpan = TimeSpan.FromMinutes(50)
            };

            var td3 = new TimerData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "t3",
                CurrentTimeSpan = TimeSpan.FromMinutes(20)
            };

            var ad4 = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "a4",
                CurrentDateTime = DateTime.Now + TimeSpan.FromMinutes(40)
            };

            var ad5 = new AlarmData(f.MyDataFile, f.MultiAudioPlayer)
            {
                Tag = "a5",
                CurrentDateTime = DateTime.Now + TimeSpan.FromMinutes(1000)
            };

            // Act
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = true;
            f.MyDataFile.ClockMCollection.AddClocks(td1, td2, td3, ad4, ad5);
            f.MyDataFile.ClockMCollection.IsUnsavedLocked = false;

            var cc = new ClockMCollection(f.MyDataFile);
            cc.Groups = f.MyDataFile.ClockMCollection.Groups;
            cc.AddClocks(td1, td2, td3, ad4, ad5);

            Assert.AreEqual(f.MyDataFile.ClockMCollection, cc);

            cc.RemoveAllClocks();
            cc.Groups = f.MyDataFile.ClockMCollection.Groups;
            cc.AddClocks(td2, td1, td3, ad4, ad5);

            Assert.AreNotEqual(f.MyDataFile.ClockMCollection, cc);
        }
    }
}
