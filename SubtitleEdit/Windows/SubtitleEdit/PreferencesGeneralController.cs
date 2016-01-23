using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace SubtitleEdit
{
    public partial class PreferencesGeneralController : AppKit.NSViewController
    {
        #region Constructors

        // Called when created from unmanaged code
        public PreferencesGeneralController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public PreferencesGeneralController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public PreferencesGeneralController()
            : base("PreferencesGeneral", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        //strongly typed view accessor
        public new PreferencesGeneral View
        {
            get
            {
                return (PreferencesGeneral)base.View;
            }
        }
    }
}
