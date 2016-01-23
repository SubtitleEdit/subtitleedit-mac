using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace SubtitleEdit
{
    public partial class PreferencesVideoController : AppKit.NSViewController
    {
        #region Constructors

        // Called when created from unmanaged code
        public PreferencesVideoController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public PreferencesVideoController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public PreferencesVideoController()
            : base("PreferencesVideo", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        //strongly typed view accessor
        public new PreferencesVideo View
        {
            get
            {
                return (PreferencesVideo)base.View;
            }
        }
    }
}
