using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using Nikse.SubtitleEdit.UILogic;
using Nikse.SubtitleEdit.Core.SubtitleFormats;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using UILogic;
using CoreGraphics;
using Nikse.SubtitleEdit.Core.DetectEncoding;
using MacLibSe;

namespace Nikse.SubtitleEdit.Windows
{
    public partial class MainWindow : NSWindow
    {

        AudioVisualizerView _audioVisualizerView;

        public NSTableView SubtitleTable
        {
            get
            { 
                return _subtitleTable;
            }
        }

        public Subtitle Subtitle
        {
            get
            { 
                return (_subtitleTable.DataSource as SubtitleTableDataSource).Subtitle;
            }
        }

        public NSTextField SubtitleText
        {
            get
            { 
                return subtitleText;
            }
        }

        public NSComboBox SubtitleFormats
        {
            get
            { 
                return toolbarSubtitleFormatComboBox;
            }
        }

        public NSView VideoPlayerView
        {
            get
            { 
                return _videoPlayerView;
            }
        }

        public void SetTimeCode(Paragraph p)
        {
            _startTime.StringValue = string.Format("{0:00}:{1:00}:{2:00}{3}{4:000}", p.StartTime.Hours, p.StartTime.Minutes, p.StartTime.Seconds, System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, p.StartTime.Milliseconds);
            _duration.StringValue = string.Format("{0:0.000}", p.Duration.TotalSeconds);
        }

