using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using VideoPlayer;
using AVFoundation;

namespace AVFoundationPlayer
{
    public partial class AVFPlayerController : AppKit.NSViewController, IVideoPlayer
    {
        private AVAsset _asset;
        private AVPlayerItem _playerItem;
        private AVPlayer _player;
        private AVPlayerLayer _playerLayer;
        private bool _playerLoaded;
        private bool _playImageIsPlay;
        private System.Timers.Timer _positionDisplayTimer;
        private double _lastPosition = -1;
        private NSMyVideoView _videoView;

        #region Constructors

        // Called when created from unmanaged code
        public AVFPlayerController(IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public AVFPlayerController(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public AVFPlayerController()
            : base("AVFPlayer", NSBundle.MainBundle)
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

            View.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable; 
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
                if (_player == null || !_playerLoaded)
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
        public new AVFPlayer View
        {
            get
            {
                return (AVFPlayer)base.View;
            }
        }


        #region IVideoPlayer implementation

        public void Open(string videoFileName)
        {
            _playerLoaded = false;
            if (_player != null)
            {
                try
                {
                    _player.Pause();
                    _player.Dispose();
                    _player = null;
                    foreach (var subView in View.VideoView.Subviews)
                    {
                        subView.RemoveFromSuperview();
                    }
                    foreach (var subLayer in View.VideoView.Layer.Sublayers)
                    {
                        subLayer.RemoveFromSuperLayer();
                    }
                    _playerLayer.Dispose();
                    _playerItem.Dispose();
                    _asset.Dispose();
                }
                catch
                {
                }
            }
            ShowPlayImage(false);
            _asset = AVAsset.FromUrl(NSUrl.FromFilename(videoFileName));
            _playerItem = new AVPlayerItem(_asset);
            _player = new AVPlayer(_playerItem);
            _playerLayer = AVPlayerLayer.FromPlayer(_player);
            _videoView = new NSMyVideoView(new CoreGraphics.CGRect(0, 0, View.VideoView.Frame.Width, View.VideoView.Frame.Width), View.VideoView, _playerLayer);
            _videoView.WantsLayer = true;
            View.VideoView.AddSubview(_videoView);
            View.VideoView.WantsLayer = true;
            _playerLayer.Frame = View.VideoView.Bounds;
            View.VideoView.Layer.AddSublayer(_playerLayer);
           _videoView.ResizeWithCorrectAspectRatio();
            _playerLoaded = true;

        }

        public void Play()
        {
            if (!_playerLoaded || _player == null)
            {
                return;
            }

            _player.Play();
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
            if (!_playerLoaded || _player == null)
            {
                return;
            }

            if (_playImageIsPlay)
            {
                Pause();
            }
            else
            {
                Play();
            }
        }

        public void Stop()
        {
            if (!_playerLoaded || _player == null)
            {
                return;
            }

            _player.Pause();
            Position = 0;
            ShowPlayImage(false);
        }

        public void Pause()
        {
            if (!_playerLoaded || _player == null)
            {
                return;
            }
            _player.Pause();
            ShowPlayImage(false);
        }

        public string Name
        {
            get
            {               
                return "AV Foundation Player";
            }
        }

        public double Position
        {
            get
            {
                if (!_playerLoaded || _player == null)
                {
                    return 0;
                }
                if (_player.CurrentTime.IsIndefinite || _player.CurrentTime.IsInvalid)
                {
                    return 0;
                }
                return _player.CurrentTime.Seconds;
            }
            set
            {
                if (!_playerLoaded || _player == null)
                {
                    return;
                }
                _player.Seek(CoreMedia.CMTime.FromSeconds(value, 1));
            }
        }

        public double Duration
        {            
            get
            {
                if (!_playerLoaded || _asset == null)
                {
                    return 0;
                }
                if (_asset.Duration.IsIndefinite || _asset.Duration.IsInvalid)
                {
                    return 0;
                }
                return _asset.Duration.Seconds;
            }
        }

        public int Volume
        {
            get
            {
                if (!_playerLoaded || _player == null)
                {
                    return 0;
                }
                return (int)(_player.Volume * 100.0);
            }
            set
            {
                if (!_playerLoaded || _player == null)
                {
                    return;
                }
                _player.Volume = (float)(value / 100.0);
            }
        }

        public bool IsPlaying
        {
            get
            {
                if (!_playerLoaded || _player == null)
                {
                    return false;
                }

                return true;
            }
        }

        public void ShowSubtitle(Nikse.SubtitleEdit.Core.Paragraph p)
        {
            View.ShowSubtitle(p);
        }

        public void DisposeVideoPlayer()
        {
            if (!_playerLoaded || _player == null)
            {
                return;
            }
            if (_positionDisplayTimer != null)
            {
                _positionDisplayTimer.Stop();
            }
            _player.Pause();
            _playerLoaded = false;
            _asset = null;
            _playerItem = null;
            _playerLayer = null;
            _player = null;
        }

        #endregion
    }
}
