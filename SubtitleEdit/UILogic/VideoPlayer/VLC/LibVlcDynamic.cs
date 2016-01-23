using System;
using System.Runtime.InteropServices;
using AppKit;
using System.Text;
using Nikse.SubtitleEdit.Core;
using System.Timers;

namespace VLC
{
    public class LibVlcDynamic: IDisposable
    {
        public event EventHandler OnVideoLoaded;
        public event EventHandler OnVideoEnded;

        private bool _closing = false;

        private Timer _videoLoadedTimer;
        private Timer _videoEndTimer;

        const string SystemLib = "/usr/lib/libSystem.dylib";
        // http://forums.xamarin.com/discussion/7171/interfacing-with-a-mac-driver-using-dlopen-and-dlsym

        [DllImport(SystemLib)]
        static extern IntPtr dlsym(IntPtr handle, string symbol);

        [DllImport(SystemLib)]
        static extern IntPtr dlopen(string path, int mode);

        [DllImport(SystemLib)]
        static extern int dlclose(IntPtr handle);

        private IntPtr _libVlcDLL;
        private IntPtr _libVlc;
        private IntPtr _mediaPlayer;

        // LibVLC Core - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__core.html
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr libvlc_new(int argc,[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] argv);

        private libvlc_new _libvlc_new;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_release(IntPtr libVlc);

        private libvlc_release _libvlc_release;

        // LibVLC Media - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__media.html
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr libvlc_media_new_path(IntPtr instance,byte[] input);

        private libvlc_media_new_path _libvlc_media_new_path;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_release(IntPtr media);

        private libvlc_media_release _libvlc_media_release;

        // LibVLC media player - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__media__player.html
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr libvlc_media_player_new_from_media(IntPtr media);

        private libvlc_media_player_new_from_media _libvlc_media_player_new_from_media;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_player_play(IntPtr mediaPlayer);

        private libvlc_media_player_play _libvlc_media_player_play;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_player_stop(IntPtr mediaPlayer);

        private libvlc_media_player_stop _libvlc_media_player_stop;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_media_player_is_playing(IntPtr mediaPlayer);

        private libvlc_media_player_is_playing _libvlc_media_player_is_playing;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_media_player_set_pause(IntPtr mediaPlayer,int doPause);

        private libvlc_media_player_set_pause _libvlc_media_player_set_pause;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate Int64 libvlc_media_player_get_time(IntPtr mediaPlayer);

        private libvlc_media_player_get_time _libvlc_media_player_get_time;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_player_set_time(IntPtr mediaPlayer,Int64 position);

        private libvlc_media_player_set_time _libvlc_media_player_set_time;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate byte libvlc_media_player_get_state(IntPtr mediaPlayer);

        private libvlc_media_player_get_state _libvlc_media_player_get_state;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate Int64 libvlc_media_player_get_length(IntPtr mediaPlayer);

        private libvlc_media_player_get_length _libvlc_media_player_get_length;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_media_player_release(IntPtr mediaPlayer);

        private libvlc_media_player_release _libvlc_media_player_release;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_media_player_set_nsobject(IntPtr mediaPlayer,IntPtr drawable);

        private libvlc_media_player_set_nsobject _libvlc_media_player_set_nsobject;


        // LibVLC Video Controls - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__video.html#g8f55326b8b51aecb59d8b8a446c3f118
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_video_get_size(IntPtr mediaPlayer,UInt32 number,out UInt32 x,out UInt32 y);

        private libvlc_video_get_size _libvlc_video_get_size;


        // LibVLC Audio Controls - http://www.videolan.org/developers/vlc/doc/doxygen/html/group__libvlc__audio.html
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int libvlc_audio_get_volume(IntPtr mediaPlayer);

        private libvlc_audio_get_volume _libvlc_audio_get_volume;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void libvlc_audio_set_volume(IntPtr mediaPlayer,int volume);

        private libvlc_audio_set_volume _libvlc_audio_set_volume;


        private object GetDllType(Type type, string name)
        {
            IntPtr address = dlsym(_libVlcDLL, name);
            if (address != IntPtr.Zero)
            {
                return Marshal.GetDelegateForFunctionPointer(address, type);
            }
            return null;
        }