        public MainWindow(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public MainWindow(NSCoder coder)
            : base(coder)
        {
        }

        void InitializeSubtitleTable()
        {           
            var columns = _subtitleTable.TableColumns();
            columns[0].SetIdentifier(SubtitleTableDelegate.CellIdentifiers[0]);
            columns[0].MinWidth = 20;
            columns[0].MaxWidth = 100;
            columns[0].Width = 40;
            columns[0].Title = Configuration.Settings.Language.General.LineNumber;
            columns[1].SetIdentifier(SubtitleTableDelegate.CellIdentifiers[1]);
            columns[1].MinWidth = 50;
            columns[1].MaxWidth = 200;
            columns[1].Width = 90;
            columns[1].Title = Configuration.Settings.Language.General.StartTime;
            _subtitleTable.AddColumn(new NSTableColumn(SubtitleTableDelegate.CellIdentifiers[2])
                {
                    MinWidth = 50,
                    MaxWidth = 200,
                    Width = 90,
                    Title = Configuration.Settings.Language.General.EndTime,
                });
            _subtitleTable.AddColumn(new NSTableColumn(SubtitleTableDelegate.CellIdentifiers[3])
                {
                    MinWidth = 40,
                    MaxWidth = 150,
                    Width = 60,
                    Title = Configuration.Settings.Language.General.Duration,
                });
            _subtitleTable.AddColumn(new NSTableColumn(SubtitleTableDelegate.CellIdentifiers[4])
                {
                    MinWidth = 100,
                    MaxWidth = 90000,
                    Width = 1000,
                    Title = Configuration.Settings.Language.General.Text,
                });

            // set up events on context menu
            _subtitleTableContextMenuDeleteLines.Menu.Delegate = new SubtitleTableContextMenuDelegate(this);
            _subtitleTableContextMenuDeleteLines.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).DeleteLines();
            };  
            _subtitleTableContextMenuInsertText.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).InsertFirstLine();
            };  
            _subtitleTableContextMenuInsertBefore.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).InsertBefore();
            };  
            _subtitletableContextMenuInsertAfter.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).InsertAfter();
            };  
            _subtitleTableContextMenuMerge.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).Merge();
            }; 
            _subtitletableContextMenuSplit.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).Split();
            }; 
            _subtitleTableContextMenuNormal.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).RemoveFormatting();
            };  
            _subtitleTableContextMenuItalic.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).Italic();
            };  

            _subtitleTable.DoubleClick += (object sender, EventArgs e) =>
            {
                var x = _subtitleTable.SelectedRows;    
                if (x.Count == 1)
                {                    
                    (WindowController as MainWindowController).ShowSubtitleRow((int)x.ToList().First());
                    (WindowController as MainWindowController).SubtitleRowDoubleClicked();
                }
            };
            
            // drag'n'drop
            _subtitleTable.RegisterForDraggedTypes(new string[] { "public.data" });
        }

        void InitializeToolbar()
        {
            toolbarNew.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).NewSubtitle();
            };
            toolbarOpen.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).OpenSubtitlePrompt();
            };
            toolbarSave.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).SaveSubtitle();
            };
            toolbarSaveAs.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).SaveAsSubtitlePrompt();
            };                              
            toolbarHelp.Activated += (object sender, EventArgs e) =>
            {
                Utilities.ShowHelp(null);
            };
            toolbarFind.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).Find();
            };
            toolbarReplace.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).Replace();
            };
            _toolbarSpellCheck.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).SpellCheckAndGrammer();
            };

            toolbarSubtitleFormat.Label = "Format";
            toolbarSubtitleFormatComboBox.RemoveAll();
            foreach (var subtitleFormat in SubtitleFormat.AllSubtitleFormats)
            {
                toolbarSubtitleFormatComboBox.Add(new NSString(subtitleFormat.FriendlyName));
            }
            toolbarSubtitleFormatComboBox.SelectItem(0);

            toolbarEncodingComboBox.RemoveAll();
            toolbarEncodingComboBox.Add(new NSString(Encoding.UTF8.BodyName));
            foreach (var ei in EncodingHelper.GetEncodings())
            {
                if (ei.Name != Encoding.UTF8.BodyName && ei.CodePage >= 949 && !ei.Name.StartsWith("IBM") && ei.CodePage != 1047) //Configuration.Settings.General.EncodingMinimumCodePage)
                    toolbarEncodingComboBox.Add(new NSString(ei.CodePage + ": " + ei.Name));
            }
            SetEncoding(Encoding.UTF8.BodyName);

            (WindowController as MainWindowController).NewSubtitle();

            _splitViewHor.Delegate = new MainWindowSplitViewHDelegate();
            _splitViewVert.Delegate = new MainWindowSplitViewVDelegate();

            _toolbarShowAudioViz.Activated += (object sender, EventArgs e) =>
            {
                ShowHideAudioVisualizer();
            };
            _toolbarShowVideo.Activated += (object sender, EventArgs e) =>
            {
                ShowHideVideoPlayer();
            };
            //  _subtitleView.AutoresizingMask = NSViewResizingMask.HeightSizable;
        }

        public void ShowHideAudioVisualizer()
        {
            _audioVizBox.Hidden = !_audioVizBox.Hidden;
            _splitViewHor.AdjustSubviews();
            _splitViewVert.AdjustSubviews();
            if (_audioVizBox.Hidden)
            {
                _toolbarShowAudioViz.Image = NSImage.ImageNamed("WaveformToggle.png");
                Configuration.Settings.General.ShowAudioVisualizer = false;
            }
            else
            {
                _toolbarShowAudioViz.Image = NSImage.ImageNamed("WaveformToggleActive.png");
                Configuration.Settings.General.ShowAudioVisualizer = true;
            }
        }

        public void ShowHideVideoPlayer()
        {
            _videoBox.Hidden = !_videoBox.Hidden;
            _splitViewHor.AdjustSubviews();
            _splitViewVert.AdjustSubviews();
            if (_videoBox.Hidden)
            {
                _toolbarShowVideo.Image = NSImage.ImageNamed("VideoToggle.png");
                Configuration.Settings.General.ShowVideoPlayer = false;
            }
            else
            {
                _toolbarShowVideo.Image = NSImage.ImageNamed("VideoToggleActive.png");
                Configuration.Settings.General.ShowVideoPlayer = true;
            }
        }

        public void SetEncoding(string encoding)
        {
            for (int i = 0; i < toolbarEncodingComboBox.Count; i++)
            {
                var item = toolbarEncodingComboBox.Values[i];
                if (item.ToString() == encoding || item.ToString().EndsWith(": " + encoding))
                {
                    toolbarEncodingComboBox.Select(item);
                    break;
                }
            }
        }

        public Encoding GetEncoding()
        {
            var selectedValue = toolbarEncodingComboBox.SelectedValue.ToString();           
            foreach (var ei in EncodingHelper.GetEncodings())
            {
                if (selectedValue.StartsWith(ei.CodePage + ":"))
                {
                    try
                    {
                        return ei.GetEncoding();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Encoding is missing: " + exception.Message);
                    }
                }
            }
            return Encoding.UTF8;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.Title = "Subtitle Edit";

            InitializeToolbar();
            InitializeSubtitleTable();
                       
            subtitleText.Delegate = new SubtitleTextDelegate(WindowController as MainWindowController);   
                       
            _startTimeStepper.MinValue = -10000;
            _startTimeStepper.MaxValue = 10000;
            _startTimeStepper.Increment = 100;
            _startTimeStepper.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).UpdateStartAndEndTime(_startTimeStepper.IntValue);
                _startTimeStepper.IntValue = 0;
            };

            _startTime.Activated += (object sender, EventArgs e) =>
            {
                var ms = TimeCode.ParseToMilliseconds(_startTime.StringValue);
                (WindowController as MainWindowController).SetStartTime(ms);
            };

            _durationStepper.MinValue = -10000;
            _durationStepper.Increment = 100;
            _durationStepper.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).UpdateEndTime(_durationStepper.IntValue);
                _durationStepper.IntValue = 0;
            };

            _duration.Activated += (object sender, EventArgs e) =>
            {
                double d;
                if (double.TryParse(_duration.StringValue, out d))
                {
                    (WindowController as MainWindowController).UpdateDuration(d * 1000.0);
                }
            };

            FixMacButtonTexts();
            
            this.Delegate = new MainWindowDelegate(WindowController as MainWindowController);

            (WindowController as MainWindowController).InitializeVideoPlayer();


            _buttonAddWaveform.Activated += (object sender, EventArgs e) =>
            {
                (WindowController as MainWindowController).AddAudioVisualization();
            };
            _audioVisualizerView = new global::UILogic.AudioVisualizerView();
            _audioViz.Superview.AddSubview(_audioVisualizerView);
            _audioVisualizerView.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
            _audioVisualizerView.SetFrameSize(_audioViz.Superview.Frame.Size);
            _audioVisualizerView.BecomeFirstResponder();
            _audioViz.Hidden = true;
            if (!Configuration.Settings.General.ShowAudioVisualizer)
            {
                ShowHideAudioVisualizer();
            }
            if (!Configuration.Settings.General.ShowVideoPlayer)
            {
                ShowHideVideoPlayer();
            }
        }

        internal void ShowAddAudioVisualizer()
        {
            _buttonAddWaveform.Hidden = false;
        }

        internal void HideAddAudioVisualizer()
        {
            _buttonAddWaveform.Hidden = true;
        }

        private void FixMacButtonTexts()
        {
            Configuration.Settings.Language.General.Ok = Configuration.Settings.Language.General.Ok.RemoveWindowsShortCut();
            Configuration.Settings.Language.General.Cancel = Configuration.Settings.Language.General.Cancel.RemoveWindowsShortCut();
        }

        public override void SendEvent(NSEvent theEvent)
        {
            if (theEvent.Type == NSEventType.KeyDown)
            {
                if (HandleKeyDown(theEvent))
                {
                    return;
                }
            }
            base.SendEvent(theEvent);
        }

        public nint ShowSubtitleRow(nint rowNumber)
        {
            if (_subtitleTable.RowCount == 0)
                return -1;
            
            if (rowNumber >= _subtitleTable.RowCount)
                rowNumber = _subtitleTable.RowCount - 1;
            
            if (rowNumber < 0)
                rowNumber = 0;
            
            _subtitleTable.SelectRow(rowNumber, false);
            _subtitleTable.ScrollRowToVisible(rowNumber);
            subtitleText.StringValue = (_subtitleTable.DataSource as SubtitleTableDataSource).Subtitle.Paragraphs[(int)rowNumber].Text;
            return rowNumber;
        }

        /// <summary>
        /// Handles the key down.
        /// </summary>
        /// <returns><c>true</c>, if key down was handled, <c>false</c> otherwise.</returns>
        /// <param name="theEvent">The event.</param>
        bool HandleKeyDown(NSEvent theEvent)
        {
            if (theEvent.ModifierFlags.HasFlag(NSEventModifierMask.AlternateKeyMask) && theEvent.KeyCode == (ushort)NSKey.DownArrow)
            {
                nint selectRow = 0;
                if (_subtitleTable.SelectedRows.Count > 0)
                {
                    selectRow = _subtitleTable.SelectedRow;
                    if (selectRow + 2 < _subtitleTable.RowCount)
                    {
                        selectRow++;
                    }
                }
                ShowSubtitleRow(selectRow);
                return true;
            }
            else if (theEvent.ModifierFlags.HasFlag(NSEventModifierMask.AlternateKeyMask) && theEvent.KeyCode == (ushort)NSKey.UpArrow)
            {
                nint selectRow = 0;
                if (_subtitleTable.SelectedRows.Count > 0)
                {
                    selectRow = _subtitleTable.SelectedRow;
                    if (selectRow > 0)
                    {
                        selectRow--;
                    }
                }
                ShowSubtitleRow(selectRow);
                return true;
            }
            else if (_subtitleTable == FirstResponder)
            {
                if (theEvent.KeyCode == (ushort)NSKey.Delete || theEvent.KeyCode == (ushort)NSKey.ForwardDelete)
                {
                    (WindowController as MainWindowController).DeleteLines();
                }
            }
            else
            {
                
            }

            return false; // event not handled
        }

        public void ContextMenuWillOpen()
        {
            for (int i = 0; i < _subtitleTableContextMenuDeleteLines.Menu.Count; i++)
            {
                _subtitleTableContextMenuDeleteLines.Menu.ItemAt(i).Hidden = _subtitleTable.RowCount == 0;
            }
            _subtitleTableContextMenuInsertText.Hidden = _subtitleTable.RowCount != 0;

            var selectCount = _subtitleTable.SelectedRowCount;
            if (_subtitleTable.RowCount == 0 || selectCount == 0)
            {                
                return;
            }

            //    var l = Configuration.Settings.Language.Main;
            _subtitleTableContextMenuDeleteLines.Title = "Delete";
            if (selectCount == 1)
            {

            }
            else if (selectCount == 2)
            {

            }
            else
            {
            }
        }

        public void FocusAndHighLightText(int currentStringIndex, int findTextLength)
        {     
            MakeFirstResponder(subtitleText);
            var nsText = subtitleText.CurrentEditor;
            if (nsText == null)
            {
                return;              
            }               
            nsText.SelectedRange = new NSRange(currentStringIndex, findTextLength);
            ;
        }

        public AudioVisualizerView AudioVisualizerView
        {
            get { return _audioVisualizerView; }
//            get { return _audioViz; }
        }

    }
}
