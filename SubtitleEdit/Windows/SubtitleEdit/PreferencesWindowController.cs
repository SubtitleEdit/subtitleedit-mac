using System;

using Foundation;
using AppKit;

namespace SubtitleEdit
{
    public partial class PreferencesWindowController : NSWindowController
    {

        public bool WasOkPressed { get; set;}

        public PreferencesWindowController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public PreferencesWindowController(NSCoder coder)
            : base(coder)
        {
        }

        public PreferencesWindowController()
            : base("PreferencesWindow")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            WasOkPressed = false;
        }

        public new PreferencesWindow Window
        {
            get { return (PreferencesWindow)base.Window; }
        }

        public void OkPressed()
        {
            WasOkPressed = true;
        }

    }
}
