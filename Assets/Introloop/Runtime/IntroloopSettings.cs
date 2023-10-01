/* 
/// Copyright (c) 2015 Sirawat Pitaksarit, Exceed7 Experiments LP 
/// http://www.exceed7.com/introloop
*/

using System;
using UnityEngine;

namespace E7.Introloop
{
    [Serializable]
    internal class IntroloopSettings
    {
        /// <summary>
        ///     When using <see cref="IntroloopPlayer.Instance"/> or <see cref="IntroloopPlayer{T}.Instance"/>
        ///     for the first time, a new game object in <c>DontDestroyOnLoad</c> scene will
        ///     have its name prefixed with this.
        /// </summary>
        internal const string singletonObjectPrefix = "Singleton-";

        /// <summary>
        ///     Check this in your <see cref="IntroloopPlayer"/> template to log various debug data.
        /// </summary>
        [Tooltip("Check to log various debug data while playing.")]
        [SerializeField] private bool logInformation;

        internal bool LogInformation => logInformation;
    }
}