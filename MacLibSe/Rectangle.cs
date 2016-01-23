using System;

namespace System.Drawing
{
    public struct Rectangle
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int Bottom { get { return Top + Height; } }

        public Rectangle (Point location, Size size)
        {
            Left = location.X;
            Top = location.Y;
            Width = size.Width;
            Height = size.Height;
        }

        public Rectangle (int x, int y, int width, int height)
        {
            Left = x;
            Top = y;
            Width = width;
            Height = height;
        }
    
    }
}

