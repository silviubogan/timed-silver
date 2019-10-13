using System.Drawing;
using System.Windows.Forms;

namespace cs_timed_silver
{
    class MyProfessionalColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => Utils.MyDarkDarkGray;

        public override Color SeparatorLight => Color.LightGray;

        public override Color SeparatorDark => Utils.MyDarkDarkGray;

        public override Color MenuItemPressedGradientBegin => Utils.MyDarkDarkGray;

        public override Color MenuItemPressedGradientEnd => Utils.MyDarkDarkGray;

        public override Color MenuItemSelectedGradientBegin => Utils.MyDarkDarkGray;

        public override Color MenuItemSelectedGradientEnd => Utils.MyDarkDarkGray;

        public override Color MenuItemBorder => Color.DarkBlue;

        public override Color MenuBorder => Color.DarkBlue;

        public override Color ToolStripBorder => Utils.MyDarkDarkGray;
        public override Color ToolStripContentPanelGradientBegin => Utils.MyDarkDarkGray;
        public override Color ToolStripContentPanelGradientEnd => Utils.MyDarkDarkGray;
        public override Color ToolStripGradientBegin => Utils.MyDarkDarkGray;
        public override Color ToolStripGradientEnd => Utils.MyDarkDarkGray;
        public override Color ToolStripGradientMiddle => Utils.MyDarkDarkGray;
        public override Color ToolStripPanelGradientBegin => Utils.MyDarkDarkGray;
        public override Color ToolStripPanelGradientEnd => Utils.MyDarkDarkGray;

        public override Color RaftingContainerGradientBegin => Utils.MyDarkDarkGray;
        public override Color RaftingContainerGradientEnd => Utils.MyDarkDarkGray;

        public override Color MenuStripGradientBegin => Utils.MyDarkDarkGray;
        public override Color MenuStripGradientEnd => Utils.MyDarkDarkGray;

        public override Color StatusStripGradientBegin => Utils.MyDarkDarkGray;
        public override Color StatusStripGradientEnd => Utils.MyDarkDarkGray;

        //public override Color GripDark => Utils.MyDarkDarkGray;
        //public override Color GripLight => Utils.MyDarkDarkGray;
    }
}
