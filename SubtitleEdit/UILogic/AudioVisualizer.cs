using System;
using AppKit;
using Nikse.SubtitleEdit.Core;
using CoreGraphics;
using System.Drawing;
using Nikse.SubtitleEdit.UILogic;
using Foundation;
using CoreText;

namespace UILogic
{
    public class AudioVisualizer : AudioVisualizerBase
    {
        AudioVisualizerView _imageView;
        byte[] _rawData;
        CGColorSpace _colorSpace;
        CGBitmapContext _context;

        public AudioVisualizer(AudioVisualizerView imageView)
        {
            _imageView = imageView;

            _imageView.OnMouseClicked += (object sender, NSEvent e) =>
            {
                bool controlDown = (AppKit.NSEventModifierMask.ControlKeyMask & NSEvent.CurrentModifierFlags) > 0;
                bool altDown = (AppKit.NSEventModifierMask.AlternateKeyMask & NSEvent.CurrentModifierFlags) > 0;
                bool shiftDown = (AppKit.NSEventModifierMask.ShiftKeyMask & NSEvent.CurrentModifierFlags) > 0;

                var pos = _imageView.ConvertPointFromView(e.LocationInWindow, _imageView.Window.ContentView);
                WaveformMouseClick(sender, new Point((int)Math.Round(pos.X), (int)Math.Round(pos.Y)), true, shiftDown, controlDown, altDown);
            };

            _imageView.OnMouseDoubleClicked += (object sender, NSEvent e) =>
            {
                var pos = _imageView.ConvertPointFromView(e.LocationInWindow, _imageView.Window.ContentView);
                WaveformMouseDoubleClick(sender, new Point((int)Math.Round(pos.X), (int)Math.Round(pos.Y)), true);
                                                
            };

            _imageView.OnMouseDown += (object sender, NSEvent e) => 
                {
                    var pos = _imageView.ConvertPointFromView(e.LocationInWindow, _imageView.Window.ContentView);
                    WaveformMouseDown(sender, new Point((int)Math.Round(pos.X), (int)Math.Round(pos.Y)), true, false); // e.ButtonMask & NSEventButtonMask.);
                };

            _imageView.OnMouseEnter += (object sender, NSEvent e) =>
            {
                if (_wavePeaks == null)
                    return;
                if (_noClear)
                {
                    _noClear = false;
                }
                else
                {
                    _imageView.AddCursorRect(_imageView.Bounds, NSCursor.ArrowCursor);  //Cursor = Cursors.Default;
                    _mouseDown = false;
                    _mouseDownParagraph = null;
                    _mouseMoveStartX = -1;
                    _mouseMoveEndX = -1;
                }
                        
                if (NewSelectionParagraph != null)
                {
                    _mouseMoveStartX = SecondsToXPosition(NewSelectionParagraph.StartTime.TotalSeconds - StartPositionSeconds);
                    _mouseMoveEndX = SecondsToXPosition(NewSelectionParagraph.EndTime.TotalSeconds - StartPositionSeconds);
                }
            };
            
            _imageView.OnMouseLeave += (object sender, NSEvent e) =>
            {
                _mouseDown = false;
            };

            _imageView.OnMouseMoved += (object sender, NSEvent e) =>
            {
                if (_wavePeaks == null)
                {
                    return;
                }

                var pos = _imageView.ConvertPointFromView(e.LocationInWindow, _imageView.Window.ContentView);
                _mouseMoveLastX = (int)(Math.Round(pos.X));
                if (_mouseMoveLastX < 0 || _mouseMoveLastX > Width)
                {
                    return;
                }                             

                double seconds = RelativeXPositionToSeconds(_mouseMoveLastX);
                var milliseconds = (int)(seconds * TimeCode.BaseUnit);
                   
                if (IsParagrapBorderHit(milliseconds, NewSelectionParagraph))
                    _imageView.AddCursorRect(_imageView.Bounds, NSCursor.IBeamCursor); // Cursors.VSplit
                else if (IsParagrapBorderHit(milliseconds, _selectedParagraph) ||
                         IsParagrapBorderHit(milliseconds, _displayableParagraphs))
                {
                    _imageView.AddCursorRect(_imageView.Bounds, NSCursor.IBeamCursor); // VSplit;
                }
                else
                {
                    _imageView.AddCursorRect(_imageView.Bounds, NSCursor.ArrowCursor); 
                }
            };

            _imageView.OnMouseDraged += (object sender, NSEvent e) => 
                {
                    var pos = _imageView.ConvertPointFromView(e.LocationInWindow, _imageView.Window.ContentView);
                    bool onlyAltDown = (AppKit.NSEventModifierMask.AlternateKeyMask == NSEvent.CurrentModifierFlags);
                    WaveformMouseMove(sender, new Point((int)Math.Round(pos.X), (int)Math.Round(pos.Y)), true, onlyAltDown);
                };

            _imageView.OnMouseWheel += (object sender, NSEvent e) =>
            {
                var delta = e.DeltaY / 3.0f;
                if (!MouseWheelScrollUpIsForward)
                    delta = delta * -1;
                if (Locked)
                {
                    FireOnPositonSelected(new ParagraphEventArgs(_currentVideoPositionSeconds + delta, null));
                }
                else
                {
                    StartPositionSeconds += delta; 
                    if (_currentVideoPositionSeconds < StartPositionSeconds || _currentVideoPositionSeconds >= EndPositionSeconds)
                        FireOnPositonSelected(new ParagraphEventArgs(StartPositionSeconds, null));
                }
            };
        }

