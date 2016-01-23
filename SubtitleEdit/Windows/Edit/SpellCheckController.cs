using System;

using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;

namespace Edit
{
    public partial class SpellCheckController : NSWindowController
    {

        public bool WasChanged { get; set;}
        private Subtitle _subtitle;

        public SpellCheckController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public SpellCheckController(NSCoder coder)
            : base(coder)
        {
        }

        public SpellCheckController(Subtitle subtitle)
            : base("SpellCheck")
        {
            _subtitle = subtitle;
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new SpellCheck Window
        {
            get { return (SpellCheck)base.Window; }
        }
    }
}
