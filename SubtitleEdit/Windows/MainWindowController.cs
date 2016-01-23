using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using System.IO;
using System.Text;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using Nikse.SubtitleEdit.UILogic;
using System.Linq;
using Tools;
using Sync;
using System.Collections.Generic;
using Edit;
using UILogic;
using VideoPlayer;
using CoreGraphics;
using AVFoundationPlayer;
using VLC;
using SubtitleEdit;
using Video;

namespace Nikse.SubtitleEdit.Windows
{
    public partial class MainWindowController : NSWindowController, IAdjustAction, IOpenSubtitle, IChangeSubtitleTableSelection, ISubtitleTextChanged
    {

        private Subtitle _subtitle;
        private LanguageStructure.Main _language;
        private LanguageStructure.General _languageGeneral;
        private string _subtitleFileName;
        private string _videoFileName;
        private Paragraph _selectedParagraph;
        private string _subtitleOriginalHash;
        private string _subtitleOrginalFormat;
        private System.Timers.Timer _autoBackupTimer;
        private string _autobackupLastHash = string.Empty;
        private static SubRip _autoBackupFormat = new SubRip();
        private FindReplaceInfo _findReplaceHelper = new FindReplaceInfo();
        private IVideoPlayer _videoPlayer;
        private System.Timers.Timer _subtitleDisplayTimer;
        private double _subtitleDisplayTimerLastPosition = -1;
        private string _subtitleDisplayTimerLastText = null;
        private System.Timers.Timer _timerWaveform;
        private AudioVisualizer _audioVisualizer;

        public MainWindowController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public MainWindowController(NSCoder coder)
            : base(coder)
        {
        }

        public MainWindowController()
            : base("MainWindow")
        {
        }

        private void SetupAudioVisualizer()
        {
            _audioVisualizer = new AudioVisualizer(Window.AudioVisualizerView);
            _timerWaveform = new System.Timers.Timer(50);
            _timerWaveform.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                InvokeOnMainThread(() =>
                    {
                        _timerWaveform.Stop();
                        System.Threading.Thread.Sleep(100);
                        if (_videoFileName != null)
                        {
                            int index = -1;
                            var selectedIndices = Window.SubtitleTable.SelectedRows.ToList();
                            if (selectedIndices.Count > 0)
                                index = (int)selectedIndices.First();

                            if (_audioVisualizer.Locked)
                            {
                                double startPos = _videoPlayer.Position - ((_audioVisualizer.EndPositionSeconds - _audioVisualizer.StartPositionSeconds) / 2.0);
                                if (startPos < 0)
                                    startPos = 0;
                                SetWaveformPosition(startPos, _videoPlayer.Position, index);
                            }
//                                else if (_videoPlayer.Position > _audioVisualizer.EndPositionSeconds || _videoPlayer.Position < _audioVisualizer.StartPositionSeconds)
//                                {
//                                    double startPos = _videoPlayer.Position - 0.01;
//                                    if (startPos < 0)
//                                        startPos = 0;
//                                    _audioVisualizer.ClearSelection();
//                                    SetWaveformPosition(startPos, _videoPlayer.Position, index);
//                                }
                                else
                            {
                                SetWaveformPosition(_videoPlayer.Position, index);
                            }
                            _audioVisualizer.AudioVisualizerPaint();
                        }
                        _timerWaveform.Start();
                    });
            };
            _audioVisualizer.OnTimeChanged += (object sender, AudioVisualizerBase.ParagraphEventArgs e) =>
            {
                InvokeOnMainThread(() =>
                    {
                        ReloadDataKeepSelection();
                    });
            };

            _audioVisualizer.OnPositionSelected += (object sender, AudioVisualizerBase.ParagraphEventArgs e) =>
            {
                if (string.IsNullOrEmpty(_videoFileName) || e.Paragraph == null)
                {
                    return;
                }
                int index = _subtitle.GetIndex(e.Paragraph);
                if (index < 0)
                {
                    return;
                }
                InvokeOnMainThread(() =>
                    {
                        ShowSubtitleRow(index);
                        _videoPlayer.Position = _selectedParagraph.StartTime.TotalSeconds + 0.1;
                    });                    
            };

            _audioVisualizer.OnDoubleClickNonParagraph += (object sender, AudioVisualizerBase.ParagraphEventArgs e) =>
            {
                if (string.IsNullOrEmpty(_videoFileName))
                {
                    return;
                }
                int index = _subtitle.GetIndex(e.Seconds);
                InvokeOnMainThread(() =>
                    {
                        if (index >= 0)
                        {
                                
                            ShowSubtitleRow(index);
                        }
                        _videoPlayer.Position = e.Seconds;
                    });     
            };
        }

