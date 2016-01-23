using System;
using AppKit;

namespace UILogic
{

    public class AudioVisMouseEventArgs : EventArgs
    {
        public NSEvent MouseEvent { get; private set; }

        public object Sender { get; private set; }

        public AudioVisMouseEventArgs(object sender, NSEvent e)
        {
            Sender = sender;
            MouseEvent = e;
        }
    }

    public class AudioVisualizerView : NSImageView
    {

        private long _lastMouseDownTicks = -1;
        private long _last2MouseDownTicks = -1;
        private long _lastMouseUpTicks = -1;
        private NSEvent _lastMouseUpEvent;

        public delegate void AudioVisMouseEventHandler(object sender,NSEvent e);

        public event AudioVisMouseEventHandler OnMouseClicked;
        public event AudioVisMouseEventHandler OnMouseDoubleClicked;
        public event AudioVisMouseEventHandler OnMouseMoved;
        public event AudioVisMouseEventHandler OnMouseDraged;
        public event AudioVisMouseEventHandler OnMouseWheel;
        public event AudioVisMouseEventHandler OnMouseUp;
        public event AudioVisMouseEventHandler OnMouseDown;
        public event AudioVisMouseEventHandler OnMouseEnter;
        public event AudioVisMouseEventHandler OnMouseLeave;

        public System.Timers.Timer _clickTimer;

        NSTrackingArea _trackingArea;


        void InitializeClickTimer()
        {
            _clickTimer = new System.Timers.Timer(200);
            _clickTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                _clickTimer.Stop();
                if (_lastMouseUpTicks - _last2MouseDownTicks < TimeSpan.TicksPerMillisecond * 500)
                {                        
                    System.Diagnostics.Debug.WriteLine("Mouse double click");
                    if (OnMouseDoubleClicked != null)
                    {
                        InvokeOnMainThread(() =>
                            {
                                OnMouseDoubleClicked(this, _lastMouseUpEvent);
                            });
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Mouse click");
                    if (OnMouseClicked != null)
                    {
                        InvokeOnMainThread(() =>
                            {
                                OnMouseClicked(this, _lastMouseUpEvent);
                            });
                    }
                }
                ResetMouseClick();
            };
        }

        void ResetMouseClick()
        {
            _lastMouseDownTicks = -1;
            _last2MouseDownTicks = -1;
            _lastMouseUpTicks = -1;
        }

        public override bool AcceptsFirstMouse(NSEvent theEvent)
        {
            return true;
        }

        public override void ScrollWheel(NSEvent theEvent)
        {
            base.ScrollWheel(theEvent);
            if (OnMouseWheel != null)
            {
                OnMouseWheel.Invoke(this, theEvent);
            }
            ResetMouseClick();
            System.Diagnostics.Debug.WriteLine("Mouse wheel " + theEvent.DeltaY);
        }

        public override bool AcceptsFirstResponder()
        {
            return true;
        }

        public override void UpdateTrackingAreas()
        {
            System.Diagnostics.Debug.WriteLine("Update tracking area");
            base.UpdateTrackingAreas();

            if (_trackingArea != null)
            {
                RemoveTrackingArea(_trackingArea);
            }
            _trackingArea = new NSTrackingArea(Frame, NSTrackingAreaOptions.MouseMoved | NSTrackingAreaOptions.MouseEnteredAndExited | NSTrackingAreaOptions.ActiveAlways, this, null);
            AddTrackingArea(_trackingArea);
            ResetMouseClick();
        }

        public override void MouseDown(NSEvent theEvent)
        {
            System.Diagnostics.Debug.WriteLine("Mouse down");
            base.MouseDown(theEvent);

            _last2MouseDownTicks = _lastMouseDownTicks;
            _lastMouseDownTicks = DateTime.Now.Ticks;

            if (OnMouseDown != null)
            {
                InvokeOnMainThread(() =>
                {
                    OnMouseDown(this, theEvent);
                });
            }
        }

        public override void MouseUp(NSEvent theEvent)
        {
            System.Diagnostics.Debug.WriteLine("Mouse up");
            base.MouseUp(theEvent);

            _lastMouseUpTicks = DateTime.Now.Ticks;
            _lastMouseUpEvent = theEvent;
            if (_lastMouseUpTicks - _lastMouseDownTicks < TimeSpan.TicksPerMillisecond * 800)
            {
                _clickTimer.Start();   
            }
            if (OnMouseUp != null)
            {
                InvokeOnMainThread(() =>
                {
                    OnMouseUp(this, theEvent);
                });
            }
        }

        public override void RightMouseUp(NSEvent theEvent)
        {
            System.Diagnostics.Debug.WriteLine("Mouse up right");
            base.RightMouseUp(theEvent);
        }

        public override void MouseDragged(NSEvent theEvent)
        {
            System.Diagnostics.Debug.WriteLine("Mouse dragged");
            base.MouseDragged(theEvent);

            ResetMouseClick();
            ResetMouseClick();
            if (OnMouseDraged != null)
            {
                InvokeOnMainThread(() =>
                    {
                        OnMouseDraged(this, theEvent);
                    });
            }
        }

        public override void MouseEntered(NSEvent theEvent)
        {
            if (_clickTimer == null)
            {
                InitializeClickTimer();
            }

            base.MouseEntered(theEvent);
            System.Diagnostics.Debug.WriteLine("Mouse enterend");

            ResetMouseClick();
            if (OnMouseEnter != null)
            {
                InvokeOnMainThread(() =>
                    {
                        OnMouseEnter(this, theEvent);
                    });
            }
        }

        public override void MouseExited(NSEvent theEvent)
        {
            base.MouseExited(theEvent);
            System.Diagnostics.Debug.WriteLine("Mouse exited");

            ResetMouseClick();
            if (OnMouseLeave != null)
            {
                InvokeOnMainThread(() =>
                    {
                        OnMouseLeave(this, theEvent);
                    });
            }
        }

        public override void MouseMoved(NSEvent theEvent)
        {
            base.MouseMoved(theEvent);
            System.Diagnostics.Debug.WriteLine("Mouse moved");
            if (OnMouseMoved != null)
            {               
                OnMouseMoved.Invoke(this, theEvent);
            }

            ResetMouseClick();
        }

    }
}

