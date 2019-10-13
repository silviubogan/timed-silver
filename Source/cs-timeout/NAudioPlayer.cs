using NAudio.Wave;
using System;

namespace cs_timed_silver
{
    // TODO: [VISUAL] let user use timers of more than 24h (TimeSpanPicker)
    public class NAudioPlayer
    {
        // TODO: [VISUAL] client-server, do task on client when timer goes off or notify server.
        internal IWavePlayer waveOutDevice;
        internal MediaFoundationReader audioFileReader;
        internal readonly string DefaultAudioPath;
        internal DataFile MyDataFile;
        //internal WaveChannel32 volumeStream = null;
        //internal bool ShouldBePlaying { get; set; } = false;


        internal string AudioFilePath { get; set; } = "";
        internal bool HasError { get; private set; } = false;


        internal NAudioPlayer(DataFile df) : this()
        {
            MyDataFile = df;
        }

        internal NAudioPlayer()
        {
            DefaultAudioPath = $"{Utils.GetInstallationFolder()}" +
                $"\\chimes.wav";

            Mute = false;
        }

        internal bool PlaySound()
        {
            StopSound();

            IsPlaying = true;
            //ShouldBePlaying = true;

            waveOutDevice = new WaveOutEvent();
            try
            {
                if (AudioFilePath != "")
                {
                    audioFileReader =
                        new MediaFoundationReader(AudioFilePath);
                }
                else
                {
                    audioFileReader =
                        new MediaFoundationReader(DefaultAudioPath);
                }

                ApplyMuteSetting();

                waveOutDevice.PlaybackStopped +=
                    WaveOutDevice_PlaybackStopped;
                waveOutDevice.Init(audioFileReader);
                waveOutDevice.Play();
            }
            catch (Exception)
            {
                HasError = true;
                IsPlaying = false;
                //ShouldBePlaying = false;
                return false;
            }
            finally
            {
            }

            // TODO: use more of the API NAudio: https://github.com/naudio/NAudio/wiki/Playing-an-Audio-File. Build a mini audio player UI and functionality.

            return true;
        }

        /// <summary>
        /// If the resources are being disposed.
        /// </summary>
        protected bool IsBeingStopped = false;

        /// <summary>
        /// Manually stops the playing sound, or does nothing if the sound is not playing.
        /// </summary>
        internal void StopSound()
        {
            if (audioFileReader != null ||
                waveOutDevice != null)
            {
                // if these vars still have valid contents, then the stop of sound is in process
                IsBeingStopped = true;
            }
            else
            {
                // these vars below are anyways set in the other if branch (after `return;` below)
                IsBeingStopped = false;
                justNormalPlaybackEnd = true;
                IsPlaying = false;
                return;
            }

            //ShouldBePlaying = false;
            justNormalPlaybackEnd = false;


            //if (volumeStream != null)
            //    volumeStream.Dispose();
            if (audioFileReader != null)
            {
                audioFileReader.Close();
                audioFileReader.Dispose();
                audioFileReader = null;
            }
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }

            justNormalPlaybackEnd = true;

            IsPlaying = false;
        }

        private bool justNormalPlaybackEnd = true;

        public bool IsPlaying { get; set; } = false;

        private void WaveOutDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (waveOutDevice == null ||
                audioFileReader == null)
            {
                return;
            }

            // I get all the states to loop the sound, excepting IsBeingStopped == false.
            if (justNormalPlaybackEnd && !IsBeingStopped)
            {
                //if (!ShouldBePlaying)
                //{
                //    return;
                //}

                // Repeat:
                waveOutDevice.Init(audioFileReader);
                audioFileReader.CurrentTime = TimeSpan.Zero;
                waveOutDevice.Play();

                // Two lines for forced Repeat:
                //StopSound();
                //PlaySound();
            }
            else
            {
                if (IsBeingStopped)
                {
                    IsBeingStopped = false;
                }
                else
                {
                    //StopSound();
                }
                //ShouldBePlaying = false;
            }
        }

        internal bool Test()
        {
            bool e = PlaySound();
            StopSound();
            return e;
        }

        internal bool _Mute = false;
        /// <summary>
        /// Used for permanent un/mute and also for temporary un/mute.
        /// </summary>
        internal bool Mute
        {
            get
            {
                return _Mute;
            }
            set
            {
                if (_Mute != value)
                {
                    _Mute = value;
                    OnMuteChanged();
                }
            }
        }

        private void OnMuteChanged()
        {
            ApplyMuteSetting();
        }

        internal void ApplyMuteSetting()
        {
            if (audioFileReader != null)
            {
                waveOutDevice.Volume = Mute ? 0f : 1f;
            }
        }
    }
}
