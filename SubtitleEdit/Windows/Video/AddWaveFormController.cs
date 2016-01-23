using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using System.Diagnostics;
using VLC;
using System.Threading.Tasks;

namespace Video
{
    public partial class AddWaveFormController : NSWindowController
    {

        public string _sourceFileName { get; private set; }

        private bool _cancel;
        private string _peakWaveFileName;
        //private string _spectrogramDirectory;

        public WavePeakData Peaks { get; private set; }

        public SpectrogramData Spectrogram { get; private set; }

        private string _encodeParamters;
        private const string RetryEncodeParameters = "acodec=s16l";
        private int _audioTrackNumber = -1;
        private int _delayInMilliseconds;
        private string _statusText;
        private string _statusTextLast;
        private bool _done;

        private System.Timers.Timer _progressTimer;
        private long _startTicks;

        public AddWaveFormController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public AddWaveFormController(NSCoder coder)
            : base(coder)
        {
        }

        public AddWaveFormController(string sourceFileName, string peakWaveFileName)
            : base("AddWaveForm")
        {
            _sourceFileName = sourceFileName;
            _peakWaveFileName = WavePeakGenerator.GetPeakWaveFileName(sourceFileName);
            _cancel = false;
            _done = false;
            _startTicks = DateTime.Now.Ticks;
            _statusText = string.Empty;
            _statusTextLast = _statusText;
            (Window as AddWaveForm).SetSourceFile(sourceFileName);
            (Window as AddWaveForm).SetProgressText(_statusText);
            _progressTimer = new System.Timers.Timer(250);
            _progressTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                if (_done || _cancel)
                {
                    _progressTimer.Stop();
                    InvokeOnMainThread(() =>
                        {
                            DoCancel(); 
                            (Window as AddWaveForm).StopProgressBar();
                            System.Threading.Thread.Sleep(100);
                            Close();
                        });
                    return;
                }
               
                _statusText = FormatTime(TimeSpan.FromTicks(DateTime.Now.Ticks - _startTicks).TotalSeconds);
                if (_statusText != _statusTextLast)
                {
                    InvokeOnMainThread(() =>
                        {
                            if (_done || _cancel)
                            {
                                return;
                            }
                            (Window as AddWaveForm).SetProgressText(_statusText);
                        });
                    _statusTextLast = _statusText;
                }
            };
            _progressTimer.Start();
            StartAudioProcessing();
        }

        private static string FormatTime(double seconds)
        {
            if (seconds < 60)
                return string.Format(Configuration.Settings.Language.AddWaveform.ExtractingSeconds, seconds);
            else
                return string.Format(Configuration.Settings.Language.AddWaveform.ExtractingMinutes, (int)(seconds / 60), (int)(seconds % 60));
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new AddWaveForm Window
        {
            get { return (AddWaveForm)base.Window; }
        }

        public static Process GetCommandLineProcess(string inputVideoFile, int audioTrackNumber, string outWaveFile, string encodeParamters, out string encoderName)
        {
            encoderName = "VLC";
            string parameters = "\"" + inputVideoFile + "\" -I dummy -vvv --no-sout-video --audio-track=" + audioTrackNumber + " --sout=\"#transcode{acodec=s16l,channels=1,ab=128}:std{access=file,mux=wav,dst=" + outWaveFile + "}\" vlc://quit";
            string exeFilePath = "/Applications/VLC.app/Contents/MacOS/VLC";

            if (Configuration.Settings.General.UseFFmpegForWaveExtraction && System.IO.File.Exists(Configuration.Settings.General.FFmpegLocation))
            {
                encoderName = "FFmpeg";
                string audioParameter = string.Empty;
                if (audioTrackNumber > 0)
                    audioParameter = string.Format("-map 0:a:{0}", audioTrackNumber);

                const string fFmpegWaveTranscodeSettings = "-i \"{0}\" -vn -ar 24000 -ac 2 -ab 128 -vol 448 -f wav {2} \"{1}\"";
                //-i indicates the input
                //-vn means no video ouput
                //-ar 44100 indicates the sampling frequency.
                //-ab indicates the bit rate (in this example 160kb/s)
                //-vol 448 will boot volume... 256 is normal
                //-ac 2 means 2 channels

                // "-map 0:a:0" is the first audio stream, "-map 0:a:1" is the second audio stream

                exeFilePath = Configuration.Settings.General.FFmpegLocation;
                parameters = string.Format(fFmpegWaveTranscodeSettings, inputVideoFile, outWaveFile, audioParameter);
            }
            return new Process { StartInfo = new ProcessStartInfo(exeFilePath, parameters) { WindowStyle = ProcessWindowStyle.Hidden } };
        }

        private async void StartAudioProcessing()
        {
            await Task.Run(() =>
                {
                    string targetFile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid() + ".wav");
                    ExtractWaveFile(targetFile);   
                    GenerateWaveformAndSpectrogram(targetFile, _delayInMilliseconds);
                    _done = true;
                });
        }

        private void ExtractWaveFile(string targetFile)
        {
            string encoderName;
            Process process;
            try
            {
                process = GetCommandLineProcess(_sourceFileName, _audioTrackNumber, targetFile, _encodeParamters, out encoderName);
                // labelInfo.Text = encoderName;
            }
            catch (DllNotFoundException)
            {
//                if (MessageBox.Show(Configuration.Settings.Language.AddWaveform.VlcMediaPlayerNotFound + Environment.NewLine +
//                    Environment.NewLine + Configuration.Settings.Language.AddWaveform.GoToVlcMediaPlayerHomePage,
//                    Configuration.Settings.Language.AddWaveform.VlcMediaPlayerNotFoundTitle,
//                    MessageBoxButtons.YesNo) == DialogResult.Yes)
//                {
//                    Process.Start("http://www.videolan.org/");
//                }
//                buttonRipWave.Enabled = true;
                return;
            }

            process.Start();
            double seconds = 0;
            try
            {
                process.PriorityClass = ProcessPriorityClass.Normal;
            }
            catch
            {
            }
            while (!process.HasExited)
            {
                System.Threading.Thread.Sleep(100);
                if (_cancel)
                {
                    process.Kill();
//                    progressBar1.Visible = false;
//                    labelPleaseWait.Visible = false;
//                    buttonRipWave.Enabled = true;
//                    buttonCancel.Visible = false;
//                    DialogResult = DialogResult.Cancel;
                    return;
                }
            }
        }

        private void GenerateWaveformAndSpectrogram(string targetFile, double delayInMilliseconds)
        {
            using (var waveFile = new WavePeakGenerator(targetFile))
            {
                // Generate and save peak file
                _statusText = Configuration.Settings.Language.AddWaveform.GeneratingPeakFile;
                Peaks = waveFile.GeneratePeaks((int)Math.Round(delayInMilliseconds), _peakWaveFileName);

                if (Configuration.Settings.VideoControls.GenerateSpectrogram)
                {
                    _statusText = Configuration.Settings.Language.AddWaveform.GeneratingSpectrogram;
//                    Spectrogram = waveFile.GenerateSpectrogram(delayInMilliseconds, _spectrogramDirectory);
                }
            }
        }

        public void DoCancel()
        {            
            _cancel = true;
        }

    }
}
