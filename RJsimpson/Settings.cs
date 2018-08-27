using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSCore.CoreAudioAPI;
using Istrib.Sound;
using NAudio.Lame;
using NAudio.Wave;
using static RJsimpson.AudioLevelMonitor;

namespace RJsimpson
{
    public partial class Settings : Form
    {
        private List<AppDetails> AppDetailsList= new List<AppDetails>();
        Timer dispatcherTimer;
        private AnyAppBroadcaster mainForm;
        public Settings(AnyAppBroadcaster anyApp )
        {
            mainForm = anyApp;
            InitializeComponent();
            _audioMonitor = new AudioLevelMonitor();
            dispatcherTimer = new Timer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = 100;
            dispatcherTimer.Start();
            PopulateListView1();

        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Start(); // trigger next timer
        }
        public void PopulateListView1()
        {
            listView1.View = View.Details;

            //CONSTRUCT COLUMNS
            listView1.Columns.Add("Player", 250);
            listView1.SmallImageList = GetAppIcons();
            listView1.Items.AddRange(GetAppNames());
        }

        private ListViewItem[] GetAppNames()
        {
            List<Process> processes = GetAppList();
            List<ListViewItem> nameList = new List<ListViewItem>();
            int i = 0;
            foreach (Process process in processes)
            {
                AppDetails appDetails = new AppDetails(process);
                nameList.Add(new ListViewItem(appDetails.Name,i));
                i++;
            }
            return nameList.ToArray();
        }

        public ImageList GetAppIcons()
        {
            List<Process> processes = GetAppList();
            ImageList images = new ImageList();
            foreach (Process process in processes)
            {
                AppDetails appDetails = new AppDetails(process);
                AppDetailsList.Add(appDetails);
                images.Images.Add(appDetails.IconAsImage);
                

            }
            return images;
        }
        AudioLevelMonitor _audioMonitor;

        public AudioLevelMonitor AudioMonitor
        {
            get { return _audioMonitor; }
            set
            {
                _audioMonitor = value;
                if (_audioMonitor != null)
                {
                }
            }
        }
        public List<Process> GetAppList()
        {
            var activeSamples = AudioMonitor.GetActiveSamples();
            List<Process> processes = new List<Process>();
            foreach(var session in activeSamples)
            {
                Process process = Process.GetProcessById(session.Value.pid);
                processes.Add(process);
            }

            return processes;
        }

        private void Settings_Load_1(object sender, EventArgs e)
        {
            foreach (SoundCaptureDevice device in SoundCaptureDevice.AllAvailable)
            {
                devicesCmb.Items.Add(device);
                devicesCmb1.Items.Add(device);
            }



        }

        #region Make Form Movable

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
                m.Result = (IntPtr)(HT_CAPTION);
        }

        private const int WM_NCHITTEST = 0x84;
        private const int HT_CAPTION = 0x2;

        #endregion

        #region Close and minimize Functions

        private void Fermer_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Reduire_Click(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Minimized;
        }

        #endregion

        private void refreshButton_Click(object sender, EventArgs e)
        {
            listView1.Clear();
            PopulateListView1();
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            if((devicesCmb.SelectedIndex != -1) && (devicesCmb1.SelectedIndex != -1))
            {
                if (devicesCmb.SelectedIndex != devicesCmb1.SelectedIndex)
                {
                    mainForm.DeviceSelection((SoundCaptureDevice)devicesCmb.SelectedItem, (SoundCaptureDevice)devicesCmb1.SelectedItem);
                    if (listView1.SelectedItems.Count != 0)
                    {
                        mainForm.SetWaveForm(AppDetailsList.ElementAt
                            (listView1.Items.IndexOf(listView1.SelectedItems[0])));

                        this.Close();
                    }
                    else
                    {

                        MessageBox.Show("Please select an application to proceed", "Application Selection Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Please select different devices for the microphone and the output sound", "Device Selection Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select devices for the microphone and the output sound", "Device Selection Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Creates a mashup of two or more mp3 files by using naudio
        /// </summary>
        /// <param name="files">Name of files as an string array</param>
        /// These files should be existing in a temporay folder
        /// <returns>The path of mashed up mp3 file</returns>
        public static string CreateMashup(string[] files)
        {
            // because there is no mash up with less than 2 files
            if (files.Count() < 2)
            {
                throw new Exception("Not enough files selected!");
            }

            try
            {
                // Create a mixer object
                // This will be used for merging files together
                var mixer = new WaveMixerStream32
                {
                    AutoStop = true
                };

                // Set the path to store the mashed up output file
                var outputFile = Path.Combine(@"C:\Users\revecom\Music\",
                    "mix.mp3");

                foreach (var file in files)
                {
                    // for each file -
                    // check if it exists in the temp folder

                    var filePath = file;
                    if (File.Exists(filePath))
                    {
                        // create mp3 reader object
                        var reader = new Mp3FileReader(filePath);

                        // create a wave stream and a channel object
                        var waveStream = WaveFormatConversionStream.CreatePcmStream(reader);
                        var channel = new WaveChannel32(waveStream)
                        {
                            //Set the volume
                            Volume = 0.5f
                        };

                        // add channel as an input stream to the mixer
                        mixer.AddInputStream(channel);
                    }
                }

                // convert wave stream from mixer to mp3
                var wave32 = new Wave32To16Stream(mixer);
                var mp3Writer = new LameMP3FileWriter(outputFile, wave32.WaveFormat, 128);
                wave32.CopyTo(mp3Writer);

                // close all streams
                wave32.Close();
                mp3Writer.Close();

                // return the mashed up file path
                return outputFile;
            }
            catch (Exception)
            {
                // TODO: handle exception
                throw;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] s = { @"C:\Users\revecom\Music\Output.mp32.mp3", @"C:\Users\revecom\Music\Output.mp33.mp3" };
            Console.WriteLine(CreateMashup(s));
        }
    }
}
