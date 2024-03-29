﻿/* 
/// Copyright (c) 2015 Sirawat Pitaksarit, Exceed7 Experiments LP 
/// http://www.exceed7.com/introloop
*/

using UnityEngine;

namespace E7.Introloop
{
    /// <summary>
    ///     An asset file for use with <see cref="IntroloopPlayer"/>.
    ///     Contains an information where should the audio loop so the player can schedule the seams ahead of time.
    /// </summary>
    [CreateAssetMenu(menuName = "Introloop/Introloop Audio")]
    public class IntroloopAudio : ScriptableObject
    {
        [SerializeField] [Range(0, 1)]
        [Tooltip(
            "Regular AudioClip couldn't alter volume per-audio, but rather must be the task of AudioSource. " +
            "Introloop controls 4 AudioSources and allows it to control volume as well.\n\n" +
            "This is useful on composing music too, so you could master the song and maximize the volume " +
            "regardless of genre, and mix it later to reasonable level here.")]
        private float volume = 1;

        [SerializeField] [Range(0.1f, 3)]
        [Tooltip(
            "Introloop couldn't change pitch in real time because that will throw off the schedule, " +
            "however, by pre-specifying the pitch, it is possible to scales the schedule accordingly. " +
            "The audio stitching will still be on time.")]
        // Trust me, even with non-realtime pitch change it is major PITA to finally
        // get this working with everything else...
        private float pitch = 1;

        [SerializeField] internal AudioClip audioClip;

        [SerializeField] [PositiveFloat] private float introBoundary;

        [SerializeField] [PositiveFloat] private float loopingBoundary;

        [SerializeField] internal bool nonLooping;

        [SerializeField] internal bool loopWholeAudio;

        /// <summary>
        ///     The underlying <see cref="AudioClip"/> that this asset uses.
        /// </summary>
        public AudioClip AudioClip => audioClip;

        /// <summary>
        ///     Length of this audio if played without looping, already taken account of pitch modification
        ///     setup on the asset. (Unity's "pitch" is not a real pitch shifting, it is just a play speed change
        ///     that in turn affects pitch.)
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Together with <see cref="ClipLength"/>, if the audio's <c>Loop</c> is <c>false</c> then you can predict
        ///         that audio should end after that much time passed.
        ///     </para>
        ///     <para>
        ///         This allows you to implement an audio queue in the case of playing non-looping audio that
        ///         wants to follow with something else afterwards.
        ///         (e.g. inn music while sleeping then continue the old ones which is set to introloop.)
        ///     </para>
        ///     <para>
        ///         You may not be expecting something like <c>IsPlaying</c> property to be lacking in <c>IntroloopPlayer</c>.
        ///         Because scheduling methods inside wrecks <c>AudioSource</c> play state that they could not be trusted 100%.
        ///         For instance, a scheduled-to-play audio is already considered "playing" while actually it is not.
        ///     </para>
        /// </remarks>
        public float ClipLength => audioClip.length / pitch;

        /// <summary>
        ///     If the asset is set to either Introloop or Loop button in the editor, this is `true`.
        /// </summary>
        /// <remarks>
        ///     If this is `false`, <see cref="ClipLength"/> can predict when the audio would end after played.
        /// </remarks>
        public bool Loop => !nonLooping;

        /// <summary>
        ///     Set the underlying <see cref="AudioSource"/> to this volume on play.
        /// </summary>
        /// <remarks>
        ///     This allows a per-music volume adjustment. The composer can master/maximize the volume from his DAW
        ///     without worry about game's context. The developer can drop the volume down after importing on their own.
        ///     Resulting in a happy game studio.
        /// </remarks>
        public float Volume
        {
            get => volume;
            set => volume = value;
        }

        /// <summary>
        ///     Read-only pitch settings that you have set on the <see cref="IntroloopAudio"/> asset file.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Introloop does not allow pitch change other than at the asset, since it will wreck the scheduling if
        ///         that happen while things are playing. Scheduling needs predictability to plan ahead of time and
        ///         pitch change will invalidates many assumptions.
        ///     </para>
        ///     <para>
        ///         (For instance, I schedule "halfway ahead of the seam" and that will not work if suddenly
        ///         you decided to change the pitch right in front of the seam.)
        ///     </para>
        ///     <para>
        ///         Also Unity's "pitch" wording in their <see cref="AudioSource"/> is technically incorrect.
        ///         It is rather "speed" that affects pitch in the end. It is not a real pitch shifting.
        ///         The wording here follows what Unity used rather than technical correctness.
        ///     </para>
        /// </remarks>
        public float Pitch => pitch;

        internal float IntroLength => introBoundary / pitch;

        internal float LoopingLength => (loopingBoundary - introBoundary) / pitch;

        /// <summary>
        ///     This is for timing the seam between intro and looping section instead of IntroLength on start playing.
        ///     It intentionally does not get divided by pitch. Unity's audio position is not affected by pitch.
        /// </summary>
        internal float UnscaledClipLength => audioClip.length;

        /// <summary>
        ///     This is for timing the seam between intro and looping section instead of IntroLength on start playing.
        ///     It intentionally does not get divided by pitch. Unity's audio position is not affected by pitch.
        /// </summary>
        internal float UnscaledIntroLength => introBoundary;

        /// <summary>
        ///     This is for timing the seam between intro and looping section instead of IntroLength on start playing.
        ///     It intentionally does not get divided by pitch. Unity's audio position is not affected by pitch.
        /// </summary>
        internal float UnscaledLoopingLength => loopingBoundary - introBoundary;

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