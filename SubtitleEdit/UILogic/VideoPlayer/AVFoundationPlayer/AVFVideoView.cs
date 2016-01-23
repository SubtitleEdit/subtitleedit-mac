using System;
using AppKit;
using AVFoundation;

namespace AVFoundationPlayer
{
    public class NSMyVideoView : NSView
    {
        NSView _view;
        AVPlayerLayer _layer;

        public NSMyVideoView(CoreGraphics.CGRect rect, NSView view, AVPlayerLayer layer)
            : base(rect)
        {
            AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
            _view = view;
            _layer = layer;
        }

        public override void ResizeSubviewsWithOldSize(CoreGraphics.CGSize oldSize)
        {
            base.ResizeSubviewsWithOldSize(oldSize);
            ResizeWithCorrectAspectRatio();
        }

        public override void ViewDidEndLiveResize()
        {
            base.ViewDidEndLiveResize();
            ResizeWithCorrectAspectRatio();
        }

        internal void ResizeWithCorrectAspectRatio()
        {
            // set background color to black
            Superview.WantsLayer = true;
            Superview.Layer.BackgroundColor = new CoreGraphics.CGColor(0, 0, 0);

            // set correct aspect ratio
            _layer.Frame = _view.Bounds;
        }
    }

}

