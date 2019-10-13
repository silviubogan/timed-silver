using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Shell;

[assembly: InternalsVisibleTo("TimedSilverTests")]

namespace cs_timed_silver
{
    internal class Program
    {
        static internal bool LogToFile = false; // for unit testing

        static internal Program DefaultProgram;

        internal string[] MyArgv;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] argv)
        {
            LogToFile = true;

            System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();

            if (System.Windows.Application.Current == null)
            {
                var wrapper = new SingleInstanceApplicationWrapper();

                DefaultProgram = new Program();
                DefaultProgram.MyArgv = argv;

                wrapper.Run(argv);
            }
        }
    }
}
