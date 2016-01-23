using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using System.Text;
using Nikse.SubtitleEdit.Core;

namespace VLC
{
    public partial class LibVlcPlayerView : AppKit.NSView
    {
        #region Constructors

        // Called when created from unmanaged code
        public LibVlcPlayerView(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public LibVlcPlayerView(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        public NSBox VideoBox
        {
            get
            { 
                return _videoBox;
            }
        }

        public NSButton PlayPauseButton
        {
            get 
            {
                return _buttonPlayPause;    
            }
        }

        public NSButton StopButton
        {
            get 
            {
                return _buttonStop;    
            }
        }

        public NSView VideoView
        {
            get 
            {
                return _customView;   
            }
        }

        public void SetCurrentPosition(double currentPositionInseconds, double durationInSeconds)
        {
            var pos = new TimeCode(currentPositionInseconds * 1000.0);
            var dur = new TimeCode(durationInSeconds * 1000.0);
            _labelPosition.StringValue = pos.ToShortDisplayString() + " / " + dur.ToShortDisplayString();

            _positionSlider.DoubleValue = currentPositionInseconds / durationInSeconds * 100.0;
        }

        public NSSlider PositionSlider
        {
            get 
            {
                return _positionSlider;   
            }
        }

        public NSSlider VolumeSlider
        {
            get 
            {
                return _volumeSlider;   
            }
        }

        public void ShowSubtitle(Nikse.SubtitleEdit.Core.Paragraph p)
        {
            if (p == null)
            { 
                _subtitleWebView.MainFrame.LoadHtmlString(new NSString("<body style='background-color:black'></body>"), null);
            }
            else
            {
                var sb = new StringBuilder();
                bool first = true;
                foreach (var line in p.Text.SplitToLines())
                {
                    if (!first)
                    {
                        sb.Append("<br />");
                    }
                    sb.Append(line);
                    first = false;
                }
                _subtitleWebView.MainFrame.LoadHtmlString(new NSString("<body style='background-color:black;color:white;text-align:center'>" + sb.ToString() +  "<body>"), null);
            }
           
        }
        #endregion
    }
}
