using System;

using Foundation;
using AppKit;

namespace Tools
{
    public partial class RenumberController : NSWindowController
    {

        public int StartNumber { get; set;}
        public bool WasOkPressed { get; set;}

        public RenumberController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public RenumberController(NSCoder coder)
            : base(coder)
        {
        }

        public RenumberController()
            : base("Renumber")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new Renumber Window
        {
            get { return (Renumber)base.Window; }
        }           

        public void OkPressed(int startNumber)
        {
            WasOkPressed = true;
            StartNumber = startNumber;
        }

    }
}