        private void SetupAutoBackup()
        {
            if (_autoBackupTimer != null)
            {
                try
                {
                    _autoBackupTimer.Stop();
                    _autoBackupTimer.Dispose();
                }
                catch
                {
                }
            }
            if (Configuration.Settings.General.AutoBackupSeconds == 0)
            {
                return;
            }
            _autoBackupTimer = new System.Timers.Timer(10000); //Configuration.Settings.General.AutoBackupSeconds * 1000);
            _autoBackupTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                InvokeOnMainThread(() =>
                    {
                        var sub = Window.Subtitle;
                        var hash = sub.GetFastHashCode();
                        if (sub.Paragraphs.Count == 0 || hash == _subtitleOriginalHash || hash == _autobackupLastHash || !Window.IsKeyWindow)
                        {
                            return;
                        }
                        if (!Directory.Exists(Configuration.AutoBackupFolder))
                        {
                            try
                            {
                                Directory.CreateDirectory(Configuration.AutoBackupFolder);
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show(string.Format(_language.UnableToCreateBackupDirectory, Configuration.AutoBackupFolder, exception.Message));
                            }
                        }
                        string title = string.Empty;
                        if (!string.IsNullOrEmpty(_subtitleFileName))
                            title = "_" + Path.GetFileNameWithoutExtension(_subtitleFileName);
                        string fileName = string.Format("{0}{1:0000}-{2:00}-{3:00}_{4:00}-{5:00}-{6:00}{7}{8}", Configuration.AutoBackupFolder, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, title, GetCurrentSubtitleFormat().Extension);
                        System.IO.File.WriteAllText(fileName, _autoBackupFormat.ToText(sub, string.Empty));
                        _autobackupLastHash = hash;
                    });
            };

