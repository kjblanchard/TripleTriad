﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace E7.Introloop
{
    /// <summary>
    ///     <para>
    ///         Each track is 2 <see cref="AudioSource"/> scheduling to stitch up audio, with accurate scheduling methods.
    ///         Those methods cannot do immediate play, but could do precise play if the start time is in the future.
    ///     </para>
    ///     <para>
    ///         In this class we give reasonable time before the actual audio seam.
    ///         Schedule cannot be canceled, but stay "dormant" while in pause and resume.
    ///         So after coming back we need to reschedule properly.
    ///     </para>
    /// </summary>
    internal class IntroloopTrack : MonoBehaviour
    {
        /// <summary>
        ///     <para>
        ///         Theoretically playing "instantly" is impossible, even though we want it to play "right now"
        ///         on calling <see cref="IntroloopPlayer.Play(E7.Introloop.IntroloopAudio,float,float)"/>
        ///     </para>
        ///     <para>
        ///         So this time is the near-instant future that we told the scheduling method to start.
        ///         With this, it could get better chance that the first loop is accurate at the expense of some waiting time.
        ///     </para>
        ///     <para>
        ///         (But Introloop was never meant to be a low latency solution,
        ///         for that please search for [Native Audio](https://exceed7.com/native-audio/).)
        ///     </para>
        ///     <para>
        ///         Previously, you could randomly get a bit off first loop depending on whether it could fulfill this
        ///         "right now" or not. You notice that usually all loops except the first one are accurate,
        ///         that's because it got time far into the future as a schedule, unlike the first play.
        ///     </para>
        ///     <para>
        ///         The default number `0.02f` was chosen to be a bit more that 16ms,
        ///         the time per 1 frame on 60 FPS.So that it could get through a busy frame.
        ///     </para>
        /// </summary>
        private const float smallPrepareTime = 0.02f;

        private double dspPlusHalfAudio;

        private float fadeVolume;
        private double introFinishDspTime;
        internal IntroloopSettings introloopSettings;

        private bool isPausing;
        private bool isPausingOnIntro;

        // There is one case that isPlaying is not correct, if non-looping music ends,
        // this isPlaying is not updated to "false".
        // In other case, music can only be stopped by user's command so I can set it to false at that moment.
        // This is intentional, since it is not required for anything else and I don't want to
        // waste a Coroutine just to correctly update this.

        //This is used by IntroloopPlayer to check about unloading.

        private int nextSourceToPlay;

        /// <summary>
        ///     The next moment that we need to switch up AudioSource.
        ///     We must schedule way earlier than this to make it in time.
        /// </summary>
        private double nextTransitionTime;

        private double pauseDspTimestamp;

        /// <summary>
        ///     When resume, this variable will add up how long we have paused the audio.
        ///     Used in determining total playtime.
        /// </summary>
        private double pauseDspTotal;

        private double playDspTimestamp;
        private int playingSourceBeforePause;
        private double rememberHowFarFromNextTransitionTime;
        private double[] sourceWillEndTime;
        private double[] sourceWillPlayTime;
        private float startPlayingUnscaledClipTime;

        /* ================================================================================================================
         Note : UNITY (somewhat hidden) AUDIO RULES (Note to self/ to those who that wanted to edit it)
         (This is from experiments from Unity 3.5 era, I am not sure if now the mechanics changed or not)

         1. When Stop(),Pause() the PLAY/STOP schedule that met while stopping will be remembered
            but won't execute UNTIL you call Play().
         2. AudioSettings.dspTime is time-critical. A couple lines of code and call dspTime again, it's different!
         3. According to 1. if you run a new schedule while Stop(), Pause().. the remembered schedule will be.......
         3.1 If STOP schedule is remembered, your newly scheduled PLAY schedule WILL NOT win.
             After your playSchedule execute the stopSchedule will follows. (stopSchedule always comes last?)
         4. AudioListener.pause = true IS THE ONLY WAY to stop AudioSettings.dspTime so no worries when pausing like this :D
         5. [audioSource].isPlaying will becomes TRUE after called PlayScheduled() (even if the schedule is not met yet!)
         6. [audioSource].Stop() does not always reset the [audioSource].time to 0, but instead reset to whatever the last
            [audioSource].time that you set to. (Which is default to 0!) / .Pause() will freeze .time at that instance.
         7. According to 5. and 1. , Stop() and Pause() will override .isPlaying to FALSE, regardless of schedule or remembered
            schedule.
         8. Calling PlaySchedule while pausing can start the AudioSource (not calling any Play()) BUT another PlaySchedule
            that have been met while pausing will not be overridden by new PlaySchedule's time. You must use SetScheduleStartTime
            to reschedule it, alongside with PlaySchedule to start the audio. Using Play() instead of PlaySchedule is untested.

            Awesome blog : https://johnleonardfrench.com/articles/ultimate-guide-to-playscheduled-in-unity/
        ================================================================================================================ */

        private AudioSource[] twoSources;

        internal IntroloopAudio Music { get; private set; }

        internal IntroloopAudio MusicAboutToPlay { get; private set; }

        internal bool IsPlaying { get; private set; }

        /// <summary>
        ///     No matter where you start playing, this is the cumulative play time.
        ///     It stops on pause and it has no maximum or wrap back.
        /// </summary>
        internal float PlayedTimeSecondsUnscaled
        {
            get
            {
                double currentDspPlayhead = 0;
                if (!IsPlaying && !isPausing)
                {
                    return 0;
                }

                if (isPausing || !IsPlaying)
                {
                    currentDspPlayhead = pauseDspTimestamp;
                }
                else
                {
                    currentDspPlayhead = AudioSettings.dspTime;
                }

                return (float) (currentDspPlayhead - playDspTimestamp - pauseDspTotal);
            }
        }

        /// <summary>
        ///     "Play head" is affected by pitch, they move slower with lower pitch.
        /// </summary>
        internal float PlayheadPositionSeconds
        {
            get
            {
                if (Music == null)
                {
                    return 0;
                }

                var playedTime = PlayedTimeSecondsUnscaled;

                // Convert time since played into audio's time. This time we need to know where was that started? Before intro or after?
                // Also it would have "went too fast" if the actual played time was pitched down, so we need to get it to unscaled time.

                if (Music.nonLooping)
                {
                    return Mathf.Min(startPlayingUnscaledClipTime + playedTime * Music.Pitch, Music.UnscaledClipLength);
                }

                if (Music.loopWholeAudio)
                {
                    return (startPlayingUnscaledClipTime + playedTime * Music.Pitch) % Music.UnscaledClipLength;
                }

                var playedUnscaled = startPlayingUnscaledClipTime + playedTime * Music.Pitch;
                var beforeIntro = playedUnscaled < Music.UnscaledIntroLength;
                if (beforeIntro)
                {
                    return playedUnscaled;
                }

                return Music.UnscaledIntroLength +
                       (playedUnscaled - Music.UnscaledIntroLength) % Music.UnscaledLoopingLength;
            }
        }

        public string[] DebugInformation
        {
            get
            {
                return new[]
                {
                    "Play head position :" + PlayheadPositionSeconds.ToString(".00"),
                    "Source 1 Will Play :" + sourceWillPlayTime[0].ToString(".00"),
                    "Source 1 Will End :" + sourceWillEndTime[0].ToString(".00"),
                    "Source 2 Will Play :" + sourceWillPlayTime[1].ToString(".00"),
                    "Source 2 Will End :" + sourceWillEndTime[1].ToString(".00"),
                    "Source 1 : " + (twoSources[0].isPlaying ? "PLAYING/SCHEDULED" : "STOPPED"),
                    "Source 2 : " + (twoSources[1].isPlaying ? "PLAYING/SCHEDULED" : "STOPPED"),
                    "Source 1 Time : " + twoSources[0].time.ToString(".00"),
                    "Source 2 Time : " + twoSources[1].time.ToString(".00"),
                    "Next transition time : " + nextTransitionTime.ToString(".00"),
                    "Dsp plus half audio : " + dspPlusHalfAudio.ToString(".00"),
                    "Is pausing : " + isPausing,
                };
            }
        }

        internal float FadeVolume
        {
            get => fadeVolume;
            set
            {
                var clampedValue = Mathf.Clamp01(value);
                fadeVolume = clampedValue;
                ApplyVolume();
            }
        }

        internal IEnumerable<AudioSource> AllAudioSources
        {
            get
            {
                if (twoSources == null)
                {
                    throw new IntroloopException(
                        "Introloop is not yet initialized. Please avoid accessing internal AudioSource on Awake.");
                }

                return twoSources;
            }
        }

        private void Awake()
        {
            twoSources = new AudioSource[2];
            sourceWillPlayTime = new double[2];
            sourceWillEndTime = new double[2];

            var gameObTransform = gameObject.transform;

            var sourceObject1 = new GameObject("AudioSource 1");
            var as1 = sourceObject1.AddComponent<AudioSource>();
            as1.playOnAwake = false;
            as1.loop = false;
            sourceObject1.transform.parent = gameObTransform;

            twoSources[0] = as1;

            var sourceObject2 = new GameObject("AudioSource 2");
            var as2 = sourceObject2.AddComponent<AudioSource>();
            as2.playOnAwake = false;
            as2.loop = false;
            sourceObject2.transform.parent = gameObTransform;

            twoSources[1] = as2;
        }

        internal void Unload()
        {
            Music.Unload();
            IntroloopLog($"Unloaded \"{Music.AudioClip.name}\" from memory.");
            twoSources[0].clip = null;
            twoSources[1].clip = null;
            Music = null;
            MusicAboutToPlay = null;
        }

        /// <summary>
        ///     Check if it is the time to schedule the next stitch already or not.
        /// </summary>
        internal void SchedulingUpdate()
        {
            if (IsPlaying)
            {
                if (!Music.nonLooping) //In the case of non-looping, no scheduling happen at all.
                {
                    dspPlusHalfAudio = AudioSettings.dspTime + Music.LoopingLength / 2f;
                    if (dspPlusHalfAudio > nextTransitionTime)
                    {
                        //Schedule halfway of looping audio.
                        ScheduleNextLoop();
                    }
                }
            }
        }

        private void ScheduleNextLoop()
        {
            // Note : (nextSourceToPlay + 1) % 2 is not always the same as "currently playing source"
            // even though we have 2 tracks in total, because this "nextSourceToPlay" updates when next loop is "scheduled".

            SetScheduledEndTime((nextSourceToPlay + 1) % 2, nextTransitionTime);
            PlayScheduled(nextSourceToPlay, nextTransitionTime);
            twoSources[nextSourceToPlay].time = Music.UnscaledIntroLength;

            nextTransitionTime = nextTransitionTime + (Music.loopWholeAudio ? Music.ClipLength : Music.LoopingLength);

            nextSourceToPlay = (nextSourceToPlay + 1) % 2;
            //Debug.Log("IntroloopTrack : Next loop scheduled.");
        }

        internal void Stop()
        {
            twoSources[0].Stop();
            twoSources[1].Stop();
            pauseDspTimestamp = AudioSettings.dspTime;

            //This is so that the schedule won't cancel the stop by itself
            IsPlaying = false;
            isPausing = false;
        }

        /// <summary>
        ///     It cannot pause if neither source is playing, since pausing need to determine which one is playing,
        ///     so we could resume scheduling the correct one (while keeping their assigned audio, be it intro part
        ///     or looping part.)
        /// </summary>
        internal bool IsPausable()
        {
            // REMARKS : This method does not work at OnApplicationPause, at that moment all AudioSource
            // will report isPlaying as FALSE regardless of its real status. I think it is 2019.1+ only bug.
            if (!IsPlaying || !twoSources[0].isPlaying && !twoSources[1].isPlaying)
            {
                return false;
            }

            return true;
        }

        internal void Pause()
        {
            if (!IsPlaying)
            {
                return;
            }

            // Note: On 2019.1+, OnApplicationPause, both are FALSE at the moment of game minimizing,
            // So pausing at that moment won't work, since we need it to find out which one is playing.
            // There is a bug that Unity loses all end schedule scheduled on minimize, probably connected to this.

            if (twoSources[0].isPlaying && twoSources[1].isPlaying)
            {
                // Hard case, which one is not actually playing but scheduled?
                // (Scheduled audio reports isPlaying as true)

                if (AudioSettings.dspTime < sourceWillPlayTime[0])
                {
                    playingSourceBeforePause = 1;
                }
                else
                {
                    playingSourceBeforePause = 0;
                }
            }
            else
            {
                // Easy case
                if (twoSources[0].isPlaying)
                {
                    playingSourceBeforePause = 0;
                }
                else
                {
                    playingSourceBeforePause = 1;
                }
            }

            twoSources[0].Pause();
            twoSources[1].Pause();

            var pausingDspTime = AudioSettings.dspTime;
            rememberHowFarFromNextTransitionTime = nextTransitionTime - pausingDspTime;
            pauseDspTimestamp = pausingDspTime;

            if (!Music.nonLooping && !Music.loopWholeAudio) //If contain intro
            {
                if (pausingDspTime >= introFinishDspTime)
                {
                    isPausingOnIntro = false;
                }
                else
                {
                    isPausingOnIntro = true;
                }
            }

            // So the schedule won't cancel the stop by itself
            IsPlaying = false;
            isPausing = true;
            IntroloopLog("\"" + Music.AudioClip.name + "\" paused.");
        }

        internal bool Resume()
        {
            if (!isPausing)
            {
                return false;
            }

            var sourceToContinuePlaying = playingSourceBeforePause;

            // Rescheduling!
            var absoluteTimeNow = AudioSettings.dspTime;

            pauseDspTotal += absoluteTimeNow - pauseDspTimestamp;

            float remainingTime;
            if (!Music.nonLooping && !Music.loopWholeAudio) //If contain intro
            {
                if (isPausingOnIntro)
                {
                    remainingTime = Music.IntroLength - twoSources[sourceToContinuePlaying].time / Music.Pitch;
                }
                else
                {
                    remainingTime = Music.IntroLength + Music.LoopingLength -
                                    twoSources[sourceToContinuePlaying].time / Music.Pitch;
                }
            }
            else
            {
                remainingTime = Music.ClipLength - twoSources[sourceToContinuePlaying].time / Music.Pitch;
            }

            // For current track
            SetScheduledEndTime(sourceToContinuePlaying, absoluteTimeNow + remainingTime); //Intro has no tail!

            // Order does not matter but both must exist.
            SetScheduledStartTime(sourceToContinuePlaying, absoluteTimeNow);
            PlayScheduled(sourceToContinuePlaying, absoluteTimeNow);

            if (!Music.nonLooping)
            {
                // For next track
                SetScheduledStartTime((sourceToContinuePlaying + 1) % 2, absoluteTimeNow + remainingTime);
                PlayScheduled((sourceToContinuePlaying + 1) % 2, absoluteTimeNow + remainingTime);
            }

            if (isPausingOnIntro)
            {
                // For the case of pausing on intro too long (so long that the previously scheduled intro has finished)
                introFinishDspTime = absoluteTimeNow + remainingTime;
            }

            nextTransitionTime = AudioSettings.dspTime + rememberHowFarFromNextTransitionTime;

            IsPlaying = true;
            isPausing = false;
            IntroloopLog("\"" + Music.AudioClip.name + "\" resumed.");
            return true;
        }

        internal void Play(IntroloopAudio music, bool isFade, float startTime)
        {
            pauseDspTimestamp = 0;
            pauseDspTotal = 0;

            twoSources[0].pitch = music.Pitch;
            twoSources[1].pitch = music.Pitch;

            var loadState = music.AudioClip.loadState;
            var musicName = music.AudioClip.name;
            FadeVolume = isFade ? 0 : 1;
            if (loadState != AudioDataLoadState.Loaded)
            {
                IntroloopLog("\"" + musicName + "\" not loaded yet. Loading into memory...");
                StartCoroutine(LoadAndPlayRoutine(music, startTime));
            }
            else
            {
                IntroloopLog("\"" + musicName + "\" already loaded in memory.");
                SetupPlayingSchedule(music, startTime);
            }
        }

        private IEnumerator LoadAndPlayRoutine(IntroloopAudio music, float startTime)
        {
            var musicName = music.AudioClip.name;
            var startLoadingTime = Time.unscaledTime;
            music.AudioClip.LoadAudioData();
            while (music.AudioClip.loadState != AudioDataLoadState.Loaded &&
                   music.AudioClip.loadState != AudioDataLoadState.Failed)
            {
                yield return null;
            }

            if (music.AudioClip.loadState == AudioDataLoadState.Loaded)
            {
                var endLoadingTime = Time.unscaledTime;
                if (music.AudioClip.loadInBackground)
                {
                    IntroloopLog(musicName + " loading successful. (Took " + (endLoadingTime - startLoadingTime) +
                                 " seconds loading in background.)");
                }
                else
                {
                    IntroloopLog(musicName + " loading successful.");
                }

                SetupPlayingSchedule(music, startTime);
            }
            else
            {
                IntroloopLogError(musicName + " loading failed!");
            }
        }

        private void SetupPlayingSchedule(IntroloopAudio music, float startTime)
        {
            IntroloopLog("Playing \"" + music.AudioClip.name + "\"");

            MusicAboutToPlay = music;

            MusicAboutToPlay = null;
            Music = music;
            ApplyVolume();
            nextSourceToPlay = 0;
            isPausing = false;

            twoSources[0].clip = music.AudioClip;
            twoSources[1].clip = music.AudioClip;

            // Essential to cancel the Pause
            Stop();

            // It is important to "anchor" this somewhere and not calling AudioSettings.dspTime again later.
            // Because its value changes even in-between lines of code.
            var dspTimeNow = AudioSettings.dspTime + smallPrepareTime;

            if (music.nonLooping)
            {
                if (startTime < music.UnscaledClipLength)
                {
                    twoSources[0].time = startTime;
                    twoSources[1].time = 0;

                    // Note : you cannot just pull the value back from `twoSources[0].time`. The setter is not instantaneous!
                    startPlayingUnscaledClipTime = startTime;

                    PlayScheduled(0, dspTimeNow);
                }
                else
                {
                    // Do not **actually play** if specified time overshoot.
                    // It could produce this error :
                    // `./Modules/Audio/Public/sound/SoundChannel.cpp(341) : Error executing result (An invalid seek position was passed to this function. )`
                    // The status turned into "play", but it is as if it had already finished.

                    twoSources[0].time = 0;
                    twoSources[1].time = 0;
                    startPlayingUnscaledClipTime = music.UnscaledClipLength;
                }

                IntroloopLog("Type : Non-looping");
            }
            else if (music.loopWholeAudio)
            {
                // This is just a simple loop at the end, but achieved with 2 sources in Introloop style.
                // Yes it is an overkill.. but to streamline the process.
                // (Also when in some case Unity 1-source loop is not seamless, you could try this)

                // PlayScheduled does not reset the playhead!

                var loopedStartTime = startTime % music.UnscaledClipLength;
                twoSources[0].time = loopedStartTime;
                // Always wait at the beginning regardless of intro boundary set.
                twoSources[1].time = 0;

                // For end time we need to scale, but also minus out the start time from the clip time, that is scaled too.
                var dspEndMusicTime = dspTimeNow + music.ClipLength - startTime / music.Pitch;
                PlayScheduled(0, dspTimeNow);
                SetScheduledEndTime(0, dspEndMusicTime);
                introFinishDspTime = dspEndMusicTime;

                PlayScheduled(1, dspEndMusicTime);
                nextTransitionTime = dspEndMusicTime + music.ClipLength;
                startPlayingUnscaledClipTime = loopedStartTime;

                IntroloopLog("Type : Loop whole audio");
            }
            else
            {
                var beforeIntro = startTime < music.UnscaledIntroLength;

                if (beforeIntro)
                {
                    // This is affected by pitch, since it will be used on pause & stuff that happen after play.
                    // Dont forget to minus out the start time that take up the intro length.
                    var dspIntroSeamTime = dspTimeNow + music.IntroLength - startTime / music.Pitch;

                    // The start time is in "play head time", we make it into regular AudioClip time.
                    twoSources[0].time = startTime;

                    // The 2nd source will wait at the intro part so it will go looping. 
                    twoSources[1].time = music.UnscaledIntroLength;

                    PlayScheduled(0, dspTimeNow);
                    SetScheduledEndTime(0, dspIntroSeamTime);
                    introFinishDspTime = dspIntroSeamTime;

                    PlayScheduled(1, dspIntroSeamTime);
                    nextTransitionTime = dspIntroSeamTime + music.LoopingLength;
                    startPlayingUnscaledClipTime = startTime;
                }
                else
                {
                    // The start time is still the "elapsed looping time", we make it into
                    // regular AudioClip "play head" time. Make sure it never overshoot.
                    var introloopedStartTime = music.UnscaledIntroLength +
                                               (startTime - music.UnscaledIntroLength) % music.UnscaledLoopingLength;

                    // This is for the AudioClip starting time where it has no idea about the pitch.
                    var unscaledTimeIntoTheLoop = introloopedStartTime - music.UnscaledIntroLength;
                    var scaledTimeIntoTheLoop = unscaledTimeIntoTheLoop / music.Pitch;
                    var timeRemainingInTheLoop = music.LoopingLength - scaledTimeIntoTheLoop;
                    var dspLoopEndTime = dspTimeNow + timeRemainingInTheLoop;

                    twoSources[0].time = music.UnscaledIntroLength + unscaledTimeIntoTheLoop;
                    // If start time is over intro, the 2nd source must be waiting at the end instead.
                    twoSources[1].time = music.UnscaledIntroLength + music.UnscaledLoopingLength;

                    // This breaks our "schedule automatically half way until the loop point" policy, 
                    // since it is now possible that we start way after that. The solution is to schedule next loop
                    // immediately, and change the state so that it looks like we had played through the intro.
                    PlayScheduled(0, dspTimeNow);

                    nextTransitionTime = dspLoopEndTime;

                    // As if the stitch had occured once.
                    nextSourceToPlay = 1;
                    ScheduleNextLoop();

                    // Intro would have already finished in this case. Time go backwards.
                    // This variable is required to make pause work.
                    introFinishDspTime = dspTimeNow - scaledTimeIntoTheLoop;

                    startPlayingUnscaledClipTime = music.UnscaledIntroLength + unscaledTimeIntoTheLoop;
                    ;
                }

                IntroloopLog("Type : Introloop");
            }

            playDspTimestamp = dspTimeNow;
            pauseDspTimestamp = 0;
            pauseDspTotal = 0;
            IsPlaying = true;
        }

        internal void ApplyVolume()
        {
            if (Music != null)
            {
                twoSources[0].volume = FadeVolume * Music.Volume;
                twoSources[1].volume = FadeVolume * Music.Volume;
            }
        }

        private void PlayScheduled(int sourceNumber, double absoluteTime)
        {
            //Debug.Log("Source " +  sourceNumber + " play at " + absoluteTime);
            twoSources[sourceNumber].PlayScheduled(absoluteTime);
            sourceWillPlayTime[sourceNumber] = absoluteTime;
        }

        private void SetScheduledEndTime(int sourceNumber, double absoluteTime)
        {
            twoSources[sourceNumber].SetScheduledEndTime(absoluteTime);
            sourceWillEndTime[sourceNumber] = absoluteTime;
        }

        private void SetScheduledStartTime(int sourceNumber, double absoluteTime)
        {
            twoSources[sourceNumber].SetScheduledStartTime(absoluteTime);
            sourceWillPlayTime[sourceNumber] = absoluteTime;
        }

        internal void IntroloopLog(string logMessage)
        {
            if (introloopSettings.LogInformation)
            {
                Debug.Log("[Introloop - " + gameObject.name + "] " + logMessage);
            }
        }

        internal void IntroloopLogError(string logMessage)
        {
            if (introloopSettings.LogInformation)
            {
                Debug.Log("<color=red>[Introloop - " + gameObject.name + "]</color> " + logMessage);
            }
        }
    }
}