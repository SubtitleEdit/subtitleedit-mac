using System;
using AppKit;
using System.Drawing;
using CoreGraphics;

namespace Nikse.SubtitleEdit.UILogic
{
    public static class ColorExtensions
    {
        public static NSColor ToNSColor(this Color c)
        {  
            return NSColor.FromDeviceRgba(c.R / 255.0f, c.G/ 255.0f, c.B/ 255.0f, c.A / 255.0f);
        }

        public static CGColor ToCGColor(this Color c)
        {  
            return new CGColor(c.R / 255.0f, c.G/ 255.0f, c.B/ 255.0f, c.A / 255.0f);
        }

    }
}