        #region implemented abstract members of AudioVisualizerBase

        protected override void SetCursorSplit()
        {
            _imageView.AddCursorRect(_imageView.Bounds, NSCursor.IBeamCursor); // Cursors.VSplit
        }

        protected override void SetCursorNormal()
        {
            _imageView.AddCursorRect(_imageView.Bounds, NSCursor.ArrowCursor); 
        }

        protected override void SetCursorHand()
        {
            _imageView.AddCursorRect(_imageView.Bounds, NSCursor.ClosedHandCursor); 
        }

        protected int _width;

        protected override int Width
        {
            get
            {
                return _width; // (int)Math.Round(_imageView.Frame.Width);
            }
        }

        private int _oldWidth = -1;

        protected int _height;

        protected override int Height
        {
            get
            {
                return _height; // (int)Math.Round(_imageView.Frame.Height);
            }
        }

        private int _oldHeight = -1;

        protected override void DrawBackground()
        {
            _context.SetFillColor(new CGColor(0, 0, 0, 1));
            _context.FillRect(new CGRect(0, 0, Width, Height));
        }

        void InitPaint()
        {
            _width = (int)Math.Round(_imageView.Frame.Width);
            _height = (int)Math.Round(_imageView.Frame.Height);
            if (_width == _oldWidth && _height == _oldHeight)
            {
                return;
            }

            _oldWidth = _width;
            _oldHeight = _height;
            _rawData = new byte[Width * Height * 4];
            var bytesPerPixel = 4;
            var bitsPerComponent = 8;
            var bytesPerRow = bytesPerPixel * Width;
            if (_colorSpace == null)
            {
                _colorSpace = CGColorSpace.CreateDeviceRGB();
            }
            if (_context != null)
            {
                _context.Dispose(); 
            }
            _context = new CGBitmapContext(_rawData, Width, Height, bitsPerComponent, bytesPerRow, _colorSpace, CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.PremultipliedLast);                    
        }

