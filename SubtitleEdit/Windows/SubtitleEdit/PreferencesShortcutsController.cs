using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace SubtitleEdit
{
    public partial class PreferencesShortcutsController : AppKit.NSViewController
    {
        #region Constructors

        // Called when created from unmanaged code
        public PreferencesShortcutsController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public PreferencesShortcutsController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public PreferencesShortcutsController()
            : base("PreferencesShortcuts", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
        }

        #endregion

        //strongly typed view accessor
        public new PreferencesShortcuts View
        {
            get
            {
                return (PreferencesShortcuts)base.View;
            }
        }
    }
}
