using System;
using AppKit;
using Nikse.SubtitleEdit.Core;

namespace VideoPlayer
{
    public interface IVideoPlayer
    {

        string Name { get; }

        void Open(string videoFileName);

        void Play();
        void Stop();
        void Pause();
        void PlayPause();
        bool IsPlaying { get; }

        double Position { get; set; }
        double Duration { get; }

        int Volume { get; set; }

        void ShowSubtitle(Paragraph p);
        void DisposeVideoPlayer();

        NSView View { get; }
    }
}

