using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using Nikse.SubtitleEdit.Core;
using VideoPlayer;

namespace VLC
{

    public partial class LibVlcPlayerViewController : AppKit.NSViewController, IVideoPlayer
    {

        private LibVlcDynamic _libVlc;
        private VlcVideoView _videoView;
        private bool _vlcReady = false;
        private System.Timers.Timer _positionDisplayTimer;
        private System.Timers.Timer _updatePlayImageTimer;
        private double _lastPosition = -1;
        private bool _playImageIsPlay = false;

        #region Constructors

        // Called when created from unmanaged code
        public LibVlcPlayerViewController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public LibVlcPlayerViewController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public LibVlcPlayerViewController()
            : base("LibVlcPlayerView", NSBundle.MainBundle)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
            // set background color to black
            View.VideoView.WantsLayer = true;
            View.VideoView.Layer.BackgroundColor = new CoreGraphics.CGColor(0, 0, 0);

            View.PlayPauseButton.Activated += (object sender, EventArgs e) =>
            {
                PlayPause();
            };
            View.StopButton.Activated += (object sender, EventArgs e) =>
            {
                Stop();
            };

            View.ShowSubtitle(null);

           
            View.PositionSlider.Activated += (object sender, EventArgs e) =>
            {
                var pos = View.PositionSlider.DoubleValue;
                var seconds = Duration * pos / 100.0;
  //              System.Diagnostics.Debug.WriteLine("Setting slider pos: " + seconds);
                Position = seconds;
            };                             

            View.VolumeSlider.Activated += (object sender, EventArgs e) =>
            {
                Volume = View.VolumeSlider.IntValue;
            };

            _positionDisplayTimer = new System.Timers.Timer(101); 
            _positionDisplayTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                if (_libVlc == null || !_vlcReady)
                {
                    return;
                }
                InvokeOnMainThread(() =>
                    {
                        var currentPosition = Position;
                        if (currentPosition == _lastPosition)
                        {
                            return;
                        }
                        _lastPosition = currentPosition;
                        View.SetCurrentPosition(currentPosition, Duration);
                    });
            };
            _positionDisplayTimer.Start();            
        }

        #endregion

        //strongly typed view accessor
        public new LibVlcPlayerView View
        {
            get
            {
                return (LibVlcPlayerView)base.View;
            }
        }

        #region IVideoPlayer implementation

        public void Open(string videoFileName)
        {            
            _vlcReady = false;
            if (_libVlc != null)
            {
                DisposeVideoPlayer();
                foreach (var subView in View.VideoView.Subviews)
                {
                    subView.RemoveFromSuperview();
                }
            }

            _videoView = new VlcVideoView(new CoreGraphics.CGRect(0, 0, View.VideoView.Frame.Width, View.VideoView.Frame.Width));
            View.VideoView.AddSubview(_videoView);
            _libVlc = new LibVlcDynamic();
            _libVlc.Initialize(_videoView, videoFileName, VideoLoaded, VideoEnded);
            ShowPlayImage(false);
        }

        internal void VideoEnded(object sender, EventArgs e)
        {
            if (_libVlc == null || !_vlcReady)
            {
                return;
            }

            _libVlc.WaitForReady();
            if (_libVlc == null || !_vlcReady)
            {
                return;
            }
            Pause();
            _libVlc.WaitForReady();
            if (_libVlc == null || !_vlcReady)
            {
                return;
            }
            Pause();
        }

        internal void VideoLoaded(object sender, EventArgs e)
        {
            if (_libVlc == null)
            {
                return;
            }

            var size = _libVlc.GetSize();
            _videoView.Width = (int)Math.Round(size.Width);
            _videoView.Height = (int)Math.Round(size.Height);
            _videoView.ResizeWithCorrectAspectRatio();
            _vlcReady = true;
            _updatePlayImageTimer = new System.Timers.Timer(1000 * 10);
            _updatePlayImageTimer.Elapsed += UpdatePlayImageTimerTick;
            _updatePlayImageTimer.Start();   
            _videoView.ResizeWithCorrectAspectRatio();
        }

        internal void UpdatePlayImageTimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_libVlc == null || !_vlcReady)
            {
                return;
            }
            InvokeOnMainThread(() =>
                {
                    _libVlc.WaitForReady();
                    ShowPlayImage(_libVlc.IsPlaying);
                });            
        }

        public void Play()
        {
            if (!_vlcReady)
            {
                return;
            }

            _libVlc.Play();
            ShowPlayImage(true);
        }

        private void ShowPlayImage(bool isPlaying)
        {
            if (_playImageIsPlay == isPlaying)
            {
                return;
            }
                
            var tmp = View.PlayPauseButton.Image;
            View.PlayPauseButton.Image = View.PlayPauseButton.AlternateImage;
            View.PlayPauseButton.AlternateImage = tmp;
            _playImageIsPlay = !_playImageIsPlay;
        }

        public void PlayPause()
        {
            if (!_vlcReady)
            {
                return;
            }

            _libVlc.TogglePlayPause();
            ShowPlayImage(!_playImageIsPlay);
        }

        public void Stop()
        {
            if (!_vlcReady)
            {
                return;
            }

            _libVlc.Pause();
            _libVlc.Position = 0;
            ShowPlayImage(false);
        }

        public void Pause()
        {
            if (!_vlcReady)
            {
                return;
            }
            _libVlc.Pause();
            ShowPlayImage(false);
        }

        public string Name
        {
            get
            {               
                return "VLC";
            }
        }

        public double Position
        {
            get
            {
                if (!_vlcReady)
                {
                    return 0;
                }
                return _libVlc.Position / 1000.0;
            }
            set
            {
                if (!_vlcReady)
                {
                    return;
                }
                _libVlc.Position = (long)Math.Round(value * 1000.0);
            }
        }

        public double Duration
        {            
            get
            {
                if (!_vlcReady)
                {
                    return 0;
                }
                return _libVlc.Duration / 1000.0;
            }
        }

        public int Volume
        {
            get
            {
                if (!_vlcReady)
                {
                    return 0;
                }
                return (int)_libVlc.Volume;
            }
            set
            {
                if (!_vlcReady)
                {
                    return;
                }
                _libVlc.Volume = value;
            }
        }

        public bool IsPlaying
        {
            get
            {
                if (!_vlcReady)
                {
                    return false;
                }

                return _libVlc.IsPlaying;
            }
        }

        public void ShowSubtitle(Nikse.SubtitleEdit.Core.Paragraph p)
        {
            View.ShowSubtitle(p);
        }

        public void DisposeVideoPlayer()
        {
            _vlcReady = false;
            if (_positionDisplayTimer != null)
            {
                _positionDisplayTimer.Stop();
            }
            if (_updatePlayImageTimer != null)
            {
                _updatePlayImageTimer.Stop();
            }
            if (_libVlc != null)
            {
                _libVlc.DisposeVideoPlayer();
            }
        }

        #endregion
    }
}
