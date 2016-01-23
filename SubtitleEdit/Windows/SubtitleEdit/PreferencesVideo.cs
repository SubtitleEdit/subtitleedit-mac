using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using VLC;

namespace SubtitleEdit
{
    public partial class PreferencesVideo : AppKit.NSView
    {
        #region Constructors

        // Called when created from unmanaged code
        public PreferencesVideo(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public PreferencesVideo(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            var vlcAvailable = LibVlcDynamic.IsVlcAvailable();

            if (Configuration.Settings.General.VideoPlayer == "AVFoundation" || !vlcAvailable)
            {
                _radioAVF.State = NSCellStateValue.On;
                _radioVlc.State = NSCellStateValue.Off;
            }
            else
            {
                _radioAVF.State = NSCellStateValue.Off;
                _radioVlc.State = NSCellStateValue.On;
            }

            if (!vlcAvailable)
            {
                _radioVlc.Enabled = false;
            }

            _radioAVF.Activated += (object sender, EventArgs e) => 
                {
                    Configuration.Settings.General.VideoPlayer = "AVFoundation";
                };

            _radioVlc.Activated += (object sender, EventArgs e) => 
                {
                    Configuration.Settings.General.VideoPlayer = "VLC";
                };
        }      

        #endregion
    }
}
