/* 
/// Copyright (c) 2015 Sirawat Pitaksarit, Exceed7 Experiments LP 
/// http://www.exceed7.com/introloop
*/

using UnityEngine;
using System.Collections;
using System;

namespace E7.Introloop
{
    public class IntroloopAudio : ScriptableObject
    {
        [SerializeField, Range(0, 1)]
        [Tooltip("Regular AudioClip couldn't alter volume per-audio, but rather must be the task of AudioSource. Introloop controls 4 AudioSources and allows it to control volume as well. This is useful on composing music too, so you could master the song and maximize the volume regardless of genre, and mix it later to reasonable level here.")]
        private float volume;

        [SerializeField, Range(0.1f, 3)]
        [Tooltip("Introloop couldn't change pitch in real time because that will throw off the schedule, however, by pre-specifying the pitch, it is possible to scales the schedule accordingly. The audio stitching will still be on time.")] 
        //Trust me, even with non-realtime pitch change it is major PITA to finally get this working with everything else...
        private float pitch = 1;

#pragma warning disable 0649
        [SerializeField]
        public AudioClip audioClip;
        [SerializeField, PositiveFloat]
        internal float introBoundary;
        [SerializeField, PositiveFloat]
        internal float loopingBoundary;
        [SerializeField]
        internal bool nonLooping;
        [SerializeField]
        internal bool loopWholeAudio;
#pragma warning restore 0649

        public float Volume
        {
            get
            {
                return this.volume;
            }
            set
            {
                this.volume = value;
            }
        }

        internal float Pitch
        {
            get
            {
                return this.pitch;
            }
        }

        internal float IntroLength
        {
            get
            {
                return introBoundary / pitch;
            }
        }

        internal float LoopingLength
        {
            get
            {
                return (loopingBoundary - introBoundary) / pitch;
            }
        }

        public float ClipLength
        {
            get
            {
                return audioClip.length / pitch;
            }
        }

        /// <summary>
        /// This is for timing the seam between intro and looping section instead of IntroLength on start playing.
        /// It intentionally does not get divided by pitch. Unity's audio position is not affected by pitch.
        /// </summary>
        internal float UnscaledClipLength
        {
            get
            {
                return audioClip.length;
            }
        }

        /// <summary>
        /// This is for timing the seam between intro and looping section instead of IntroLength on start playing.
        /// It intentionally does not get divided by pitch. Unity's audio position is not affected by pitch.
        /// </summary>
        internal float UnscaledIntroLength
        {
            get
            {
                return introBoundary;
            }
        }

        /// <summary>
        /// This is for timing the seam between intro and looping section instead of IntroLength on start playing.
        /// It intentionally does not get divided by pitch. Unity's audio position is not affected by pitch.
        /// </summary>
        internal float UnscaledLoopingLength
        {
            get
            {
                return loopingBoundary - introBoundary;
            }
        }

        internal void Preload()
        {
            audioClip.LoadAudioData();
        }

        internal void Unload()
        {
            audioClip.UnloadAudioData();
        }
    }

}