using System;
using System.IO;
using System.Drawing;
using AppKit;
using CoreGraphics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace System.Drawing 
{
    public class Bitmap : IDisposable
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public PixelFormat PixelFormat
        {
            get
            { 
                return PixelFormat.Format32bppArgb;
            }
        }

        private Color[] _colors;

        public Bitmap(int width, int height)
        {
            Width = width;
            Height = height;
            _colors = new Color[Width * Height];
        }

        public Color GetPixel(int x, int y)
        {
            return _colors[Width * y + x];
        }

        public void SetPixel(int x, int y, Color c)
        {
            _colors[Width * y + x] = c;
        }

        public NSImage ToNSImage()
        {
            return new NSImage(ToCGImage(), new CGSize(Width, Height));
        }

        public Bitmap(NSImage sourceImage)
        {
            Width = (int)sourceImage.CGImage.Width;
            Height = (int)sourceImage.CGImage.Height;
            _colors = new Color[Width * Height];

            var rawData = new byte[Width * Height * 4];
            var bytesPerPixel = 4;
            var bytesPerRow = bytesPerPixel * Width;
            var bitsPerComponent = 8;

            using (var colorSpace = CGColorSpace.CreateDeviceRGB())
            {
                using (var context = new CGBitmapContext(rawData, Width, Height, bitsPerComponent, bytesPerRow, colorSpace, CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.PremultipliedLast))
                {
                    context.DrawImage(new CGRect(0, 0, Width, Height), sourceImage.CGImage);

                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            var i = bytesPerRow * y + bytesPerPixel * x;   
                            byte red = rawData[i + 0];
                            byte green = rawData[i + 1];
                            byte blue = rawData[i + 2];
                            byte alpha = rawData[i + 3];
                            SetPixel(x, y, Color.FromArgb(alpha, red, green, blue));
                        }
                    }
                }
            }
        }

        public CGImage ToCGImage()
        {
            var rawData = new byte[Width * Height * 4];
            var bytesPerPixel = 4;
            var bytesPerRow = bytesPerPixel * Width;
            var bitsPerComponent = 8;
            using (var colorSpace = CGColorSpace.CreateDeviceRGB())
            {
                using (var context = new CGBitmapContext(rawData, Width, Height, bitsPerComponent, bytesPerRow, colorSpace, CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.PremultipliedLast))
                {
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            Color c = GetPixel(x, y);
                            var i = bytesPerRow * y + bytesPerPixel * x;
                            rawData[i + 0] = c.R;
                            rawData[i + 1] = c.G;
                            rawData[i + 2] = c.B;
                            rawData[i + 3] = c.A;
                        }
                    }
//                    //context.Flush();
//
//                    context.SetFillColor(new CGColor(1,0,0, 1));
//                    context.FillRect(new CGRect(0,0, 20, 20));
//
//                    context.SetTextDrawingMode(CGTextDrawingMode.Clip);
//                    context.SetFillColor(new CGColor(0,0,0, 1));
//                    context.ShowTextAtPoint(20, 20, "HEJ");
//                    context.SetTextDrawingMode(CGTextDrawingMode.Fill);
//                    context.ShowTextAtPoint(30, 30, "Yo");
                    return context.ToImage();
                }
            }
        }

        #region IDisposable implementation
        public void Dispose ()
        {
            _colors = null;
        }
        #endregion

        public Bitmap Clone (Rectangle rectangle, System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            return null; //TODO: Get section...
        }

        public Bitmap Clone ()
        {
            return new Bitmap (ToNSImage ());
        }

    }
}

