using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Lame;
using System.IO;
using System.Runtime.InteropServices;
using Istrib.Sound;
using Istrib.Sound.Formats;
using System.Threading;

namespace RJsimpson
{
    public partial class AnyAppBroadcaster : Form
    {
        AudioLevelMonitor audioMonitor;
        LoginForm loginForm;
        public AnyAppBroadcaster(LoginForm loginForm)
        {
            InitializeComponent();
            InitVariables(loginForm);
        }

        private void InitVariables(LoginForm loginForm)
        {
            this.loginForm = loginForm;
            StopBroadCasting = false;
            StopRecPictureBox.Visible = false;
            BroadCastingLabel.Visible = false;
            audioLevelsUIControl1.Visible = false;

            OutputFolder = Path.Combine(Path.GetTempPath(), "RJsimpson");

            Directory.CreateDirectory(OutputFolder);

           // CaptureInstance = new WasapiLoopbackCapture(WasapiLoopbackCapture.GetDefaultLoopbackCaptureDevice());

            MixerLoopParity = false;
            MicLoopParity = false;
            OutputRecorderLoopParity = false;
            BroadcasterRecorderLoopParity = false;

            MixerParity = 0;
            MicRecorderParity = 0;
            OutputRecorderParity = 0;
            BroadcastParity = 0;

            IsBoradcasting = false;
        }

        private void InitVariables()
        {
            StopBroadCasting = false;
            StopRecPictureBox.Visible = false;
            BroadCastingLabel.Visible = false;
            audioLevelsUIControl1.Visible = false;

            OutputFolder = Path.Combine(Path.GetTempPath(), "RJsimpson");

            Directory.CreateDirectory(OutputFolder);

            // CaptureInstance = new WasapiLoopbackCapture(WasapiLoopbackCapture.GetDefaultLoopbackCaptureDevice());

            MixerLoopParity = false;
            MicLoopParity = false;
            OutputRecorderLoopParity = false;
            BroadcasterRecorderLoopParity = false;

            MixerParity = 0;
            MicRecorderParity = 0;
            OutputRecorderParity = 0;
            BroadcastParity = 0;

            IsBoradcasting = false;
        }


        #region GlobalVariables


        public const int RECORD_FILE_FROM_OUTPUT_DEVICE = 0;

        public const int RECORD_FILE_FROM_MIC = 1;

        public const int MIX_OUTPUT_FILE = 2;

        public const int BROADCAST_MIX_OUTPUT_FILE = 2;

        public const int RECORD_FILE_FROM_OUTPUT_DEVICE_WAV = 3;

        public const int RECORD_FILE_FROM_MIC_WAV = 4;

        public const int BUFFER_FILE_COUNT = 15;

        public const int BUFFER_LENGTH = 5000;


        private static bool MixerLoopParity { get; set; }

        private static bool MicLoopParity { get; set; }

        private static bool OutputRecorderLoopParity { get; set; }

        private static bool BroadcasterRecorderLoopParity { get; set; }

        public static int MixerParity { get; set; }

        public static int MicRecorderParity { get; set; }

        public static int OutputRecorderParity { get; set; }

        public static int BroadcastParity { get; set; }

        public static bool IsBoradcasting { get; set; }

        public static bool StopBroadCasting { get; set; }


        private static string OutputFolder { get; set; }

        private static Libshout icecast;

        static byte[] buff = new byte[4096];

        static int read;

        private static SemaphoreSlim RecordingCompletedSignal { get; } = new SemaphoreSlim(0, 1);

        public static string CurrentReadyBroadCast { get; private set; }



        #endregion

        #region importantStuff

        public void SetWaveForm(AppDetails process)
        {
            PlayingIcon.Image = process.IconAsImage;
            SoftwareName.Visible = false;
            audioMonitor = new AudioLevelMonitor(process.processID);
            audioLevelsUIControl1.AudioMonitor = audioMonitor;
            audioLevelsUIControl1.Visible = true;

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
            Application.Exit();


        }

