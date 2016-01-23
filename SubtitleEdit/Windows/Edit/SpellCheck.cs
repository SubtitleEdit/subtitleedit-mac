using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;

namespace Edit
{
    public partial class SpellCheck : NSWindow
    {
        public SpellCheck(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SpellCheck(NSCoder coder)
            : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.WillClose += (object sender, EventArgs e) =>
            { 
                NSApplication.SharedApplication.StopModal(); 
            };

            var l = Configuration.Settings.Language.SpellCheck;
            Title = l.Title;
        }

    }
}
