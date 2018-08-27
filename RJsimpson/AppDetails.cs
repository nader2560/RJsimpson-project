using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJsimpson
{
    public class AppDetails
    {
        public Icon Icon;

        public Image IconAsImage;
        public Process process;
        public string Name;
        public int processID;
        public AppDetails(Process process)
        {
            
            try
            {
                this.process = process;
                processID = process.Id;
                Name = "";

                if (process != null)
                {
                    if (Name == "") { Name = process.MainWindowTitle; }
                    if (Name == "") { Name = process.ProcessName; }
                }
                if (Name == "") { Name = "--unnamed--"; }
                if (Name.Contains("@")) { Name = "System Sound"; }
                try
                {
                    Icon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                    IconAsImage = Icon.ToBitmap();
                }
                catch(Exception error)
                {
                    if(error.Message.Contains("32 bits"))
                    IconAsImage = Properties.Resources.Computer_Hardware_64bit_icon;
                    else
                    {
                        Name = "System Sounds";
                        IconAsImage = Properties.Resources.windows_;
                    }
                }
            }
            catch (Exception error)
            {
                Name = "System Sounds";
                IconAsImage = Properties.Resources.windows_;
                Console.Write(error.Message);
            }

        }
    }
}
