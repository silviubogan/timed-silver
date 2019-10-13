using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.ApplicationServices;

namespace cs_timed_silver
{
    public class SingleInstanceApplicationWrapper : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
    {
        public SingleInstanceApplicationWrapper()
        {
            //IsSingleInstance = true;
        }

        private App MyApp;
        protected override bool OnStartup(StartupEventArgs eventArgs)
        {
            MyApp = new App();
            MyApp.Run();

            return false;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            // e.CommandLine.Count, e.CommandLine[0]
        }
    }
}
