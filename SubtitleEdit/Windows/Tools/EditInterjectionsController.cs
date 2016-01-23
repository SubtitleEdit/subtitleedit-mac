using System;

using Foundation;
using AppKit;

namespace Tools
{
    public partial class EditInterjectionsController : NSWindowController
    {
        public bool WasOkPressed { get; set;}

        public EditInterjectionsController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public EditInterjectionsController(NSCoder coder)
            : base(coder)
        {
        }

        public EditInterjectionsController()
            : base("EditInterjections")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new EditInterjections Window
        {
            get { return (EditInterjections)base.Window; }
        }

        public void OkPressed()
        {
            WasOkPressed = true;
        }

    }
}
