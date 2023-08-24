using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibVLCSharp.Shared;

namespace ICE_HOCKEY_SIMULATOR
{
    public class MacAudioPlayer
    {
        private readonly LibVLC _libVLC;
        private readonly MediaPlayer _mediaPlayer;

        public MacAudioPlayer()
        {
            Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
        }

        public void PlaySound(string fileName)
        {
            var media = new Media(_libVLC, fileName, FromType.FromPath);
            _mediaPlayer.Play(media);
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
        }
    }
}