        protected override void DrawGridLines(int imageHeight)
        {
            _context.SetFillColor(GridColor.ToCGColor());
            _context.SetStrokeColor(GridColor.ToCGColor());            
            if (_wavePeaks == null)
            {              
                for (int i = 0; i < Width; i += 10)
                {
                    DrawLine(i, 0, i, imageHeight);
                    DrawLine(0, i, Width, i);                    
                }
            }
            else
            {
                double interval = ZoomFactor >= 0.4 ?
                        0.1 * _wavePeaks.SampleRate * _zoomFactor : // a pixel is 0.1 second
                        1.0 * _wavePeaks.SampleRate * _zoomFactor;  // a pixel is 1.0 second

                for (double i = SecondsToXPosition(StartPositionSeconds) % ((int)Math.Round(interval)); i < Width; i += interval)
                {
                    var j = (int)Math.Round(i);
                    //    graphics.DrawLine(pen, j, 0, j, imageHeight);
                    DrawLine(j, 0, j, imageHeight);

                }
                for (double i = 0; i < imageHeight; i += interval)
                {
                    var j = (int)Math.Round(i);
                    DrawLine(0, j, Width, j);
                    //  graphics.DrawLine(pen, 0, j, Width, j);
                }
            }
        }

        private void DrawLine(int x, int y, int x2, int y2)
        {
            var path = new CGPath();
            path.AddLines(new CGPoint[] { new CGPoint(x, y), new CGPoint(x2, y2) });
            path.CloseSubpath();
            _context.AddPath(path);       
            _context.DrawPath(CGPathDrawingMode.Stroke);
        }

        protected override void DrawTimeLine(int imageHeight)
        {
            double seconds = Math.Ceiling(StartPositionSeconds) - StartPositionSeconds;
            int position = SecondsToXPosition(seconds);
            var textColor = TextColor.ToCGColor();

//               using (var pen = new Pen(TextColor))
//               using (var textBrush = new SolidBrush(TextColor))
//               using (var textFont = new Font(Font.FontFamily, 7))
//               {
            while (position < Width)
            {
                var n = _zoomFactor * _wavePeaks.SampleRate;
                if (n > 38 || (int)Math.Round(StartPositionSeconds + seconds) % 5 == 0)
                {
                    //     graphics.DrawLine(pen, position, imageHeight, position, imageHeight - 10);
                    _context.SetStrokeColor(textColor);
                    DrawLine(position, imageHeight, position, imageHeight - 10);

                    //    graphics.DrawString(GetDisplayTime(StartPositionSeconds + seconds), textFont, textBrush, new PointF(position + 2, imageHeight - 13));
                    _context.SelectFont("Arial", 7f, CGTextEncoding.MacRoman); 
                    _context.SetTextDrawingMode(CGTextDrawingMode.FillStroke);
                    _context.ShowTextAtPoint(position + 2, imageHeight - 13, GetDisplayTime(StartPositionSeconds + seconds));                                       
                }
           
                seconds += 0.5;
                position = SecondsToXPosition(seconds);
           
                if (n > 64)
                {
//                        graphics.DrawLine(pen, position, imageHeight, position, imageHeight - 5);
                    _context.SetStrokeColor(textColor);
                    DrawLine(position, imageHeight, position, imageHeight - 5);
                }
           
                seconds += 0.5;
                position = SecondsToXPosition(seconds);
            }
            //}
        }

