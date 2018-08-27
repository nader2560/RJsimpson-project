using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJsimpson
{
    public partial class WaveFormUserControl : Form
    {
        AudioLevelMonitor audioMonitor;

        public WaveFormUserControl(int processID)
        {
            InitializeComponent();
            this.Text = "SoundLevelMonitor";
            audioMonitor = new AudioLevelMonitor(processID);
            audioLevelsControl.AudioMonitor = audioMonitor;
        }
    }
}
