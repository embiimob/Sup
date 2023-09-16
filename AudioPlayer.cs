using NAudio.Wave;
using System.Collections.Generic;
using System.Threading;

namespace SUP
{
    //GPT3 ROCKS!
    internal class AudioPlayer
    {
        private Queue<string> playlist;
        private bool isPlaying;
        private object lockObject = new object();
        private WaveOut waveOut;

        public AudioPlayer()
        {
            playlist = new Queue<string>();
            isPlaying = false;
        }

        public void AddToPlaylist(string audioLocation)
        {
            lock (lockObject)
            {
                playlist.Enqueue(audioLocation);
                if (!isPlaying)
                {
                    isPlaying = true;
                    PlayNext();
                }
            }
        }

        public void SkipCurrent()
        {
            lock (lockObject)
            {
                if (waveOut != null)
                {
                    waveOut.Stop();
                    waveOut.Dispose();
                    waveOut = null;
                }

                // Move to the next audio in the playlist
                PlayNext();
            }
        }

        private void PlayNext()
        {
            if (playlist.Count > 0)
            {
                string audioLocation = playlist.Dequeue();

                waveOut = new WaveOut();
                IWaveProvider reader;

                if (audioLocation.ToLower().EndsWith(".wav"))
                {
                    reader = new WaveFileReader(audioLocation);
                }
                else if (audioLocation.ToLower().EndsWith(".mp3"))
                {
                    reader = new Mp3FileReader(audioLocation);
                }
                else
                {
                    // Handle unsupported audio formats
                    PlayNext();
                    return;
                }

                waveOut.Init(reader);
                waveOut.Play();

                waveOut.PlaybackStopped += (sender, args) =>
                {
                    waveOut.Dispose();
                    Thread.Sleep(1000);
                    PlayNext(); // Play the next audio after the current one is finished
                };
            }
            else
            {
                lock (lockObject)
                {
                    isPlaying = false;
                }
            }
        }

    }
}