        protected override void DrawParagraph(Paragraph paragraph)
        {
            int currentRegionLeft = SecondsToXPosition(paragraph.StartTime.TotalSeconds - StartPositionSeconds);
            int currentRegionRight = SecondsToXPosition(paragraph.EndTime.TotalSeconds - StartPositionSeconds);
            int currentRegionWidth = currentRegionRight - currentRegionLeft;
        
            // background
            _context.SetFillColor(new CGColor(1, 1, 1, (System.nfloat)0.2));
            _context.FillRect(new CGRect(currentRegionLeft, 0, currentRegionWidth, Height));
            //            using (var brush = new SolidBrush(Color.FromArgb(42, 255, 255, 255)))
            //                graphics.FillRectangle(brush, currentRegionLeft, 0, currentRegionWidth, graphics.VisibleClipBounds.Height);
            //


            // left edge
            _context.SetStrokeColor(Color.FromArgb(175, 0, 100, 0).ToCGColor());
            DrawLine(currentRegionLeft, 0, currentRegionLeft, Height);
            //            using (var pen = new Pen(new SolidBrush(Color.FromArgb(175, 0, 100, 0))) { DashStyle = DashStyle.Solid, Width = 2 })
            //                graphics.DrawLine(pen, currentRegionLeft, 0, currentRegionLeft, graphics.VisibleClipBounds.Height);
            //

            // right edge
            _context.SetStrokeColor(Color.FromArgb(175, 110, 10, 10).ToCGColor());
            DrawLine(currentRegionRight - 1, 0, currentRegionRight - 1, Height);
            //            using (var pen = new Pen(new SolidBrush(Color.FromArgb(175, 110, 10, 10))) { DashStyle = DashStyle.Dash, Width = 2 })
            //                graphics.DrawLine(pen, currentRegionRight - 1, 0, currentRegionRight - 1, graphics.VisibleClipBounds.Height);

            //            using (var font = new Font(Configuration.Settings.General.SubtitleFontName, TextSize, TextBold ? FontStyle.Bold : FontStyle.Regular))
            //            using (var textBrush = new SolidBrush(TextColor))
            //            using (var outlineBrush = new SolidBrush(Color.Black))
            //            {
            //                Action<string, int, int> drawStringOutlined = (text, x, y) =>
            //                    {
            //                        // poor mans outline + text
            //                        graphics.DrawString(text, font, outlineBrush, new PointF(x, y - 1));
            //                        graphics.DrawString(text, font, outlineBrush, new PointF(x, y + 1));
            //                        graphics.DrawString(text, font, outlineBrush, new PointF(x - 1, y));
            //                        graphics.DrawString(text, font, outlineBrush, new PointF(x + 1, y));
            //                        graphics.DrawString(text, font, textBrush, new PointF(x, y));
            //                    };
            //
            const int padding = 3;
            double n = _zoomFactor * _wavePeaks.SampleRate;
        
            _context.SelectFont("Arial", 12f, CGTextEncoding.MacRoman); 
            _context.SetTextDrawingMode(CGTextDrawingMode.FillStroke);
            _context.SetStrokeColor(TextColor.ToCGColor());

            // paragraph text
            if (n > 80)
            {
                string text = HtmlUtil.RemoveHtmlTags(paragraph.Text, true).Replace(Environment.NewLine, "  ");
//                int removeLength = 1;
//                            while (text.Length > removeLength && graphics.MeasureString(text, font).Width > currentRegionWidth - padding - 1)
//                            {
//                                text = text.Remove(text.Length - removeLength).TrimEnd() + "…";
//                                removeLength = 2;
//                            }
                     var attributedString = new NSAttributedString (text,
                    new CTStringAttributes{
                    ForegroundColorFromContext =  true,
                    Font = new CTFont ("Arial", 12)
                });
                _context.TextPosition = new CGPoint(currentRegionLeft + padding, padding);
                using (var textLine = new CTLine (attributedString)) {
                    textLine.Draw (_context);
                }                    
//                    drawStringOutlined(text, currentRegionLeft + padding, padding);
            }
//        
//                        // paragraph number
            if (n > 25)
            {
                string text = "#" + paragraph.Number + "  " + paragraph.Duration.ToShortDisplayString();
//                            if (n <= 51 || graphics.MeasureString(text, font).Width >= currentRegionWidth - padding - 1)
//                                text = "#" + paragraph.Number;
//                            drawStringOutlined(text, currentRegionLeft + padding, Height - 14 - (int)graphics.MeasureString("#", font).Height);
                _context.ShowTextAtPoint(currentRegionLeft + padding, Height - 14, text);
            }
//                    }
        }

