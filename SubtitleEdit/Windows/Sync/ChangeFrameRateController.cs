using System;

using Foundation;
using AppKit;

namespace Sync
{
    public partial class ChangeFrameRateController : NSWindowController
    {

        public double FromFrameRate { get; set;}
        public double ToFrameRate { get; set;}
        public bool WasOkPressed { get; set;}

        public ChangeFrameRateController(IntPtr handle)
            : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public ChangeFrameRateController(NSCoder coder)
            : base(coder)
        {
        }

        public ChangeFrameRateController()
            : base("ChangeFrameRate")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public new ChangeFrameRate Window
        {
            get { return (ChangeFrameRate)base.Window; }
        }

        public void OkPressed(double toFrameRate, double fromFrameRate)
        {
            WasOkPressed = true;
            FromFrameRate = fromFrameRate;
            ToFrameRate = toFrameRate;
        }

    }
}