        private void LoadLibVlcDynamic()
        {
            _libvlc_new = (libvlc_new)GetDllType(typeof(libvlc_new), "libvlc_new");
            _libvlc_release = (libvlc_release)GetDllType(typeof(libvlc_release), "libvlc_release");
            _libvlc_media_new_path = (libvlc_media_new_path)GetDllType(typeof(libvlc_media_new_path), "libvlc_media_new_path");
            _libvlc_media_player_new_from_media = (libvlc_media_player_new_from_media)GetDllType(typeof(libvlc_media_player_new_from_media), "libvlc_media_player_new_from_media");
            _libvlc_media_release = (libvlc_media_release)GetDllType(typeof(libvlc_media_release), "libvlc_media_release");
            _libvlc_media_player_play = (libvlc_media_player_play)GetDllType(typeof(libvlc_media_player_play), "libvlc_media_player_play");
            _libvlc_media_player_stop = (libvlc_media_player_stop)GetDllType(typeof(libvlc_media_player_stop), "libvlc_media_player_stop");
            _libvlc_media_player_is_playing = (libvlc_media_player_is_playing)GetDllType(typeof(libvlc_media_player_is_playing), "libvlc_media_player_is_playing");
            _libvlc_media_player_set_pause = (libvlc_media_player_set_pause)GetDllType(typeof(libvlc_media_player_set_pause), "libvlc_media_player_set_pause");
            _libvlc_media_player_get_time = (libvlc_media_player_get_time)GetDllType(typeof(libvlc_media_player_get_time), "libvlc_media_player_get_time");
            _libvlc_media_player_set_time = (libvlc_media_player_set_time)GetDllType(typeof(libvlc_media_player_set_time), "libvlc_media_player_set_time");
            _libvlc_media_player_get_state = (libvlc_media_player_get_state)GetDllType(typeof(libvlc_media_player_get_state), "libvlc_media_player_get_state");
            _libvlc_media_player_get_length = (libvlc_media_player_get_length)GetDllType(typeof(libvlc_media_player_get_length), "libvlc_media_player_get_length");
            _libvlc_media_player_release = (libvlc_media_player_release)GetDllType(typeof(libvlc_media_player_release), "libvlc_media_player_release");
            _libvlc_video_get_size = (libvlc_video_get_size)GetDllType(typeof(libvlc_video_get_size), "libvlc_video_get_size");
            _libvlc_audio_get_volume = (libvlc_audio_get_volume)GetDllType(typeof(libvlc_audio_get_volume), "libvlc_audio_get_volume");
            _libvlc_audio_set_volume = (libvlc_audio_set_volume)GetDllType(typeof(libvlc_audio_set_volume), "libvlc_audio_set_volume");
            _libvlc_media_player_set_nsobject = (libvlc_media_player_set_nsobject)GetDllType(typeof(libvlc_media_player_set_nsobject), "libvlc_media_player_set_nsobject");
        }

        private bool IsAllMethodsLoaded()
        {
            return _libvlc_new != null &&
            _libvlc_release != null &&
            _libvlc_media_new_path != null &&
            _libvlc_media_player_new_from_media != null &&
            _libvlc_media_release != null &&
            _libvlc_media_player_play != null &&
            _libvlc_media_player_stop != null &&
            _libvlc_media_player_is_playing != null &&
            _libvlc_media_player_get_time != null &&
            _libvlc_media_player_set_time != null &&
            _libvlc_media_player_get_state != null &&
            _libvlc_media_player_get_length != null &&
            _libvlc_media_player_release != null;
        }

        public void Play()
        {
            if (_libVlc == IntPtr.Zero || _mediaPlayer == IntPtr.Zero || _closing)
            {
                return;
            }

            _libvlc_media_player_play(_mediaPlayer);
        }

        public void Pause()
        {
            if (_libVlc == IntPtr.Zero || _mediaPlayer == IntPtr.Zero || _closing)
            {
                return;
            }

            _libvlc_media_player_set_pause(_mediaPlayer, 1);
        }

        public void Stop()
        {
            if (_libVlc == IntPtr.Zero || _mediaPlayer == IntPtr.Zero || _closing)
            {
                return;
            }

            _libvlc_media_player_stop(_mediaPlayer);
        }

