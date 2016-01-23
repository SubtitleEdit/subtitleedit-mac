using System;

namespace System.Drawing
{
    public struct Size
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public Size (int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}