        private void Reduire_Click(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Minimized;
        }

        #endregion

        private void Settings_Click(object sender, EventArgs e)
        {
            Settings setting = new Settings(this);
            setting.Show();
        }

        public void DeviceSelection(SoundCaptureDevice soundCaptureDevice, SoundCaptureDevice micCaptureDevice)
        {
            #region outputDeviceSetup
            mp3SoundCapture.CaptureDevice = soundCaptureDevice;
            mp3SoundCapture.NormalizeVolume = true;
            mp3SoundCapture.OutputType = Mp3SoundCapture.Outputs.Mp3;

            mp3SoundCapture.WaveFormat = PcmSoundFormat.Pcm44kHz16bitMono;
            mp3SoundCapture.Mp3BitRate = Mp3BitRate.BitRate192;
            mp3SoundCapture.WaitOnStop = true;
            InternalMp3SoundCapture = mp3SoundCapture;
            #endregion

            #region micDeviceSetup
            mp3MicCapture.CaptureDevice = micCaptureDevice;
            mp3MicCapture.NormalizeVolume = true;
            mp3MicCapture.OutputType = Mp3SoundCapture.Outputs.Mp3;

            mp3MicCapture.WaveFormat = PcmSoundFormat.Pcm44kHz16bitMono;
            mp3MicCapture.Mp3BitRate = Mp3BitRate.BitRate192;
            mp3MicCapture.WaitOnStop = true;
            InternalMp3MicCapture = mp3MicCapture;
            #endregion
        }

        public static Mp3SoundCapture InternalMp3SoundCapture { get; set; }
        public static Mp3SoundCapture InternalMp3MicCapture { get; set; }


        private void Play_Click(object sender, EventArgs e)
        {
            Thread RecordMicFilesThread = new Thread(RecordMicFiles);
            Thread RecordOutputFilesThread = new Thread(RecordOutputFiles);
            Thread MixingThread = new Thread(MixFiles);
            Thread BroadCastFilesThread = new Thread(BroadCastFiles);
            StopBroadCasting = false;

            RecordMicFilesThread.IsBackground = true;
            RecordOutputFilesThread.IsBackground = true;
            MixingThread.IsBackground = true;
            BroadCastFilesThread.IsBackground = true;

            RecordMicFilesThread.Start();
            RecordOutputFilesThread.Start();
            MixingThread.Start();
            BroadCastFilesThread.Start();
            this.Settings.Click -= Settings_Click;
            this.Settings.Click += Settings_Click_While_Playing;

            
            Play.Visible = false;
            SetStatus();
            StopRecPictureBox.Visible = true;
        }