        public void TogglePlayPause()
        {
            if (_libVlc == IntPtr.Zero || _mediaPlayer == IntPtr.Zero || _closing)
            {
                return;
            }

            if (_libvlc_media_player_is_playing(_mediaPlayer) == 1)
            {
                _libvlc_media_player_set_pause(_mediaPlayer, 1);
            }
            else
            {
                _libvlc_media_player_play(_mediaPlayer);
            }
        }

        public long Position
        {
            get
            { 
                if (_libVlc == IntPtr.Zero || _mediaPlayer == IntPtr.Zero || _closing)
                {
                    return 0;
                }


                return _libvlc_media_player_get_time(_mediaPlayer);
            }
            set
            { 
                if (_libVlc == IntPtr.Zero || _mediaPlayer == IntPtr.Zero || _closing)
                {
                    return;
                }
                    
                _libvlc_media_player_set_time(_mediaPlayer, value);
            }
        }

        public int Volume
        {
            get
            { 
                return _libvlc_audio_get_volume(_mediaPlayer);
            }
            set
            { 
                _libvlc_audio_set_volume(_mediaPlayer, value);
            }
        }

        public long Duration
        {
            get
            { 
                if (_libVlc == IntPtr.Zero || _mediaPlayer == IntPtr.Zero || _closing)
                {
                    return 0;
                }

                return _libvlc_media_player_get_length(_mediaPlayer);
            }
        }

        public bool IsPaused
        {
            get
            {
                const int Paused = 4;
                int state = _libvlc_media_player_get_state(_mediaPlayer);
                return state == Paused;
            }
        }

        public bool IsPlaying
        {
            get
            {
                if (_libVlc == IntPtr.Zero || _mediaPlayer == IntPtr.Zero || _closing)
                {
                    return false;
                }                   

                const int Playing = 3;
                int state = _libvlc_media_player_get_state(_mediaPlayer);
                return state == Playing;
            }
        }

        public CoreGraphics.CGSize GetSize()
        {
            if (_libVlc == IntPtr.Zero || _mediaPlayer == IntPtr.Zero || _closing)
            {
                return new CoreGraphics.CGSize(0, 0);
                ;
            }

            uint width;
            uint height;
            {
                _libvlc_video_get_size(_mediaPlayer, 0, out width, out height);
                return new CoreGraphics.CGSize(width, height);
            }
        }

        private static string GetVlcLibFullPath()
        {            
            string libFile = System.IO.Path.Combine(Configuration.BaseDirectory, "VLC.app/Contents/MacOS/lib/libvlc.dylib");
            if (!System.IO.File.Exists(libFile))
            {
                libFile = "/Applications/VLC.app/Contents/MacOS/lib/libvlc.dylib";
            }
            if (!System.IO.File.Exists(libFile))
            {
                libFile = System.IO.Path.Combine(Configuration.BaseDirectory, "Contents/MacOS/VLC/lib/libvlc.dylib");
            }
            return libFile;
        }

        private static string GetVlcFullPath()
        {            
            string libFile = System.IO.Path.Combine(Configuration.BaseDirectory, "VLC.app/Contents/MacOS/lib/libvlc.dylib");
            if (!System.IO.File.Exists(libFile))
            {
                libFile = "/Applications/VLC.app/Contents/MacOS/lib/libvlc.dylib";
            }
            if (!System.IO.File.Exists(libFile))
            {
                libFile = System.IO.Path.Combine(Configuration.BaseDirectory, "Contents/MacOS/VLC/lib/libvlc.dylib");
            }
            return libFile;
        }


