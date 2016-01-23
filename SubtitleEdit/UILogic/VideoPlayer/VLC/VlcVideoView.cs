using System;
using AppKit;

namespace VLC
{
    public class VlcVideoView : NSView
    {
        public VlcVideoView(CoreGraphics.CGRect rect)
            : base(rect)
        {
            AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public override void ViewWillStartLiveResize()
        {
            //   this.Frame = new CoreGraphics.CGRect(0, 0, Superview.Frame.Width, Superview.Frame.Height);
            base.ViewWillStartLiveResize();
        }

        public override void ResizeSubviewsWithOldSize(CoreGraphics.CGSize oldSize)
        {
            base.ResizeSubviewsWithOldSize(oldSize);
            ResizeWithCorrectAspectRatio();
        }

        public override void ViewDidEndLiveResize()
        {
            base.ViewDidEndLiveResize();
            if (Width > 0 && Height > 0)
            {
                ResizeWithCorrectAspectRatio();
            }
            else
            {
                this.Frame = new CoreGraphics.CGRect(0, 0, Superview.Frame.Width, Superview.Frame.Height);
            }               
        }

        internal void ResizeWithCorrectAspectRatio()
        {
            // set background color to black
            Superview.WantsLayer = true;
            Superview.Layer.BackgroundColor = new CoreGraphics.CGColor(0, 0, 0);

            // set correct aspect ratio
            var ar = (double)Width / Height;
            var newWidth = Superview.Frame.Width;
            var newHeight = (int)Math.Round(newWidth / ar);
            double top = (Superview.Frame.Height - newHeight) / 2.0;
            double left = 0;
            if (newHeight > Superview.Frame.Height + 1)
            {
                newHeight = (int)Math.Round(Superview.Frame.Height);
                newWidth = (int)Math.Round(newHeight * ar);
                left = (Superview.Frame.Width - newWidth) / 2.0;
                top = 0;
            }
            this.Frame = new CoreGraphics.CGRect(left, top, newWidth, newHeight);
        }
    }

}