            _autoBackupTimer.Start();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            _language = Configuration.Settings.Language.Main;
            _languageGeneral = Configuration.Settings.Language.General;
            SetupAudioVisualizer();
            foreach (var item in Configuration.Settings.RecentFiles.Files)
            {
                if (System.IO.File.Exists(item.FileName))
                {
                    OpenSubtitle(item.FileName, false, null);
                    break;
                }    
            }
            UpdateRecentFiles();
            SetupAutoBackup();
        }

        public new MainWindow Window
        {
            get { return (MainWindow)base.Window; }
        }

        public void DoClose()
        {
            Configuration.Settings.Save();
            _videoPlayer.DisposeVideoPlayer();
        }

        private void SetSubtitleFormat(SubtitleFormat format)
        {
            foreach (var value in Window.SubtitleFormats.Values)
            {
                if (value.ToString() == format.FriendlyName)
                {
                    Window.SubtitleFormats.Select(value);
                    return;
                }
            }
        }

        private void SetNewSubtitle(Subtitle subtitle)
        {
            _selectedParagraph = null;
            var ds = new SubtitleTableDataSource(subtitle, this);
            Window.SubtitleTable.DataSource = ds;
            Window.SubtitleTable.Delegate = new SubtitleTableDelegate(ds, this);
            ShowSubtitleRow(0);
            _subtitleOriginalHash = subtitle.GetFastHashCode();
            _subtitleOrginalFormat = GetCurrentSubtitleFormat().FriendlyName;
            SetTitle();
        }

        private void ReloadSubtitle(Subtitle subtitle, bool keepSelected)
        {
            var ds = new SubtitleTableDataSource(subtitle, this);
            Window.SubtitleTable.DataSource = ds;
            Window.SubtitleTable.Delegate = new SubtitleTableDelegate(ds, this);

            var selectedIndices = Window.SubtitleTable.SelectedRows;
            Window.SubtitleTable.ReloadData();
            if (keepSelected)
            {
                foreach (var index in selectedIndices)
                {
                    Window.SubtitleTable.SelectRow((nint)index, true);
                }
            }
            else
            {
                ShowSubtitleRow(0);
            }
        }

        public void NewSubtitle()
        {
            if (Window.SubtitleTable.DataSource == null || ContinueIfChanged())
            {
                _subtitleFileName = null;
                SetNewSubtitle(new Subtitle());
                Window.SetEncoding(Encoding.UTF8.BodyName);
                Window.SubtitleText.StringValue = string.Empty;
                Window.SetTimeCode(new Paragraph());
            }
        }

        public void Renumber()
        {
            if (Window.Subtitle == null || Window.Subtitle.Paragraphs.Count == 0)
                return;

            using (var controller = new RenumberController())
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
                if (controller.WasOkPressed)
                {
                    Window.Subtitle.Renumber(controller.StartNumber);
                    ReloadDataKeepSelection();
                }
            }    
        }

        public void AdjustAllTimes()
        {
            if (Window.Subtitle == null || Window.Subtitle.Paragraphs.Count == 0)
                return;

            using (var controller = new AdjustAllTimesController(this)) // this implements call back interface
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
            }    
        }

        public void ChangeFrameRate()
        {
            if (Window.Subtitle == null || Window.Subtitle.Paragraphs.Count == 0)
                return;

            using (var controller = new ChangeFrameRateController())
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
                if (controller.WasOkPressed)
                {
                    Window.Subtitle.ChangeFrameRate(controller.FromFrameRate, controller.ToFrameRate);
                    ReloadDataKeepSelection();
                }
            }    
        }

        public void DeleteLines()
        {
            var selectedRows = Window.SubtitleTable.SelectedRows;
            if (selectedRows.Count == 0)
                return;

            if (Configuration.Settings.General.PromptDeleteLines)
            {
                NSAlert alert = new NSAlert();
                var count = selectedRows.Count;
                alert.MessageText = count == 1 ? _language.DeleteOneLinePrompt : string.Format(_language.DeleteXLinesPrompt, count);
                alert.AddButton(_languageGeneral.Ok.Replace("&", string.Empty));
                alert.AddButton(_languageGeneral.Cancel.Replace("&", string.Empty));
                var result = alert.RunModal();
                if (result != (long)(NSAlertButtonReturn.First))
                {
                    return;
                }
            }

            foreach (var row in selectedRows.Reverse())
            {
                Window.Subtitle.Paragraphs.RemoveAt((int)row);
            }
            Window.Subtitle.Renumber();
            Window.SubtitleTable.ReloadData();
            ShowSubtitleRow((nint)selectedRows.First());
        }

        public void ReloadDataKeepSelection()
        {
            var selectedIndices = Window.SubtitleTable.SelectedRows;
            Window.SubtitleTable.ReloadData();
            foreach (var index in selectedIndices)
            {
                Window.SubtitleTable.SelectRow((nint)index, true);
            }
        }

        public nint ShowSubtitleRow(nint rowNumber)
        {
            var row = Window.ShowSubtitleRow(rowNumber);
            _selectedParagraph = Window.Subtitle.GetParagraphOrDefault((int)row);
            return row;
        }

        public void InsertFirstLine()
        {
            var p = new Paragraph(string.Empty, 1000, Configuration.Settings.General.NewEmptyDefaultMs);
            Window.Subtitle.Paragraphs.Add(p);
            Window.Subtitle.Renumber();
            Window.SubtitleTable.ReloadData();
            ShowSubtitleRow(0);
            Window.SubtitleText.BecomeFirstResponder();
        }

        void InsertAndShow(Paragraph p)
        {
            Window.Subtitle.InsertParagraphInCorrectTimeOrder(p);
            Window.Subtitle.Renumber();
            Window.SubtitleTable.ReloadData();
            Window.ShowSubtitleRow(Window.Subtitle.Paragraphs.IndexOf(p));
            Window.SubtitleText.BecomeFirstResponder();
        }

        public void InsertBefore()
        {
            if (_selectedParagraph == null)
                return;

            var endtMs = _selectedParagraph.StartTime.TotalMilliseconds - Configuration.Settings.General.MinimumMillisecondsBetweenLines;
            var p = new Paragraph(string.Empty, endtMs - Configuration.Settings.General.NewEmptyDefaultMs, endtMs);
            InsertAndShow(p);
        }

        public void InsertAfter()
        {
            if (_selectedParagraph == null)
                return;

            var startMs = _selectedParagraph.EndTime.TotalMilliseconds + Configuration.Settings.General.MinimumMillisecondsBetweenLines;
            var p = new Paragraph(string.Empty, startMs, startMs + Configuration.Settings.General.NewEmptyDefaultMs);
            InsertAndShow(p);
        }

        public void Split()
        {
            if (_selectedParagraph == null)
                return;

            var selectedIndices = Window.SubtitleTable.SelectedRows;
            if (selectedIndices.Count != 1)
                return;

            var duration = _selectedParagraph.Duration.TotalMilliseconds; 
            var oldEndTime = _selectedParagraph.EndTime.TotalMilliseconds;
            _selectedParagraph.EndTime.TotalMilliseconds = _selectedParagraph.StartTime.TotalMilliseconds + ((duration - Configuration.Settings.General.MinimumMillisecondsBetweenLines) / 2);
            var startMs = _selectedParagraph.EndTime.TotalMilliseconds + Configuration.Settings.General.MinimumMillisecondsBetweenLines;
            var p = new Paragraph(string.Empty, startMs, oldEndTime);
            var arr = Utilities.AutoBreakLine(_selectedParagraph.Text).SplitToLines();
            if (arr.Length == 2)
            {
                if (arr[0].StartsWith("<i>", StringComparison.Ordinal) && arr[1].EndsWith("</i>", StringComparison.Ordinal) &&
                    Utilities.CountTagInText(arr[0], "<i>") == 1)
                {
                    arr[0] += "</i>";
                    arr[1] = "<i>" + arr[1];
                }
                _selectedParagraph.Text = arr[0];               
                p.Text = arr[1];
            }
            Window.Subtitle.InsertParagraphInCorrectTimeOrder(p);
            Window.Subtitle.Renumber();
            Window.SubtitleTable.ReloadData();
            var index = selectedIndices.First();
            Window.SubtitleTable.SelectRow((nint)index, true);
            Window.SubtitleTable.SelectRow((nint)index + 1, true);
        }


        public void Merge()
        {
            if (_selectedParagraph == null)
                return;

            var selectedIndices = Window.SubtitleTable.SelectedRows;
            if (selectedIndices.Count < 2)
                return;

            var sb = new StringBuilder();
            var deleteIndices = new List<int>();
            bool first = true;
            int firstIndex = 0;
            int next = 0;
            double endTime = 0;
            foreach (var index in selectedIndices)
            {
                if (first)
                {
                    firstIndex = (int)index;
                    next = (int)index + 1;
                }
                else
                {
                    deleteIndices.Add((int)index);
                    if (next != (int)index)
                        return;
                    next++;
                    endTime = Window.Subtitle.Paragraphs[(int)index].EndTime.TotalMilliseconds;
                }
                first = false;
                sb.AppendLine(Window.Subtitle.Paragraphs[(int)index].Text);                              
            }
            if (sb.Length > 200)
                return;

            Window.Subtitle.Paragraphs[firstIndex].Text = HtmlUtil.FixInvalidItalicTags(sb.ToString().Trim());
            Window.Subtitle.Paragraphs[firstIndex].EndTime.TotalMilliseconds = endTime;
            deleteIndices.Reverse();
            foreach (var index in deleteIndices)
                Window.Subtitle.Paragraphs.RemoveAt(index);
            Window.Subtitle.Renumber();
            Window.SubtitleTable.ReloadData();
            Window.SubtitleTable.SelectRow((nint)firstIndex, true);
        }

        public void RemoveFormatting()
        {
            if (Window.Subtitle == null)
                return;
           
            var selectedIndices = Window.SubtitleTable.SelectedRows;
            foreach (var index in selectedIndices)
            {
                var p = Window.Subtitle.Paragraphs[(int)index];
                p.Text = HtmlUtil.RemoveHtmlTags(p.Text, true);
            }
            ReloadDataKeepSelection();
        }

        public void Italic()
        {
            if (Window.Subtitle == null)
                return;

            ListViewToggleTag("i");
        }

        private void ListViewToggleTag(string tag)
        {
            var selectedIndices = Window.SubtitleTable.SelectedRows;
            foreach (var index in selectedIndices)
            {
                var p = Window.Subtitle.Paragraphs[(int)index];
                p.Text = HtmlUtil.ToogleTag(p.Text, tag);
            }
            ReloadDataKeepSelection();
        }

        public void ChangeSubtitleTableSelection()
        {
            if (Window.SubtitleTable.SelectedRows.Count > 0)
            {
                nint selectRow = Window.SubtitleTable.SelectedRow;
                _selectedParagraph = Window.Subtitle.Paragraphs[(int)selectRow];
                Window.SubtitleText.StringValue = _selectedParagraph.Text;
                Window.SetTimeCode(_selectedParagraph);
            }
        }

        public void UpdateStartAndEndTime(int millisecondsToAdd)
        {
            if (_selectedParagraph == null)
                return;

            _selectedParagraph.StartTime.TotalMilliseconds += millisecondsToAdd;
            _selectedParagraph.EndTime.TotalMilliseconds += millisecondsToAdd;
            Window.SetTimeCode(_selectedParagraph);
            ReloadDataKeepSelection();
        }

        public void UpdateEndTime(int millisecondsToAdd)
        {
            if (_selectedParagraph == null)
                return;

            _selectedParagraph.EndTime.TotalMilliseconds += millisecondsToAdd;
            Window.SetTimeCode(_selectedParagraph);
            ReloadDataKeepSelection();
        }

        public void OpenSubtitlePrompt()
        {
            using (var dlg = NSOpenPanel.OpenPanel)
            {
                dlg.CanChooseFiles = true;
                dlg.CanChooseDirectories = false;
                if (dlg.RunModal() == Constants.NSOpenPanelOK)
                {
                    OpenSubtitle(dlg.Filename, false);
                }
            }
        }

        public bool OpenSubtitlePromptForChanges(string fileName, bool requireSaveAs = false)
        {
            if (ContinueIfChanged())
            {
                OpenSubtitle(fileName, requireSaveAs);
                return true;
            }
            return false;
        }

        public void OpenWithManualChosenEncoding()
        {
            string fileName;
            using (var dlg = NSOpenPanel.OpenPanel)
            {
                dlg.CanChooseFiles = true;
                dlg.CanChooseDirectories = false;
                if (dlg.RunModal() == Constants.NSOpenPanelOK)
                {
                    fileName = dlg.Filename;
                }
                else
                {
                    return;
                }
            }

            using (var controller = new File.OpenWithManualChosenEncodingController(fileName))
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
                if (controller.WasOkPressed && ContinueIfChanged())
                {
                    OpenSubtitle(fileName, false, controller.ChosenEcoding);
                }
            }    
        }

        public void OpenSubtitle(string fileName, bool requireSaveAs, Encoding useThisEncoding = null)
        {
            if (System.IO.File.Exists(fileName))
            {
                //    bool videoFileLoaded = false;
                var file = new FileInfo(fileName);
                var ext = file.Extension.ToLowerInvariant();

                _subtitle = new Subtitle();
                Encoding encoding = useThisEncoding;
                if (encoding == null)
                {
                    LanguageAutoDetect.GetEncodingFromFile(fileName);
                }
                SubtitleFormat format = _subtitle.LoadSubtitle(fileName, out encoding, encoding);

                // check for idx file
                if (format == null && file.Length > 100 && ext == ".idx")
                {
                    MessageBox.Show(_language.ErrorLoadIdx);
                    return;
                }

                // check for .rar file
                if (format == null && file.Length > 100 && FileUtil.IsRar(fileName))
                {
                    MessageBox.Show(_language.ErrorLoadRar);
                    return;
                }

                // check for .zip file
                if (format == null && file.Length > 100 && FileUtil.IsZip(fileName))
                {
                    MessageBox.Show(_language.ErrorLoadZip);
                    return;
                }

                // check for .png file
                if (format == null && file.Length > 100 && FileUtil.IsPng(fileName))
                {
                    MessageBox.Show(_language.ErrorLoadPng);
                    return;
                }

                // check for .jpg file
                if (format == null && file.Length > 100 && FileUtil.IsJpg(fileName))
                {
                    MessageBox.Show(_language.ErrorLoadJpg);
                    return;
                }

                // check for .srr file
                if (format == null && file.Length > 100 && ext == ".srr" && FileUtil.IsSrr(fileName))
                {
                    MessageBox.Show(_language.ErrorLoadSrr);
                    return;
                }

                // check for Torrent file
                if (format == null && file.Length > 50 && FileUtil.IsTorrentFile(fileName))
                {
                    MessageBox.Show(_language.ErrorLoadTorrent);
                    return;
                }

                // check for all binary zeroes (I've heard about this a few times... perhaps related to crashes?)
                if (format == null && file.Length > 50 && FileUtil.IsSubtitleFileAllBinaryZeroes(fileName))
                {
                    MessageBox.Show(_language.ErrorLoadBinaryZeroes);
                    return;
                }


                if (format == null && file.Length < 500000)
                {

                    // Try to use a generic subtitle format parser (guessing subtitle format)
                    try
                    {
                        var enc = LanguageAutoDetect.GetEncodingFromFile(fileName);
                        var s = System.IO.File.ReadAllText(fileName, enc);

                        var uknownFormatImporter = new UknownFormatImporter { UseFrames = true };
                        var genericParseSubtitle = uknownFormatImporter.AutoGuessImport(s.SplitToLines());
                        if (genericParseSubtitle.Paragraphs.Count > 1)
                        {
                            _subtitle = genericParseSubtitle;
                            //ShowStatus("Guessed subtitle format via generic subtitle parser!");
                        }
                    }
                    catch
                    {
                    }
                }

                if (format != null && format.IsFrameBased)
                    _subtitle.CalculateTimeCodesFromFrameNumbers(CurrentFrameRate);
                else
                    _subtitle.CalculateFrameNumbersFromTimeCodes(CurrentFrameRate);

                if (format != null)
                {
                    if (Configuration.Settings.General.RemoveBlankLinesWhenOpening)
                    {
                        _subtitle.RemoveEmptyLines();
                    }

                    foreach (var p in _subtitle.Paragraphs)
                    {
                        // Replace U+0456 (CYRILLIC SMALL LETTER BYELORUSSIAN-UKRAINIAN I) by U+0069 (LATIN SMALL LETTER I)
                        p.Text = p.Text.Replace("<і>", "<i>").Replace("</і>", "</i>");
                    }
                    _subtitleFileName = fileName;
                    SetSubtitleFormat(format);
                    Window.SetEncoding(encoding.BodyName);
                    SetNewSubtitle(_subtitle);
                    Configuration.Settings.RecentFiles.Add(fileName, null, null);
                    UpdateRecentFiles();

                    TryToFindAndOpenVideoFile(_subtitleFileName);
                    ShowAudioVisualizerIfDataIsReady();

                }
                else
                {
                    //
                }
            }
            else
            {
                MessageBox.Show(string.Format(_language.FileNotFound, fileName));
            }
        }

        private void ShowAudioVisualizerIfDataIsReady()
        {
            _timerWaveform.Stop();
            Window.ShowAddAudioVisualizer();
            if (!string.IsNullOrEmpty(_videoFileName))
            {
                string peakWaveFileName = WavePeakGenerator.GetPeakWaveFileName(_videoFileName);
                //                    string spectrogramFolder = Nikse.SubtitleEdit.Core.WavePeakGenerator.SpectrogramDrawer.GetSpectrogramFolder(_videoFileName);
                if (System.IO.File.Exists(peakWaveFileName))
                {
                    Window.HideAddAudioVisualizer();
                    _audioVisualizer.WavePeaks = WavePeakData.FromDisk(peakWaveFileName);
                    //                        audioVisualizer.Spectrogram = SpectrogramData.FromDisk(spectrogramFolder);
                    //                        toolStripComboBoxWaveform_SelectedIndexChanged(null, null);
                    SetWaveformPosition(0, 0, 0);
                    _timerWaveform.Start();
                }
            }
        }

        public bool SaveAsSubtitlePrompt()
        {
            if (Window.Subtitle.Paragraphs.Count == 0)
            {
                MessageBox.Show(_language.NoSubtitlesFound);
                return true;
            }

            var dlg = new NSSavePanel();
            dlg.AllowedFileTypes = new string[] { GetCurrentSubtitleFormat().Extension.TrimStart('.') };
            dlg.AllowsOtherFileTypes = false;
            if (_subtitleFileName != null)
            {
                dlg.NameFieldStringValue = Path.GetFileNameWithoutExtension(_subtitleFileName);
            }
            dlg.Title = _language.SaveSubtitleAs;
            ;
            if (dlg.RunModal() == Constants.NSOpenPanelOK)
            {
                SaveSubtitle(dlg.Filename);
                return true;
            }
            return false;
        }

        public bool SaveSubtitle()
        {           
            if (string.IsNullOrEmpty(_subtitleFileName) || _subtitleOrginalFormat != GetCurrentSubtitleFormat().FriendlyName)
            {
                return SaveAsSubtitlePrompt();
            }
            else
            {
                return SaveSubtitle(_subtitleFileName);
            }
        }

        public bool SaveSubtitle(string fileName)
        {
            _subtitleFileName = fileName;
            var format = GetCurrentSubtitleFormat();
            string allText = Window.Subtitle.ToText(format);
            var currentEncoding = GetCurrentEncoding();

            using (var fs = System.IO.File.Open(_subtitleFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var sw = new StreamWriter(fs, currentEncoding))
            {
                sw.Write(allText);
            }
            _subtitleOriginalHash = Window.Subtitle.GetFastHashCode();
            _subtitleOrginalFormat = GetCurrentSubtitleFormat().FriendlyName;
            SetTitle();

            Configuration.Settings.RecentFiles.Add(fileName, null, null);
            UpdateRecentFiles();

            return true;
        }

        public void SubtitleTextChanged()
        {          
            if (_selectedParagraph == null)
                return;

            _selectedParagraph.Text = Window.SubtitleText.StringValue;
            ReloadDataKeepSelection();
        }

        public double CurrentFrameRate
        {
            get { return Configuration.Settings.General.CurrentFrameRate; }
        }

        public Encoding GetCurrentEncoding()
        {
            return Window.GetEncoding();
        }

        public SubtitleFormat GetCurrentSubtitleFormat()
        {
            var frindlyName = Window.SubtitleFormats.SelectedValue.ToString();
            var format = SubtitleFormat.AllSubtitleFormats.FirstOrDefault(p => p.FriendlyName == frindlyName);
            if (format == null)
                return new SubRip();
            return format;
        }

        public bool ContinueIfChanged()
        {
            if (Window.Subtitle.Paragraphs.Count == 0 || Window.Subtitle.GetFastHashCode() == _subtitleOriginalHash)
            {
                return true;
            }

            NSAlert alert = new NSAlert();
            if (_subtitleFileName == null)
            {
                alert.MessageText = _language.SaveChangesToUntitled;
            }
            else
            {
                alert.MessageText = string.Format(_language.SaveChangesToX, _subtitleFileName);
            }
            alert.AddButton("Yes".Replace("&", string.Empty));
            alert.AddButton("No".Replace("&", string.Empty));
            alert.AddButton(_languageGeneral.Cancel.Replace("&", string.Empty));
            var result = alert.RunModal();
            if (result == (long)(NSAlertButtonReturn.First))
            {
                return SaveSubtitle();
            }
            else if (result == (long)(NSAlertButtonReturn.Second))
            {
                Window.Subtitle.Paragraphs.Clear(); 
                return true;
            }
            return false;
        }

        public void SetTitle()
        {
            string title = "Subtitle Edit 3.4 beta 1";
            if (!string.IsNullOrWhiteSpace(_subtitleFileName))
                title += " - " + _subtitleFileName;
            Window.Title = title;
        }

        public void DoAdjustment(double milliseconds, AdjustmentSelection selection)
        {
            double adjustInSeconds = milliseconds / 1000.0;
            if (selection == AdjustmentSelection.SelectedLines)
            {               
                var selectedIndices = Window.SubtitleTable.SelectedRows;
                foreach (var index in selectedIndices)
                {
                    Window.Subtitle.Paragraphs[(int)index].Adjust(1, adjustInSeconds);
                }
            }
            else
            {
                foreach (var p in Window.Subtitle.Paragraphs)
                {
                    p.Adjust(1, adjustInSeconds);
                }
            }
            ReloadDataKeepSelection();   
        }

        public void UpdateRecentFiles()
        {
            AppDelegate ad = (AppDelegate)NSApplication.SharedApplication.Delegate;
            var recentMenu = new NSMenu("DemoApp");
            int count = 0;
            foreach (var item in Configuration.Settings.RecentFiles.Files)
            {
                if (System.IO.File.Exists(item.FileName))
                {
                    var menuItem = new NSMenuItem(System.IO.Path.GetFileName(item.FileName), (s, e) =>
                        {
                            OpenSubtitlePromptForChanges(item.FileName);
                        });
                    recentMenu.AddItem(menuItem);
                    count++;
                }
            }
            if (count > 0)
            {
                recentMenu.AddItem(NSMenuItem.SeparatorItem); 
                recentMenu.AddItem(new NSMenuItem("Clear", (s, e) =>
                        { 
                            Configuration.Settings.RecentFiles.Files.Clear();
                            UpdateRecentFiles();
                        }));
            }
            ad.OpenRecent.Submenu = recentMenu;
        }

        public void RemoveTextForHi()
        {
            using (var controller = new RemoveTextForHearingImpairedController(Window.Subtitle))
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
                if (controller.WasOkPressed)
                {
                    ReloadSubtitle(controller.FixedSubtitle, false);
                }
            }   
        }

        public void ShowMultipleReplace()
        {
            using (var controller = new MultipleReplaceController(Window.Subtitle))
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
                if (controller.WasOkPressed)
                {
                    ReloadSubtitle(controller.FixedSubtitle, false);
                }
            }     
        }

        public void FixCommonErrors()
        {
            using (var controller = new FixCommonErrorsController(Window.Subtitle))
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
                if (controller.WasOkPressed)
                {
                    ReloadSubtitle(controller.Subtitle, false);
                }
            }     
        }

        public void Find()
        {
            using (var controller = new FindController(Window.Subtitle))
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
                if (controller.WasFindPressed)
                {
                    _findReplaceHelper = controller.FindReplaceInfo;
                    if (_findReplaceHelper.Success)
                    {
                        ShowSubtitleRow((nint)_findReplaceHelper.CurrentLineIndex);
                        Window.FocusAndHighLightText(_findReplaceHelper.CurrentStringIndex, _findReplaceHelper.FindTextLength);
                    }
                }
            }            
        }

        public void FindNext()
        {
            if (string.IsNullOrEmpty(_findReplaceHelper.FindText))
            {
                Find();
                return;
            }

            var selectedRows = Window.SubtitleTable.SelectedRows;
            if (selectedRows.Count == 0)
                return;

            var selectedRow = (int)selectedRows.First();
            if (selectedRow == _findReplaceHelper.CurrentLineIndex)
            {
                _findReplaceHelper.CurrentStringIndex++;
            }
            else
            {
                _findReplaceHelper.CurrentLineIndex = selectedRow;
            }
            _findReplaceHelper.PerformFind(Window.Subtitle);
            if (_findReplaceHelper.Success)
            {
                ShowFindResult(_findReplaceHelper);
            }
        }

        internal void ShowFindResult(FindReplaceInfo findReplace)
        {
            ShowSubtitleRow((nint)findReplace.CurrentLineIndex);
            Window.FocusAndHighLightText(findReplace.CurrentStringIndex, findReplace.FindTextLength);
        }

        public void FindPrevious()
        {
            if (string.IsNullOrEmpty(_findReplaceHelper.FindText))
            {
                Find();
                return;
            }

            var selectedRows = Window.SubtitleTable.SelectedRows;
            if (selectedRows.Count == 0)
                return;

            var selectedRow = (int)selectedRows.First();
            if (selectedRow == _findReplaceHelper.CurrentLineIndex)
            {
                if (_findReplaceHelper.CurrentLineIndex > 0)
                {
                    _findReplaceHelper.CurrentLineIndex--;
                }
            }
            else
            {
                _findReplaceHelper.CurrentLineIndex = selectedRow;
            }
            _findReplaceHelper.PerformFind(Window.Subtitle, false);
            if (_findReplaceHelper.Success)
            {
                ShowFindResult(_findReplaceHelper);
            }            
        }

        internal void ShowReplaceResult(FindReplaceInfo findReplace)
        {
            _findReplaceHelper = findReplace;
            ReloadSubtitle(Window.Subtitle, false);
            ShowSubtitleRow((nint)findReplace.CurrentLineIndex);
        }

        public void Replace()
        {
            using (var controller = new ReplaceController(Window.Subtitle, this))
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
                if (controller.WasFindPressed)
                {
                    _findReplaceHelper = controller.FindReplaceInfo;
                    if (_findReplaceHelper.Success)
                    {
                        //   ShowSubtitleRow((nint)_findReplaceHelper.CurrentLineIndex);
                        //   Window.FocusAndHighLightText(_findReplaceHelper.CurrentStringIndex, _findReplaceHelper.FindTextLength);
                    }
                }
            }            
        }

        public void SubtitleRowDoubleClicked()
        {
            if (_selectedParagraph == null || string.IsNullOrEmpty(_videoFileName))
            {
                return;
            }

            _videoPlayer.Position = _selectedParagraph.StartTime.TotalSeconds + 0.1;
            SetWaveformPosition(_selectedParagraph.StartTime.TotalSeconds - 1.0, _selectedParagraph.StartTime.TotalSeconds + 0.1, _subtitle.GetIndex(_selectedParagraph));
        }

        public void ShowPreferences()
        {
            using (var controller = new PreferencesWindowController())
            {
                controller.Window.ReleasedWhenClosed = true;
                var oldVideoPlayer = Configuration.Settings.General.VideoPlayer;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
                if (oldVideoPlayer != Configuration.Settings.General.VideoPlayer)
                {
                    InitializeVideoPlayer();
                    if (!string.IsNullOrEmpty(_videoFileName))
                    {
                        OpenVideo(_videoFileName);
                    }
                }
            }    
        }

        public void SpellCheckAndGrammer()
        {
            using (var controller = new SpellCheckController(Window.Subtitle))
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
            }    
        }

        public void InitializeVideoPlayer()
        {
            // clean up if already called
            if (_subtitleDisplayTimer != null)
            {
                _subtitleDisplayTimer.Stop();
            }
            foreach (var subView in Window.VideoPlayerView.Subviews)
            {
                subView.RemoveFromSuperview();
            }
            if (_videoPlayer != null)
            {
                _videoPlayer.DisposeVideoPlayer();
            }

            if (Configuration.Settings.General.VideoPlayer == "AVFoundation" || !LibVlcDynamic.IsVlcAvailable())
            {
                _videoPlayer = new AVFPlayerController();
            }
            else
            {
                _videoPlayer = new LibVlcPlayerViewController();
            }

            _videoPlayer.View.AutoresizesSubviews = true;
            _videoPlayer.View.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
            _videoPlayer.View.Frame = Window.VideoPlayerView.Bounds;
            Window.VideoPlayerView.AddSubview(_videoPlayer.View);
            Window.VideoPlayerView.Frame = Window.VideoPlayerView.Bounds;

            _subtitleDisplayTimer = new System.Timers.Timer(100); 
            _subtitleDisplayTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                try
                {

                    if (_subtitle == null || _videoPlayer == null)
                    {
                        return;
                    }

                    // check for same position
                    var currentPositionInSeconds = _videoPlayer.Position;
                    if (Math.Abs(currentPositionInSeconds - _subtitleDisplayTimerLastPosition) < 0.001)
                    {
                        return;
                    }
                    _subtitleDisplayTimerLastPosition = currentPositionInSeconds;

                    // find current paragraph from current video position
                    Paragraph currentParagraph = null;
                    foreach (Paragraph p in _subtitle.Paragraphs)
                    {
                        if (p.StartTime.TotalSeconds <= currentPositionInSeconds && currentPositionInSeconds <= p.EndTime.TotalSeconds)
                        {
                            currentParagraph = p;
                            if (string.CompareOrdinal(p.Text, _subtitleDisplayTimerLastText) == 0)
                            {
                                return;
                            }
                            break;
                        }
                    }

                    if (currentParagraph == null)
                    {
                        // last show subtitle text was also blank
                        if (_subtitleDisplayTimerLastText == null)
                        {
                            return;
                        }
                        _subtitleDisplayTimerLastText = null;
                    }
                    else
                    {
                        _subtitleDisplayTimerLastText = currentParagraph.Text;
                    }

                    InvokeOnMainThread(() =>
                        {
                            if (_videoPlayer != null)
                            {
                                _videoPlayer.ShowSubtitle(currentParagraph);
                            }
                        });
                }
                catch (Exception exception)
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                }
            };

            _subtitleDisplayTimer.Start();

        }

        private void TryToFindAndOpenVideoFile(string fileNameNoExtension)
        {
            string movieFileName = null;

            foreach (var extension in Utilities.GetMovieFileExtensions())
            {
                var fileName = fileNameNoExtension + extension;
                if (System.IO.File.Exists(fileName))
                {
                    movieFileName = fileName;
                    break;
                }
            }

            if (movieFileName != null)
            {
                OpenVideo(movieFileName);
            }
            else
            {
                var index = fileNameNoExtension.LastIndexOf('.');
                if (index > 0)
                {
                    TryToFindAndOpenVideoFile(fileNameNoExtension.Remove(index));
                }
            }
        }

        private void OpenVideo(string fileName)
        {
            _videoFileName = fileName;
            _videoPlayer.Open(fileName);
            _videoPlayer.Play();
        }

        public void OpenVideo()
        {
            using (var dlg = NSOpenPanel.OpenPanel)
            {
                dlg.CanChooseFiles = true;
                dlg.CanChooseDirectories = false;
                if (dlg.RunModal() == Constants.NSOpenPanelOK)
                {
                    OpenVideo(dlg.Filename);
                }
            }
        }

        public void AddAudioVisualization()
        {            
            using (var controller = new AddWaveFormController(_videoFileName, ""))
            {
                controller.Window.ReleasedWhenClosed = true;
                NSApplication.SharedApplication.RunModalForWindow(controller.Window); // window's WillClose event stops modal
            }    
            ShowAudioVisualizerIfDataIsReady();
        }

        private void SetWaveformPosition(double currentVideoPositionSeconds, int subtitleIndex)
        {
            var list = new List<int>();
            foreach (var index in Window.SubtitleTable.SelectedRows)
            {
                list.Add((int)index);
            }
            _audioVisualizer.SetPosition(_audioVisualizer.StartPositionSeconds, _subtitle, currentVideoPositionSeconds, subtitleIndex, list);
        }

        private void SetWaveformPosition(double startPositionSeconds, double currentVideoPositionSeconds, int subtitleIndex)
        {
            var list = new List<int>();
            foreach (var index in Window.SubtitleTable.SelectedRows)
            {
                list.Add((int)index);
            }
            _audioVisualizer.SetPosition(startPositionSeconds, _subtitle, currentVideoPositionSeconds, subtitleIndex, list);
        }

    }
}
