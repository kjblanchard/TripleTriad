using System;
using UnityEngine.Events;

namespace E7.Introloop
{
    /// <summary>
    ///     A <see cref="UnityEvent"/> subclass that could send <see cref="IntroloopAudio"/> to
    ///     dynamically invoke <see cref="IntroloopPlayer.Play(IntroloopAudio)"/> on <see cref="IntroloopAudio"/>.
    /// </summary>
    [Serializable]
    public class IntroloopAudioUnityEvent : UnityEvent<IntroloopAudio>
    {
    }
}