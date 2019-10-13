using cs_timed_silver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Windows.Forms;

namespace TimedSilverTests
{
    [TestClass]
    public class AudioFileChooserUnitTest
    {
        [TestMethod]
        public void AudioFileChooserTest_MainForm_change()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            MainForm mf = new MainForm();
            
            // Act
            mf.Show();
            Application.DoEvents();

            // Assert
            Assert.AreEqual(mf, mf.MySettingsForm.audioFileChooser1.recentFilesMenuStrip1.MainForm);
            Assert.IsNotNull(mf.MySettingsForm.audioFileChooser1.AudioPlayer);
        }

        [TestMethod]
        public void AudioFileChooserTest_UpdateDropIndicator()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();

            // Act
            mf.Show();
            Application.DoEvents();
            mf.MySettingsForm.audioFileChooser1.LoadOverlay();
            mf.MySettingsForm.audioFileChooser1.UpdateDropIndicator(); // TODO: test more of this call

            // Assert
            Assert.AreEqual(mf.MySettingsForm.audioFileChooser1.MyOverlay.Visible, mf.MySettingsForm.audioFileChooser1.DraggingOver);
        }

        [TestMethod]
        public void AudioFileChooserTest_SetAudioPath()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();

            // Act
            mf.Show();
            Application.DoEvents();
            string pp = mf.MySettingsForm.audioFileChooser1.FilePath;
            bool eval = mf.MySettingsForm.audioFileChooser1.SetAudioPath("wrong path.wav");
            string np = mf.MySettingsForm.audioFileChooser1.FilePath;

            // Assert
            Assert.IsFalse(eval);

            Assert.AreEqual(pp, np);
        }

        [TestMethod]
        public void AudioFileChooserTest_ResetAudioPath()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            MainForm mf = new MainForm();

            // Act
            mf.Show();
            Application.DoEvents();
            mf.MySettingsForm.audioFileChooser1.SetAudioPath("chimes.wav");

            // Assert 1
            Assert.AreEqual("chimes.wav", mf.MySettingsForm.audioFileChooser1.FilePath);

            mf.MySettingsForm.audioFileChooser1.ResetAudioPath();
            string np = mf.MySettingsForm.audioFileChooser1.FilePath;

            // Assert 2
            Assert.AreEqual("", np);
        }

        [TestMethod]
        public void AudioFileChooserTest_UpdateBtnRecentFilesMenuBottomRightPoint()
        {
            // Arrange
            cs_timed_silver.Properties.Settings.Default.AutoOpenLastFile = "No";
            var mf = new MainForm();

            // Act
            mf.Show();
            Application.DoEvents();
            mf.MySettingsForm.audioFileChooser1.UpdateBtnRecentFilesMenuBottomRightPoint();

            // Assert
            Assert.AreNotEqual(Point.Empty, mf.MySettingsForm.audioFileChooser1.btnRecentFilesMenuBottomRightPoint);
        }
    }
}