        private void Settings_Click_While_Playing(object sender, EventArgs e)
        {
             MessageBox.Show("The app is already broadcasting please stop broadcast to enter settings", "Settings error",
                  MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        #endregion

        #region backgroundProcesses

        private static void RecordMicFiles()
        {

            while (!StopBroadCasting)
            {
                string broadcastingFile = FileName(MicRecorderParity, RECORD_FILE_FROM_MIC_WAV);
                Console.WriteLine("entered record mic thread");

                while (!CanReadFile(broadcastingFile))
                {
                    Console.WriteLine(broadcastingFile);
                    Console.WriteLine("can't access file from mic recorder");
                    System.Threading.Thread.Sleep(50);
                }
                #region OLD code
                //StartRecording();
                //Console.WriteLine("Started record mic");

                //System.Threading.Thread.Sleep(BUFFER_LENGTH);
                //Console.WriteLine("End record mic thread");

                //StopRecording();
                //Console.WriteLine("stop record mic thread");
                //System.Threading.Thread.Sleep(50); 
                #endregion


                #region new code


                var configurationForRecording =
                    $"\"{broadcastingFile}\" " +
                    $"{BUFFER_LENGTH}" + " " +
                    $"\"{FileName(MicRecorderParity, RECORD_FILE_FROM_MIC)}\"";

                Console.WriteLine(configurationForRecording);


                Process MicRecProcess = RecordFromMic(configurationForRecording, null);

                Console.WriteLine($"MIC process id = {MicRecProcess.Id}");

                System.Threading.Thread.Sleep(BUFFER_LENGTH);

                Console.WriteLine(configurationForRecording);

                Console.WriteLine("config for rec just ended");

                Console.WriteLine("stop record out thread");
                System.Threading.Thread.Sleep(50);

                FlagManager(RECORD_FILE_FROM_MIC);

                #endregion

            }
        }



        private static void RecordOutputFiles()
        {
            while(!StopBroadCasting)
            {
                Console.WriteLine("entered record out thread");

                string broadcastingFile = FileName(OutputRecorderParity, RECORD_FILE_FROM_OUTPUT_DEVICE_WAV);
                while (!CanReadFile(broadcastingFile))
                {
                    Console.WriteLine(broadcastingFile);
                    Console.WriteLine("can't access file output from recorder");
                    System.Threading.Thread.Sleep(50);
                }

                #region new code


                var configurationForRecording =
                    $"\"{broadcastingFile}\" " +
                    $"{BUFFER_LENGTH}" + " " +
                    $"\"{FileName(OutputRecorderParity, RECORD_FILE_FROM_OUTPUT_DEVICE)}\"";

                Console.WriteLine(configurationForRecording);


                Process SoundCardRecordingProcess = RecordSoundCard(configurationForRecording, null);

                Console.WriteLine($"sound card process id = {SoundCardRecordingProcess.Id}");

                System.Threading.Thread.Sleep(BUFFER_LENGTH);

                Console.WriteLine(configurationForRecording);

                try
                {
                    // RecordingProcess.WaitForExit();
                    //RecordingProcess.Kill();

                }
                catch (Exception e)
                {
                    throw (e);
                }
                Console.WriteLine("config for rec just ended");

                //var configuration =
                //    $"\"{broadcastingFile}\" " +
                //    $"\"{FileName(OutputRecorderParity, RECORD_FILE_FROM_OUTPUT_DEVICE)}\" " +
                //    $"--preset extreme";

                //Console.WriteLine(configuration);


                //CompressingProcess =
                //   CompressFileToMP3(configuration, null);

                //Console.WriteLine(configuration);

                ////CompressingProcess.WaitForExit();
                //Console.WriteLine("compress wait for exit just exited");

                //try
                //{
                //    CompressingProcess.Close();
                //    CompressingProcess.Kill();

                //}
                //catch (Exception e)
                //{

                //}

                Console.WriteLine("stop record out thread");
                System.Threading.Thread.Sleep(50);

                FlagManager(RECORD_FILE_FROM_OUTPUT_DEVICE);
                #endregion

                
            }

        }


        #region newLibrary


        private static Process CompressFileToMP3(string arguments, EventHandler onExecutionCompleted)
        {
            var recordingProc = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    WorkingDirectory = Application.StartupPath,
                    FileName = "lame.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                },
                EnableRaisingEvents = true
            };

            if (onExecutionCompleted != null)
                recordingProc.Exited += onExecutionCompleted;

            recordingProc.Start();
            return recordingProc;
        }

        private static Process RecordSoundCard(string arguments, EventHandler onExecutionCompleted)
        {
            var recordingProc = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    WorkingDirectory = Application.StartupPath,
                    FileName = "SoundCardRecorder.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                },
                EnableRaisingEvents = true
            };

            if (onExecutionCompleted != null)
                recordingProc.Exited += onExecutionCompleted;

            recordingProc.Start();
            return recordingProc;
        }

        private static Process RecordFromMic(string arguments, EventHandler onExecutionCompleted)
        {
            var recordingProc = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    WorkingDirectory = Application.StartupPath,
                    FileName = "MicrophoneRecorder.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                },
                EnableRaisingEvents = true
            };