        public override void AudioVisualizerPaint() //object sender, PaintEventArgs e)
        {
            InitPaint();

            if (_wavePeaks != null)
            {
                bool showSpectrogram = IsSpectrogramAvailable && ShowSpectrogram;
                bool showSpectrogramOnly = showSpectrogram && !ShowWaveform;
                int waveformHeight = Height - (showSpectrogram ? SpectrogramDisplayHeight : 0);

                // background
                DrawBackground();

                // grid lines
                if (ShowGridLines && !showSpectrogramOnly)
                {
                    DrawGridLines(waveformHeight);
                }

                // spectrogram
                if (showSpectrogram)
                {
                    DrawSpectrogram();
                }

                // waveform
                if (ShowWaveform)
                {
                    var colorNormal = Color.ToCGColor();
                    var colorSelected = SelectedColor.ToCGColor();
                    var isSelectedHelper = new IsSelectedHelper(_allSelectedParagraphs, _wavePeaks.SampleRate);
                    int baseHeight = (int)(_wavePeaks.HighestPeak / VerticalZoomFactor);
                    int halfWaveformHeight = waveformHeight / 2;
                    Func<float, float> calculateY = (value) =>
                    {
                        float offset = (value / baseHeight) * halfWaveformHeight;
                        if (offset > halfWaveformHeight)
                            offset = halfWaveformHeight;
                        if (offset < -halfWaveformHeight)
                            offset = -halfWaveformHeight;
                        return halfWaveformHeight - offset;
                    };
                    for (int x = 0; x < Width; x++)
                    {
                        float pos = (float)RelativeXPositionToSeconds(x) * _wavePeaks.SampleRate;
                        int pos0 = (int)pos;
                        int pos1 = pos0 + 1;
                        if (pos1 >= _wavePeaks.Peaks.Count)
                            break;
                        float pos1Weight = pos - pos0;
                        float pos0Weight = 1F - pos1Weight;
                        var peak0 = _wavePeaks.Peaks[pos0];
                        var peak1 = _wavePeaks.Peaks[pos1];
                        float max = peak0.Max * pos0Weight + peak1.Max * pos1Weight;
                        float min = peak0.Min * pos0Weight + peak1.Min * pos1Weight;
                        float yMax = calculateY(max);
                        float yMin = Math.Max(calculateY(min), yMax + 0.1F);
                        var c = isSelectedHelper.IsSelected(pos0) ? colorNormal : colorSelected;
                        _context.SetStrokeColor(c);
                        DrawLine(x, (int)Math.Round(yMax), x, (int)Math.Round(yMin));
                    }
                }

                // time line
                if (!showSpectrogramOnly)
                {
                    DrawTimeLine(waveformHeight);
                }

                // scene changes
                //                if (_sceneChanges != null)
                //                {
                //                    foreach (double time in _sceneChanges)
                //                    {
                //                        int pos = SecondsToXPosition(time - StartPositionSeconds);
                //                        if (pos > 0 && pos < Width)
                //                        {
                //                            using (var p = new Pen(Color.AntiqueWhite))
                //                                graphics.DrawLine(p, pos, 0, pos, Height);
                //                        }
                //                    }
                //                }

                // current video position
                if (_currentVideoPositionSeconds > 0)
                {
                    int pos = SecondsToXPosition(_currentVideoPositionSeconds - StartPositionSeconds);
                    if (pos > 0 && pos < Width)
                    {
                        _context.SetStrokeColor(Color.Turquoise.ToCGColor());
                        DrawLine(pos, 0, pos, Height);
//                        using (var p = new Pen(Color.Turquoise))
//                            graphics.DrawLine(p, pos, 0, pos, Height);
                    }
                }

                // paragraphs
                foreach (Paragraph p in _displayableParagraphs)
                {
                    if (p.EndTime.TotalSeconds >= StartPositionSeconds && p.StartTime.TotalSeconds <= EndPositionSeconds)
                    {
                        DrawParagraph(p);
                    }
                }

                // current selection
//                if (NewSelectionParagraph != null)
//                {
//                    int currentRegionLeft = SecondsToXPosition(NewSelectionParagraph.StartTime.TotalSeconds - StartPositionSeconds);
//                    int currentRegionRight = SecondsToXPosition(NewSelectionParagraph.EndTime.TotalSeconds - StartPositionSeconds);
//                    int currentRegionWidth = currentRegionRight - currentRegionLeft;
//                    if (currentRegionRight >= 0 && currentRegionLeft <= Width)
//                    {
//                        using (var brush = new SolidBrush(Color.FromArgb(128, 255, 255, 255)))
//                            graphics.FillRectangle(brush, currentRegionLeft, 0, currentRegionWidth, graphics.VisibleClipBounds.Height);
//
//                        if (currentRegionWidth > 40)
//                        {
//                            using (var brush = new SolidBrush(Color.Turquoise))
//                                graphics.DrawString(string.Format("{0:0.###} {1}", ((double)currentRegionWidth / _wavePeaks.SampleRate / _zoomFactor), Configuration.Settings.Language.Waveform.Seconds), Font, brush, new PointF(currentRegionLeft + 3, Height - 32));
//                        }
//                    }
//                }
            }
            else
            {
                DrawBackground();

                if (ShowGridLines)
                {
                    DrawGridLines(Height);
                }

//                using (var textBrush = new SolidBrush(TextColor))
//                using (var textFont = new Font(Font.FontFamily, 8))
//                {
//                    if (Width > 90)
//                    {
//                        graphics.DrawString(WaveformNotLoadedText, textFont, textBrush, new PointF(Width / 2 - 65, Height / 2 - 10));
//                    }
//                    else
//                    {
//                        using (var stringFormat = new StringFormat(StringFormatFlags.DirectionVertical))
//                            graphics.DrawString(WaveformNotLoadedText, textFont, textBrush, new PointF(1, 10), stringFormat);
//                    }
//                }
            }
//            if (Focused)
//            {
//                using (var p = new Pen(SelectedColor))
//                    graphics.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
//            }
    
            // _imageView.Image = _bitmap.ToNSImage();
            _imageView.Image = new NSImage(_context.ToImage(), new CGSize(Width, Height));
            ;

//            public CGImage ToCGImage()
//            {
//                var rawData = new byte[Width * Height * 4];
//                var bytesPerPixel = 4;
//                var bytesPerRow = bytesPerPixel * Width;
//                var bitsPerComponent = 8;
//                using (var colorSpace = CGColorSpace.CreateDeviceRGB())
//                {
//                    using (var context = new CGBitmapContext(rawData, Width, Height, bitsPerComponent, bytesPerRow, colorSpace, CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.PremultipliedLast))
//                    {
//                        for (int y = 0; y < Height; y++)
//                        {
//                            for (int x = 0; x < Width; x++)
//                            {
//                                Color c = GetPixel(x, y);
//                                var i = bytesPerRow * y + bytesPerPixel * x;
//                                rawData[i + 0] = c.R;
//                                rawData[i + 1] = c.G;
//                                rawData[i + 2] = c.B;
//                                rawData[i + 3] = c.A;
//                            }
//                        }
//                        //context.Flush();
//
//                        context.SetFillColor(new CGColor(1,0,0, 1));
//                        context.FillRect(new CGRect(0,0, 20, 20));
//
//                        context.SetTextDrawingMode(CGTextDrawingMode.Clip);
//                        context.SetFillColor(new CGColor(0,0,0, 1));
//                        context.ShowTextAtPoint(20, 20, "HEJ");
//                        context.SetTextDrawingMode(CGTextDrawingMode.Fill);
//                        context.ShowTextAtPoint(30, 30, "Yo");
//                        return context.ToImage();
//                    }
//                }
//            }


        }



        #endregion
    }
}

