//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
//
//  Email:  lukasz@istrib.org
//
//  Copyright (C) 2008-2009 Lukasz Kwiecinski. 
//
//  LAME ( LAME Ain't an Mp3 Encoder ) 
//  You must call the fucntion "beVersion" to obtain information  like version 
//  numbers (both of the DLL and encoding engine), release date and URL for 
//  lame_enc's homepage. All this information should be made available to the 
//  user of your product through a dialog box or something similar.
//  You must see all information about LAME project and legal license infos at 
//  http://www.mp3dev.org/  The official LAME site
//
//
//  About Thomson and/or Fraunhofer patents:
//  Any use of this product does not convey a license under the relevant 
//  intellectual property of Thomson and/or Fraunhofer Gesellschaft nor imply 
//  any right to use this product in any finished end user or ready-to-use final 
//  product. An independent license for such use is required. 
//  For details, please visit http://www.mp3licensing.com.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Istrib.Sound.Formats;
using System.IO;
using System.Threading;
using Un4seen.Bass.Misc;
using Un4seen.Bass;

namespace Istrib.Sound.Example.WinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            isbroadcasting = false;
        }

        string broadcastingFile;
        private void Form1_Load(object sender, EventArgs e)
        {
            fileTxt.Text = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "Output");
            broadcastingFile = fileTxt.Text + ".mp3";
            LoadAvailableFormats(PcmSoundFormat.StandardFormats);

            foreach (SoundCaptureDevice device in SoundCaptureDevice.AllAvailable)
            {
                devicesCmb.Items.Add(device);
            }

            devicesCmb.SelectedIndex = 0;

            mp3Rd_CheckedChanged(sender, e);
            currentReadyBroadCast = broadcastingFile;

        }

        private void LoadAvailableFormats(IEnumerable<PcmSoundFormat> formats)
        {
            formatsCmb.Items.Clear();

            foreach (PcmSoundFormat format in formats)
            {
                formatsCmb.Items.Add(format);
            }

            formatsCmb.SelectedIndex = 0;
        }

        private void LoadAvailableBitRates()
        {
            bitRateCmb.Items.Clear();

            foreach (Mp3BitRate bitRate in ((PcmSoundFormat)formatsCmb.SelectedItem).GetCompatibleMp3BitRates())
            {
                bitRateCmb.Items.Add(bitRate);
            }

            if (bitRateCmb.Items.Count > 0)
            {
                bitRateCmb.SelectedIndex = 0;
            }
        }

        public static bool isbroadcasting { get; private set; }


        private void startBtn_Click(object sender, EventArgs e)
        {
            try
            {
                mp3SoundCapture.CaptureDevice = (SoundCaptureDevice)devicesCmb.SelectedItem;
                mp3SoundCapture.NormalizeVolume = normalizeChk.Checked;
                if (rawPcmRd.Checked)
                {
                    mp3SoundCapture.OutputType = Mp3SoundCapture.Outputs.RawPcm;
                }
                else if (mp3Rd.Checked)
                {
                    mp3SoundCapture.OutputType = Mp3SoundCapture.Outputs.Mp3;
                }
                else
                {
                    mp3SoundCapture.OutputType = Mp3SoundCapture.Outputs.Wav;
                }

                mp3SoundCapture.WaveFormat = (PcmSoundFormat)formatsCmb.SelectedItem;
                mp3SoundCapture.Mp3BitRate = (Mp3BitRate)bitRateCmb.SelectedItem;
                mp3SoundCapture.WaitOnStop = !asyncStopChk.Checked;
                while (!CanReadFile(broadcastingFile))
                {
                    Console.WriteLine(broadcastingFile);
                    Console.WriteLine("can't access file from recorder");
                    System.Threading.Thread.Sleep(50);
                }
                mp3SoundCapture.Start(broadcastingFile);
                if (!isbroadcasting)
                {
                    isbroadcasting = true;
                    Thread T1 = new Thread(BroadcastThisFile);
                    T1.IsBackground = true;
                    T1.Start();
                }

                System.Threading.Thread.Sleep(30000);
                nextFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Not so easy...", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private static Libshout icecast;
        private static bool ConnectToIceCast()
        {
            icecast = new Libshout();
            icecast.setProtocol(0);
            icecast.setHost("fluoz.radiojar.com");
            icecast.setPort(80);
            icecast.setPassword("cWZz2qGG");
            icecast.setFormat(Libshout.FORMAT_MP3);
            icecast.setPublic(true);
            icecast.setName("source");
            icecast.setMount("6w5bwaennfeuv/source");
            icecast.open();
            return icecast.isConnected();

            //lame = new EncoderLAME(65541)
            //{

            //    LAME_Bitrate = (int)EncoderLAME.BITRATE.kbps_192,
            //    LAME_Mode = EncoderLAME.LAMEMode.Stereo,
            //    LAME_TargetSampleRate = (int)EncoderLAME.SAMPLERATE.Hz_44100,
            //    LAME_Quality = EncoderLAME.LAMEQuality.Quality
            //};
            //lame.ChannelInfo.ctype = BASSChannelType.BASS_CTYPE_STREAM_MP3;
            //iCEcast = new ICEcast(lame);
            //iCEcast.ServerAddress = "fluoz.radiojar.com";
            //iCEcast.ServerPort = 80;
            //iCEcast.Password = "cWZz2qGG";
            //iCEcast.MountPoint = "6w5bwaennfeuv/source";
            //iCEcast.PublicFlag = true;
            //iCEcast.Username = "source";
            //return iCEcast.Connect();


        }

        static byte[] buff = new byte[4096];
        static int read;
        static bool loopParity = false;
        private void nextFile()
        {

            Console.WriteLine("broadcasting stopped");
            mp3SoundCapture.Stop();
            Parity++;
            if (Parity == 0)
                broadcastingFile = fileTxt.Text + ".mp3";
            else if (Parity == 1)
                broadcastingFile = fileTxt.Text + "1.mp3";
            else if (Parity == 2)
                broadcastingFile = fileTxt.Text + "2.mp3";
            else if (Parity == 3)
                broadcastingFile = fileTxt.Text + "3.mp3";
            else if (Parity == 4)
                broadcastingFile = fileTxt.Text + "4.mp3";
            else if (Parity == 5)
                broadcastingFile = fileTxt.Text + "5.mp3";
            else if (Parity == 6)
                broadcastingFile = fileTxt.Text + "6.mp3";
            else if (Parity == 7)
                broadcastingFile = fileTxt.Text + "7.mp3";
            else if (Parity == 8)
                broadcastingFile = fileTxt.Text + "8.mp3";
            else if (Parity == 9)
                broadcastingFile = fileTxt.Text + "9.mp3";
            else if (Parity == 10)
                broadcastingFile = fileTxt.Text + "10.mp3";
            else if (Parity == 11)
                broadcastingFile = fileTxt.Text + "11.mp3";
            else
            {
                broadcastingFile = fileTxt.Text + "12.mp3";
                Parity = 0;
                loopParity = !loopParity;
            }
            Console.WriteLine($"recording this file {Parity}");

            System.Threading.Thread.Sleep(50);

            startBtn_Click(null, null);
        }
        private void stopBtn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("broadcasting stopped");
            mp3SoundCapture.Stop();
            isbroadcasting = false;

        }

        internal static bool CanReadFile(string filePath)
        {
            //Try-Catch so we dont crash the program and can check the exception
            try
            {
                //The "using" is important because FileStream implements IDisposable and
                //"using" will avoid a heap exhaustion situation when too many handles  
                //are left undisposed.
                using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    if (fileStream != null) fileStream.Close();  //This line is me being overly cautious, fileStream will never be null unless an exception occurs... and I know the "using" does it but its helpful to be explicit - especially when we encounter errors - at least for me anyway!
                }
            }
            catch (Exception)
            {
                return false;

            }
            finally
            { }
            return true;
        }

        private static void BroadcastThisFile()
        {
            Console.WriteLine("entered Broadcaster");
            int parity = 0;
            Console.Write("The is broadcasting value is : ");
            Console.WriteLine(isbroadcasting);
            Console.WriteLine("I am sleeping");
            System.Threading.Thread.Sleep(60000);

            Console.WriteLine("I Woke Up ched ja3bek");

            bool connected = ConnectToIceCast();
            if (connected)
            {
                Console.WriteLine("Connected to IceCast()");
            }
            //string error = iCEcast.LastErrorMessage;
            //string error2 = iCEcast.LastError.ToString();
            //Console.WriteLine(error);
   
            while (isbroadcasting)
            {

                currentReadyBroadCast = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "Output");
                if (parity == 0)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp3";
                else if (parity == 1)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp31.mp3";
                else if (parity == 2)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp32.mp3";
                else if (parity == 3)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp33.mp3";
                else if (parity == 4)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp34.mp3";
                else if (parity == 5)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp35.mp3";
                else if (parity == 6)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp36.mp3";
                else if (parity == 7)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp37.mp3";
                else if (parity == 8)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp38.mp3";
                else if (parity == 9)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp39.mp3";
                else if (parity == 10)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp310.mp3";
                else if (parity == 11)
                    currentReadyBroadCast = currentReadyBroadCast + ".mp311.mp3";
                else
                {
                    currentReadyBroadCast = currentReadyBroadCast + ".mp312.mp3";
                    parity = 0;
                    loopParity = !loopParity;
                }
                Console.WriteLine(currentReadyBroadCast);

                while ((Parity <= parity) && (!loopParity))
                {
                    Console.WriteLine("broadcasting too fast");
                    System.Threading.Thread.Sleep(50);

                }
                while (!CanReadFile(currentReadyBroadCast))
                {
                    Console.WriteLine(currentReadyBroadCast);
                    Console.WriteLine("can't access file from broadcaster");
                    System.Threading.Thread.Sleep(50);
                }


                using (var file = File.Open(currentReadyBroadCast, FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(file);
                    int total = 0;
                    while (true)
                    {
                        //читаем буфер
                        read = reader.Read(buff, 0, buff.Length);
                        total = total + read;

                        Console.WriteLine("Position:  " + reader.BaseStream.Position);
                        //если прочитан не весь, то передаем
                        if (read > 0)
                        {
                            try
                            {
                                icecast.send(buff, read);
                            }
                            catch (IOException e)
                            {

                                ConnectToIceCast();
                            }   
                        }
                        else break;  

                    }
                    parity++;

                    Console.WriteLine("Done!");
                }

            }



        }

        public static int Parity { get; private set; }

        public static string currentReadyBroadCast { get; private set; }

        private void mp3Rd_CheckedChanged(object sender, EventArgs e)
        {
            if (mp3Rd.Checked)
            {
                LoadAvailableFormats(Mp3SoundFormat.AllSourceFormats);
                formatsCmb.SelectedItem = PcmSoundFormat.Pcm44kHz16bitStereo;
                bitRateCmb.Enabled = true;
                bitRateCmb.SelectedItem = Mp3BitRate.BitRate192;

                fileTxt.Text = Path.Combine(Path.GetDirectoryName(fileTxt.Text),
                    Path.GetFileNameWithoutExtension(fileTxt.Text) + ".mp3");
            }
            else
            {
                LoadAvailableFormats(PcmSoundFormat.StandardFormats);
                fileTxt.Text = Path.Combine(Path.GetDirectoryName(fileTxt.Text),
                    Path.GetFileNameWithoutExtension(fileTxt.Text) + ".wav");
                bitRateCmb.Enabled = false;
            }
        }

        private void mp3SoundCapture_Starting(object sender, EventArgs e)
        {
            startBtn.Enabled = !(stopBtn.Enabled = true);
            dataAvailableLbl.Visible = false;
        }

        private void mp3SoundCapture_Stopping(object sender, EventArgs e)
        {
            startBtn.Enabled = !(stopBtn.Enabled = false);
        }

        private void mp3SoundCapture_Stopped(object sender, Mp3SoundCapture.StoppedEventArgs e)
        {
            dataAvailableLbl.Text = "Data available in " + e.OutputFileName;
            dataAvailableLbl.Visible = true;
        }

        private void browseBtn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            saveFileDialog.FileName = fileTxt.Text;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileTxt.Text = saveFileDialog.FileName;
            }
        }

        private void formatsCmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAvailableBitRates();
        }
    }
}