            if (onExecutionCompleted != null)
                recordingProc.Exited += onExecutionCompleted;

            recordingProc.Start();
            return recordingProc;
        }

        private static Process InitiateMp3StreamProcess(string arguments, EventHandler onExecutionCompleted)
        {
            var recordingProc = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    WorkingDirectory = Application.StartupPath,
                    FileName = "mp3_stream.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                },
                EnableRaisingEvents = true
            };

            if (onExecutionCompleted != null)
                recordingProc.Exited += onExecutionCompleted;

            recordingProc.Start();
            return recordingProc;
        }

        private static Process MixingFilesProcess(string arguments, EventHandler onExecutionCompleted)
        {
            var recordingProc = new Process
            {
                StartInfo =
                {
                    CreateNoWindow = true,
                    WorkingDirectory = Application.StartupPath,
                    FileName = "MixerProcess.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                },
                EnableRaisingEvents = true
            };

            if (onExecutionCompleted != null)
                recordingProc.Exited += onExecutionCompleted;

            recordingProc.Start();
            return recordingProc;
        }


        private static string Execute(string command)
        {
            var process = InitiateMp3StreamProcess(command, null);
            process.WaitForExit();
            string stringList = process.StandardOutput.ReadLine();
            return stringList;
        }

        private static IEnumerable<string> ReadResponseLines(StreamReader process)
        {
            string line = "";
            while ((line = process.ReadLine()) != null)
            {
                yield return line;
            }
        }

        private static Process RecordingProcess { get; set; }

        private static Process CompressingProcess { get; set; }

        private static Process SoundCardRecordingProcess { get; set; }


        private static void StartRecording()
        {
            var lines = Execute($"-device=\"{InternalMp3MicCapture.CaptureDevice.Description.Substring(0, 31)}\"");
            string fileDestination = FileName(MicRecorderParity, RECORD_FILE_FROM_MIC);

            string line = lines;
            var configuration =
                $"-device=\"{InternalMp3MicCapture.CaptureDevice.Description.Substring(0, 31)}\" " +
                $"-line=\"{line}\" " +
                $"-v=100 " +
                $"-br=192 -sr=44100 -trg=\"{fileDestination}\" -tm={BUFFER_LENGTH}";

            RecordingProcess =
                InitiateMp3StreamProcess(configuration, null);


        }

        private static void StopRecording()
        {
            RecordingProcess.WaitForExit();


            RecordingProcess.Close();
            //SaveRecording();

        }


        private static bool isMixingStarted = false;
        #endregion
        private static void MixFiles()
        {
            while (!StopBroadCasting)
            {
                if (!isMixingStarted)
                {
                    Console.WriteLine("entered mix thread and sleap");
                    System.Threading.Thread.Sleep(3 * BUFFER_LENGTH);
                    isMixingStarted = true;

                }
                Console.WriteLine("entered mix thread and awake");
                if (((MixerParity >= OutputRecorderParity-1) && (OutputRecorderLoopParity == MixerLoopParity)) || ((MixerParity >= MicRecorderParity-1)) && (MixerLoopParity == MicLoopParity))
                {
                    System.Threading.Thread.Sleep(50);
                    Console.WriteLine($"something is worng with the mixer mixerparity: {MixerParity}" +
                        $"MixerLoopParity :  {MixerLoopParity} " +
                        $"OutputRecorderParity : {OutputRecorderParity} " +
                        $"MicRecorderParity  :  {MicRecorderParity}");
                    MixFiles();
                }
                else
                {

                    string file1 = FileName(MixerParity, RECORD_FILE_FROM_OUTPUT_DEVICE);
                    string file2 = FileName(MixerParity, RECORD_FILE_FROM_MIC);
                    string outputFile = FileName(MixerParity, MIX_OUTPUT_FILE);
                    while (!CanReadFile(file1) || !CanReadFile(file2) || !CanReadFile(outputFile))
                    {
                        Console.WriteLine(file1);
                        Console.WriteLine("can't access file output from recorder");
                        System.Threading.Thread.Sleep(50);
                    }
                    string[] files = { file1, file2 };
                    //CreateMashup(files, outputFile);
                    var configuration =
                    $"\"{file1}\" " +
                    $"\"{file2}\" " +
                    $"\"{outputFile}\"";

                    Process mixingProcess =
                        MixingFilesProcess(configuration, null);

                    Console.WriteLine(" mixing done thread and going out");

                    Console.WriteLine($"MixerParity = {MixerParity}");
                    System.Threading.Thread.Sleep(BUFFER_LENGTH);

                    FlagManager(RECORD_FILE_FROM_OUTPUT_DEVICE_WAV);

                }

            }
            
        }

        private static void BroadCastFiles()
        {
            while (!StopBroadCasting)
            {
                
                Console.WriteLine("entered broadcasting");

                if (!IsBoradcasting)
                {
                    Console.WriteLine("entered broadcasting sleep mode");

                    System.Threading.Thread.Sleep(4 * BUFFER_LENGTH);

                }
                bool broadcastingTooFast = false;
                while ((BroadcasterRecorderLoopParity == MixerLoopParity) && (BroadcastParity >= MixerParity))
                {
                    if (!broadcastingTooFast)
                    {
                        Console.WriteLine("broadcasting too fast");
                        broadcastingTooFast = !broadcastingTooFast;
                    }
                    System.Threading.Thread.Sleep(50);

                }
                Console.WriteLine("entered broadcasting not too fast");

                string currentReadyBroadCast = FileName(BroadcastParity, BROADCAST_MIX_OUTPUT_FILE);

                while (!CanReadFile(currentReadyBroadCast))
                {
                    Console.WriteLine(currentReadyBroadCast);
                    Console.WriteLine("can't access file from broadcaster");
                    System.Threading.Thread.Sleep(50);
                }

                if (!IsBoradcasting)
                {
                    ConnectToIceCast();
                    if (icecast.isConnected())
                    { //create method to set these labels outside of this method
                        IsBoradcasting = true;
                        Console.WriteLine("Connected broadcasting");
                        MessageBox.Show("Broadcasting Started", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    else
                    {
                        MessageBox.Show(icecast.GetError(), "IceCast Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
                    using (var file = File.Open(currentReadyBroadCast, FileMode.Open))
                    {
                        Console.WriteLine("started broadcasting");

                        BinaryReader reader = new BinaryReader(file);
                        int total = 0;
                        while (true)
                        {
                            read = reader.Read(buff, 0, buff.Length);
                            total = total + read;

                            Console.WriteLine("Position:  " + reader.BaseStream.Position);
                            if (read > 0)
                            {
                                try
                                {
                                    icecast.send(buff, read);
                                }
                                catch (IOException)
                                {
                                    IsBoradcasting = false;
                                    MessageBox.Show(icecast.GetError(), "IceCast Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                                break;
                                }
                            }
                            else break;

                        }
                        Console.WriteLine(BroadcastParity);
                        FlagManager(BROADCAST_MIX_OUTPUT_FILE);
                        Console.WriteLine("Done!");
                    }
                }
            }

        

        #endregion

        private void SetStatus()
        {

            label1.Visible = false;
            BroadCastingLabel.Visible = true;
        }

        private static string FileName(int fileNumber, int fileType)
        {
            string broadcastingFile;

            if (fileType == RECORD_FILE_FROM_OUTPUT_DEVICE)
                broadcastingFile = Path.Combine(OutputFolder, $"RecordFromOutput {fileNumber}.mp3");
            else if (fileType == RECORD_FILE_FROM_MIC)
                broadcastingFile = Path.Combine(OutputFolder, $"RecordFromMic {fileNumber}.mp3");
            else if (fileType == BROADCAST_MIX_OUTPUT_FILE)
                broadcastingFile = Path.Combine(OutputFolder, $"MixOutput {fileNumber}.mp3");
            else if (fileType == RECORD_FILE_FROM_OUTPUT_DEVICE_WAV)
                broadcastingFile = Path.Combine(OutputFolder, $"RecordFromOutput {fileNumber}.wav");
            else if (fileType == RECORD_FILE_FROM_MIC_WAV)
                broadcastingFile = Path.Combine(OutputFolder, $"RecordFromMic {fileNumber}.wav");
            else return null;
            return broadcastingFile;
        }

        private static void FlagManager(int caller)
        {
            if (caller == BROADCAST_MIX_OUTPUT_FILE)
            {
                BroadcasterRecorderLoopParity = LoopManager(BroadcastParity, BroadcasterRecorderLoopParity);
                BroadcastParity = FileNumberManager(BroadcastParity);
            }
            else if (caller == RECORD_FILE_FROM_OUTPUT_DEVICE_WAV)
            {
                MixerLoopParity = LoopManager(MixerParity, MixerLoopParity);
                MixerParity = FileNumberManager(MixerParity);
            }
            else if (caller == RECORD_FILE_FROM_MIC)
            {
                MicLoopParity = LoopManager(MicRecorderParity, MicLoopParity);
                MicRecorderParity = FileNumberManager(MicRecorderParity);
            }
            else if (caller == RECORD_FILE_FROM_OUTPUT_DEVICE)
            {
                OutputRecorderLoopParity = LoopManager(OutputRecorderParity, OutputRecorderLoopParity);
                OutputRecorderParity = FileNumberManager(OutputRecorderParity);
            }
        }

        private static bool LoopManager(int number, bool loopState)
        {
            if (number == BUFFER_FILE_COUNT)
            {
                return !loopState;
            }
            else
            {
                number++;

                return loopState;
            }
        }

        private static int FileNumberManager(int number)
        {
            if (number == BUFFER_FILE_COUNT)
            {
                return 0;
            }
            else
            {
                number++;

                return number;
            }
        }

        /// <summary>
        /// Creates a mashup of two or more mp3 files by using naudio
        /// </summary>
        /// <param name="files">Name of files as an string array</param>
        /// These files should be existing in a temporay folder
        /// <returns>The path of mashed up mp3 file</returns>
        /// 
       

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

        }

        private void StopRecPictureBox_Click(object sender, EventArgs e)
        {
            StopBroadCasting = true;
            Play.Visible = true;
            InitVariables();
            label1.Visible = true;
            BroadCastingLabel.Visible = false;
            

            this.Settings.Click -= Settings_Click_While_Playing;

            this.Settings.Click += Settings_Click;
            StopRecPictureBox.Visible = false;
        }
    }
}
/* TODO: From the MainForm What needs to be done is the following:
 * 
 * 
 * 
 * I neeed to import all of the methods that are to record the from a device
 * and then within those methods I should add the new device mp3MicCapture
 * and record from both streams simultainusly
 * I should then deligate this method to a thread by itself or try making both recorders each on a different thread
 * third I should make a thread that keeps watch on the files that are being written once both files have been written
 * I should moove on to the mixing part
 * I should creat a flagging systme
 * one flag for file being recorded 
 * the nex files being mixed
 * once all is ready forward it to the streamer that should just take on himself the mixed files 
 * and pushes them to the icecast servers while raising a flag that says this file is beign broadcasted
 * the flags should be on a 3 way matrix (maybe this needs further thought) order should be kept between threads
 * so they should have a commen matrix that follows all the flags
 * the main form should have a stop button for stop recording
 * and then I should handle most of the exceptions by clicking on the stop record button then raising a 
 * box that will say Hey you have a problem somewhere contact the support team for any further inquiries
 * 
 * ///////// do not forget the starting stopped stopping methods from the device sound capturers
 */