        public static bool IsVlcAvailable()
        {            
            var libFile = GetVlcLibFullPath();
            if (!System.IO.File.Exists(libFile))
            {
                return false;
            }
            try
            {
                var libHandle = dlopen(libFile, 0);
                if (libHandle != IntPtr.Zero)
                {
                    try
                    {
                        dlclose(libHandle);
                    }
                    catch
                    {
                        
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public void Initialize(NSView view, string videoFileName, EventHandler onVideoLoaded, EventHandler onVideoEnded)
        {
            if (string.IsNullOrEmpty(videoFileName))
            {
                return;
            }

            string libFile = GetVlcLibFullPath();
            string pluginPath = System.IO.Directory.GetParent(libFile).Parent.FullName;
            if (!System.IO.File.Exists(libFile))
            {
                throw new Exception("VLC application not found!");   
            }

            Environment.SetEnvironmentVariable("VLC_PLUGIN_PATH", pluginPath);
            var dir = System.IO.Path.GetDirectoryName(pluginPath);
            System.IO.Directory.SetCurrentDirectory(dir);
            _libVlcDLL = dlopen(libFile, 0);
            LoadLibVlcDynamic();
            if (!IsAllMethodsLoaded())
            {
                throw new Exception("Not all methods from libvlc loaded!");   
            }

            string[] initParameters = { "--no-sub-autodetect-file" }; 
            _libVlc = _libvlc_new(initParameters.Length, initParameters); 
            if (_libVlc == IntPtr.Zero)
            {
                throw new Exception("Unable to call 'libvlc_new' - check VLC installation + plugin folder");   
            }

            var bytes = Encoding.UTF8.GetBytes(videoFileName + "\0");
            IntPtr media = _libvlc_media_new_path(_libVlc, bytes);
            _mediaPlayer = _libvlc_media_player_new_from_media(media);
            _libvlc_media_release(media);
            _libvlc_media_player_set_nsobject(_mediaPlayer, view.Handle);
            _libvlc_media_player_play(_mediaPlayer);

            OnVideoLoaded = onVideoLoaded;
            _videoLoadedTimer = new Timer(100);
            _videoLoadedTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                view.InvokeOnMainThread(() =>
                    {
                        if (IsPlaying)
                        {
                            _videoLoadedTimer.Stop();
                            Pause();
                            if (OnVideoLoaded != null)
                            {                                    
                                OnVideoLoaded.Invoke(_mediaPlayer, new EventArgs());
                            }
                        }
                    });
            };
            _videoLoadedTimer.Start();

            OnVideoEnded = onVideoEnded;
            _videoEndTimer = new Timer(250);
            _videoEndTimer.Elapsed += (object sender, ElapsedEventArgs e) =>
            {
                view.InvokeOnMainThread(() =>
                    {                        
                        const int Ended = 6;
                        if (_libVlc == IntPtr.Zero || _mediaPlayer == IntPtr.Zero || _closing)
                        {
                            return;
                        }
                        int state = _libvlc_media_player_get_state(_mediaPlayer);
                        if (state == Ended)
                        {
                            // hack to make sure VLC is in ready state
                            Stop();
                            WaitForReady();
                            if (_libVlc == IntPtr.Zero || _mediaPlayer == IntPtr.Zero || _closing)
                            {
                                return;
                            }
                            Play();
                            Pause();
                            if (OnVideoEnded != null)
                            {
                                OnVideoEnded.Invoke(_mediaPlayer, new EventArgs());
                            }
                        }
                    });
            };
            _videoEndTimer.Start();
        }

        internal void WaitForReady()
        {
            const int Opening = 1;
            const int Buffering = 2;
            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(25);
                if (_closing)
                    return;

                var state = _libvlc_media_player_get_state(_mediaPlayer);
                if (state == Opening || state == Buffering)
                {
                    i++;
                }
                else
                {
                    return;
                }
            }
            while (i < 1000);
        }

        public void DisposeVideoPlayer()
        {
            Dispose();
        }

        private void ReleaseMangedResources()
        {
            try
            {
                if (_videoLoadedTimer != null)
                {
                    _videoLoadedTimer.Stop();
                }
                if (_videoEndTimer != null)
                {
                    _videoEndTimer.Stop();
                }
            }
            catch
            {
            }
        }

        private readonly object LockObject = new object();

        private void ReleaseUnmangedResources()
        {            
            try
            {
                lock (LockObject)
                {
                    if (_mediaPlayer != IntPtr.Zero && _libVlc != IntPtr.Zero)
                    {
                        _libvlc_media_player_stop(_mediaPlayer);
                        _mediaPlayer = IntPtr.Zero;
                    }

                    if (_libvlc_release != null && _libVlc != IntPtr.Zero)
                    {
                        _libvlc_release(_libVlc);
                        _libVlc = IntPtr.Zero;
                    }

                    if (_libVlcDLL != IntPtr.Zero)
                    {
                        dlclose(_libVlcDLL);
                        _libVlcDLL = IntPtr.Zero;
                    }
                }
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            _closing = true;
            ReleaseMangedResources();
            ReleaseUnmangedResources();
        }

    }
}
