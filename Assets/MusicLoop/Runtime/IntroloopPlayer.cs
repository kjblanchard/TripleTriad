/* 
/// Copyright (c) 2015 Sirawat Pitaksarit, Exceed7 Experiments LP 
/// http://www.exceed7.com/introloop
*/

using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace E7.Introloop
{
    /// <summary>
    /// A component that orchestrate 4 <see cref="AudioSource"> together to achieve gapless looping music with intro section.
    /// 
    /// 2 <see cref="AudioSource"> uses scheduling methods to stitch up audio precisely, while the other 2 sources
    /// are there to support cross fading to a new Introloop audio, where there will be a moment when all 4 sources
    /// are playing at the same time.
    /// </summary>
    public class IntroloopPlayer : MonoBehaviour
    {
        private IntroloopTrack[] twoTracks = new IntroloopTrack[2];
        private float[] towardsVolume = new float[2];
        private bool[] willStop = new bool[2];
        private bool[] willPause = new bool[2];
        private float[] fadeLength = new float[2];
        private IntroloopAudio previousPlay;

        protected virtual bool IsIntroloopSubclass { get { return false; } }

        /// <summary>
        /// If you wish to do something that affects all 4 <see cref="AudioSource"> that Introloop utilize at once, do a `foreach` on this property.
        /// You should not use this in `Awake`, as Introloop might still not yet spawn the <see cref="AudioSource">.
        /// </summary>
        public IEnumerable<AudioSource> InternalAudioSources
        {
            get
            {
                if(twoTracks == null)
                {
                    throw new IntroloopException("Child game objects of Introloop player is not yet initialized. Please avoid accessing internal AudioSource on Awake.");
                }

                foreach (AudioSource aSource in twoTracks[0].AllAudioSources)
                {
                    yield return aSource;
                }
                foreach (AudioSource aSource in twoTracks[1].AllAudioSources)
                {
                    yield return aSource;
                }
            }
        }

        private bool staticInstance = false;
        internal bool StaticInstance { get { return staticInstance; } }

        /// <summary>
        /// It will change to 0 on first <see cref="Play">. 0 is the first track.
        /// </summary>
        private int currentTrack = 1;

        /// <summary>
        /// This fade is inaudible, it helps removing loud pop/click when you stop song suddenly.
        /// This is used automatically when you <see cref="Stop">.
        /// If you really don't want this, use <see cref="StopFade(float)"> with 0 second fade length.
        /// </summary>
        private const float popRemovalFadeTime = 0.055f;

#pragma warning disable 0649
        [SerializeField] private IntroloopSettings introloopSettings;
#pragma warning restore 0649

        private static IntroloopPlayer instance;

        /// <summary>
        /// Singleton pattern point of access. Get a central-use instance of <see cref="IntroloopPlayer">.
        /// </summary>
        public static IntroloopPlayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = MakeSingletonInstance<IntroloopPlayer>();
                }
                return instance;
            }
        }

        protected static T MakeSingletonInstance<T>() where T : IntroloopPlayer
        {
            System.Type type = typeof(T);
            string typeString = type.Name;

            //Try loading a template prefab by matching name
            string path = IntroloopSettings.defaultTemplatePathWithoutFileName + typeString;
            GameObject templatePrefab = Resources.Load<GameObject>(path);

            GameObject introloopPlayer;

            //Create a new one in hierarchy, and this one will persist throughout the game/scene too.
            if (templatePrefab != null)
            {
                //Based on prefab
                introloopPlayer = Instantiate(templatePrefab);
                introloopPlayer.name = IntroloopSettings.singletonObjectPrefix + typeString;
            }
            else
            {
                //New game object
                introloopPlayer = new GameObject(typeString);
                introloopPlayer.AddComponent<T>();
            }

            T singletonInstance = introloopPlayer.GetComponent<T>();
            DontDestroyOnLoad(singletonInstance.gameObject);

            singletonInstance.CreateImportantChilds();

            singletonInstance.staticInstance = true;
            singletonInstance.Set2DSpatialBlend();

            return singletonInstance;
        }

        void Awake()
        {
            if(introloopSettings == null)
            {
                introloopSettings = new IntroloopSettings();
            }

            CreateImportantChilds();

            Set3DSpatialBlend(); //For local Introloop.
        }

        private bool importantChildsCreated = false;
        private void CreateImportantChilds()
        {
            if (!importantChildsCreated)
            {
                // These are all the components that make this plugin works. Basically 4 AudioSources with special control script
                // to juggle music file carefully, stop/pause/resume gracefully while Introloop-ing.

                Transform musicPlayerTransform = transform;
                GameObject musicTrack1 = new GameObject();
                musicTrack1.AddComponent<IntroloopTrack>();
                musicTrack1.name = "IntroloopTrack 1";
                musicTrack1.transform.parent = musicPlayerTransform;
                musicTrack1.transform.localPosition = Vector3.zero;
                twoTracks[0] = musicTrack1.GetComponent<IntroloopTrack>();
                twoTracks[0].introloopSettings = this.introloopSettings;

                GameObject musicTrack2 = new GameObject();
                musicTrack2.AddComponent<IntroloopTrack>();
                musicTrack2.name = "IntroloopTrack 2";
                musicTrack2.transform.parent = musicPlayerTransform;
                musicTrack2.transform.localPosition = Vector3.zero;
                twoTracks[1] = musicTrack2.GetComponent<IntroloopTrack>();
                twoTracks[1].introloopSettings = this.introloopSettings;

                SetMixerGroup(introloopSettings.routeToMixerGroup);

                importantChildsCreated = true;
            }
        }

        /// <summary>
        /// You can also call this later to assign a different audio mixer group.
        /// </summary>
        public void SetMixerGroup(AudioMixerGroup audioMixerGroup)
        {
            foreach(AudioSource aSource in InternalAudioSources)
            {
                aSource.outputAudioMixerGroup = audioMixerGroup;
            }
        }

        private void Update()
        {
            FadeUpdate();
            twoTracks[0].SchedulingUpdate();
            twoTracks[1].SchedulingUpdate();
        }

        private void FadeUpdate()
        {
            //For two main tracks
            for (int i = 0; i < 2; i++)
            {
                float towardsVolumeBgmVolumeApplied = towardsVolume[i];
                if (twoTracks[i].FadeVolume != towardsVolumeBgmVolumeApplied)
                {
                    //Handles the fade in/out
                    if (fadeLength[i] == 0)
                    {
                        twoTracks[i].FadeVolume = towardsVolumeBgmVolumeApplied;
                    }
                    else
                    {
                        if (twoTracks[i].FadeVolume > towardsVolumeBgmVolumeApplied)
                        {
                            twoTracks[i].FadeVolume -= Time.unscaledDeltaTime / fadeLength[i];
                            if (twoTracks[i].FadeVolume <= towardsVolumeBgmVolumeApplied)
                            {
                                //Stop the fade
                                twoTracks[i].FadeVolume = towardsVolumeBgmVolumeApplied;
                            }
                        }
                        else
                        {
                            twoTracks[i].FadeVolume += Time.unscaledDeltaTime / fadeLength[i];
                            if (twoTracks[i].FadeVolume >= towardsVolumeBgmVolumeApplied)
                            {
                                //Stop the fade
                                twoTracks[i].FadeVolume = towardsVolumeBgmVolumeApplied;
                            }
                        }
                    }
                    //Stop check
                    if (willStop[i] && twoTracks[i].FadeVolume == 0)
                    {
                        willStop[i] = false;
                        willPause[i] = false;
                        twoTracks[i].Stop();
                        UnloadTrack(i);
                    }
                    //Pause check
                    if (willPause[i] && twoTracks[i].FadeVolume == 0)
                    {
                        willStop[i] = false;
                        willPause[i] = false;
                        twoTracks[i].Pause();
                        //don't unload!
                    }
                }
            }
        }

        private void UnloadTrack(int trackNumber)
        {
            //Have to check if other track is using the music or not?

            //If playing the same song again,
            //the loading of the next song might come earlier, then got immediately unloaded by this.

            //Also check for when using different IntroloopAudio with the same source file.
            //In this case .Music will be not equal, but actually the audioClip inside is the same song.

            //Note that load/unloading has no effect on "Streaming" audio type.

            bool musicEqualCurrent = (twoTracks[trackNumber].Music == twoTracks[(trackNumber + 1) % 2].Music);
            bool clipEqualCurrent = (twoTracks[trackNumber].Music != null && twoTracks[(trackNumber + 1) % 2].Music != null) &&
             (twoTracks[trackNumber].Music.audioClip == twoTracks[(trackNumber + 1) % 2].Music.audioClip);

            //As = AudioSource
            bool isSameSongAsCurrent = musicEqualCurrent || clipEqualCurrent;

            bool musicEqualNext = (twoTracks[trackNumber].Music == twoTracks[(trackNumber + 1) % 2].MusicAboutToPlay);
            bool clipEqualNext = (twoTracks[trackNumber].Music != null && twoTracks[(trackNumber + 1) % 2].MusicAboutToPlay != null) &&
            (twoTracks[trackNumber].Music.audioClip == twoTracks[(trackNumber + 1) % 2].MusicAboutToPlay.audioClip);

            bool isSameSongAsAboutToPlay = musicEqualNext || clipEqualNext;

            bool usingAndPlaying = twoTracks[(trackNumber + 1) % 2].IsPlaying && isSameSongAsCurrent;

            if (!usingAndPlaying && !isSameSongAsAboutToPlay)
            {
                //If not, it is now safe to unload it
                //Debug.Log("Unloading");
                twoTracks[trackNumber].Unload();
            }
        }

        internal void ApplyVolumeSettingToAllTracks()
        {
            twoTracks[0].ApplyVolume();
            twoTracks[1].ApplyVolume();
        }

        /// <summary>
        /// Play an <see cref="IntroloopAudio"> asset. It applies <see cref="IntroloopAudio.Volume"> and <see cref="IntroloopAudio.Pitch"> to the underlying sources.
        /// If an another <see cref="IntroloopAudio"> is playing on this player, it could cross-fade between the two if <paramref name="fadeLengthSeconds"> is provided.
        /// The faded out audio will be unloaded automatically once the fade is finished.
        /// </summary>
        /// <param name="audio"> An <see cref="IntroloopAudio"> asset file to play.</param>
        /// <param name="fadeLengthSeconds">Fade in/out length to use in seconds. If 0, it uses a small pop removal fade time. If negative, it is immediate. The audio will be unloaded only after it had fade out completely.</param>
        /// <param name="startTime">Specify starting point in time instead of start from the beginning. The time you specify here will be converted to "playhead time", Introloop will make the playhead at the point in time as if you had played for this amount of time before starting. Since <see cref="IntroloopAudio"> conceptually has infinite length, any number that is over looping boundary will be wrapped over to the intro boundary in the calculation. (Except that if the audio is non-looping) The time specified here is not taking <see cref="IntroloopAudio.Pitch"> into account. It's an elapsed time as if <see cref="IntroloopAudio.Pitch"> is 1. </param>
        public void Play(IntroloopAudio audio, float fadeLengthSeconds = 0, float startTime = 0)
        {
            //Auto-crossfade old ones. If no fade length specified, a very very small fade will be used to avoid pops/clicks.
            Stop(fadeLengthSeconds);

            int next = (currentTrack + 1) % 2;
            twoTracks[next].Play(audio, fadeLengthSeconds == 0 ? false : true, startTime);
            towardsVolume[next] = 1;
            fadeLength[next] = TranslateFadeLength(fadeLengthSeconds);

            currentTrack = next;
            this.previousPlay = audio;
        }

        /// <summary>
        /// If it is currently playing, you can instantly move the playhead position to anywhere else. If it is not playing, no effect.
        /// 
        /// An internal implementation is not actually a seek, but a completely new <see cref="Play"> 
        /// with the previous <see cref="IntroloopAudio">.
        /// It is handy because it doesn't require you to remember and specify that audio again.
        /// </summary>
        /// <param name="elapsedTime">Introloop will make the playhead at the point in time as if you had played for this amount of time before starting. Since <see cref="IntroloopAudio"> conceptually has infinite length, any number that is over looping boundary will be wrapped over to the intro boundary in the calculation. (Except that if the audio is non-looping) The time specified here is not taking <see cref="IntroloopAudio.Pitch"> into account. It's an elapsed time as if <see cref="IntroloopAudio.Pitch"> is 1. </param>
        public void Seek(float elapsedTime)
        {
            if (twoTracks[currentTrack].IsPlaying)
            {
                twoTracks[currentTrack].Play(previousPlay, isFade: false, startTime: elapsedTime);
                towardsVolume[currentTrack] = 1;
                fadeLength[currentTrack] = 0;
            }
        }

        /// <summary>
        /// Stop the currently playing <see cref="IntroloopAudio"> immediately and unload it from memory.
        /// </summary>
        public void Stop()
        {
            willStop[currentTrack] = false;
            willPause[currentTrack] = false;
            fadeLength[currentTrack] = 0;
            twoTracks[currentTrack].FadeVolume = 0;
            twoTracks[currentTrack].Stop();
            UnloadTrack(currentTrack);
        }

        /// <summary>
        /// Fading out to stop the currently playing <see cref="IntroloopAudio">, and unload it from memory once it is completely faded out.
        /// </summary>
        /// <param name="fadeLengthSeconds">Fade out length to use in seconds.
        /// 0 is a special value that will still apply small pop removal fade time. If negative, this method works like <see cref="Stop"> overload.
        /// </param>
        public void Stop(float fadeLengthSeconds)
        {
            if (fadeLengthSeconds < 0)
            {
                Stop();
            }
            else
            {
                willStop[currentTrack] = true;
                willPause[currentTrack] = false;
                fadeLength[currentTrack] = TranslateFadeLength(fadeLengthSeconds);
                towardsVolume[currentTrack] = 0;
            }
        }

        /// <summary>
        /// Pause the currently playing <see cref="IntroloopAudio"> immediately without unloading,
        /// Call <see cref="Resume(float)"> to continue playing.
        /// </summary>
        public void Pause()
        {
            if (twoTracks[currentTrack].IsPausable())
            {
                willStop[currentTrack] = false;
                willPause[currentTrack] = false;
                fadeLength[currentTrack] = 0;
                twoTracks[currentTrack].FadeVolume = 0;
                twoTracks[currentTrack].Pause();
            }
        }

        /// <summary>
        /// Fading out to pause the currently playing <see cref="IntroloopAudio"> without unloading,
        /// Call <see cref="Resume(float)"> to continue playing.
        /// </summary>
        /// <param name="fadeLengthSeconds">Fade out length to use in seconds.
        /// 0 is a special value that will still apply small pop removal fade time. If negative, this method works like <see cref="Pause"> overload.
        /// </param>
        public void Pause(float fadeLengthSeconds)
        {
            if (twoTracks[currentTrack].IsPausable())
            {
                if (fadeLengthSeconds < 0)
                {
                    Pause();
                }
                else
                {
                    willPause[currentTrack] = true;
                    willStop[currentTrack] = false;
                    fadeLength[currentTrack] = TranslateFadeLength(fadeLengthSeconds); ;
                    towardsVolume[currentTrack] = 0;
                }
            }
        }

        /// <summary>
        /// Resume playing of previously paused (<see cref="Pause(float)">) <see cref="IntroloopAudio">.
        /// </summary>
        /// <param name="fadeLengthSeconds">Fade out length to use in seconds. If 0, it uses a small pop removal fade time. If negative, it is immediate.</param>
        public void Resume(float fadeLengthSeconds = 0)
        {
            if (twoTracks[currentTrack].Resume())
            {
                //Resume success
                willStop[currentTrack] = false;
                willPause[currentTrack] = false;
                towardsVolume[currentTrack] = 1;
                fadeLength[currentTrack] = TranslateFadeLength(fadeLengthSeconds);
            }
        }

        /// <summary>
        /// Zero length is a special value that equals pop removal small fade time.
        /// Negative length is a special value that equals (real) 0.
        /// </summary>
        private float TranslateFadeLength(float fadeLength)
        {
            return fadeLength > 0 ? fadeLength : fadeLength < 0 ? 0 : popRemovalFadeTime;
        }

        /// <summary>
        /// An experimental feature in the case that you really want the audio to start in an instant you call <see cref="Play(IntroloopAudio, float, float)">.
        /// By normally using <see cref="Play(IntroloopAudio, float, float)"> and <see cref="Stop(float)"> it loads the audio the moment you called <see cref="Play(IntroloopAudio, float, float)">. 
        /// Introloop waits for an audio to load before playing with a coroutine. (If you have <see cref="AudioClip.loadInBackground"> in the import settings, otherwise <see cref="Play(IntroloopAudio, float, float)"> will be a blocking call)
        /// Introloop can't guarantee that the playback will be instant but your game can continue while it is loading.
        /// 
        /// By using <see cref="Preload(IntroloopAudio)"> before actually calling <see cref="Play(IntroloopAudio, float, float)"> it will instead be instant. This function is special that even songs with <see cref="AudioClip.loadInBackground"> can be loaded
        /// in a blocking fashion. (You can put <see cref="Play(IntroloopAudio, float, float)"> immediately in the next line expecting a fully loaded audio)
        /// 
        /// However be aware that memory is managed less efficiently in the following case : 
        /// 
        /// Normally Introloop immediately unloads the previous track to minimize memory, but if you use <see cref="Preload(IntroloopAudio)"> then 
        /// did not call <see cref="Play(IntroloopAudio, float, float)"> with the same <see cref="IntroloopAudio"> afterwards, the loaded memory will be unmanaged. 
        /// (Just like if you tick <see cref="AudioClip.preloadAudioData"> on your clip and have them in a hierarchy somewhere, then did not use it.)
        /// 
        /// Does not work with <see cref="AudioClipLoadType.Streaming"> audio loading type.
        /// </summary>
        public void Preload(IntroloopAudio audio)
        {
            audio.Preload();
        }

        /// <summary>
        /// Set a new audio source curve to this <see cref="IntroloopPlayer">. The settings will be propagated to all <see cref="AudioSource"> it uses.
        /// </summary>
        public void SetAudioSourceCurveType(AudioSourceCurveType curveType, AnimationCurve audioCurve)
        {
            foreach (AudioSource aSource in InternalAudioSources)
            {
                aSource.SetCustomCurve(curveType, audioCurve);
            }
        }

        /// <summary>
        /// Make this <see cref="IntroloopPlayer"> audio curve to be like <paramref name="matchTo"> for a specific <see cref="AudioSourceCurveType">
        /// </summary>
        public void MatchAudioSourceCurveType(AudioSourceCurveType curveType, AudioSource matchTo)
        {
            SetAudioSourceCurveType(curveType, matchTo.GetCustomCurve(curveType));
        }

        /// <summary>
        /// Make this <see cref="IntroloopPlayer"> audio curve to be like <paramref name="matchTo"> for all 4 curves.
        /// </summary>
        public void MatchAudioSourceCurveType(AudioSource matchTo)
        {
            MatchAudioSourceCurveType(AudioSourceCurveType.CustomRolloff, matchTo);
            MatchAudioSourceCurveType(AudioSourceCurveType.ReverbZoneMix, matchTo);
            MatchAudioSourceCurveType(AudioSourceCurveType.SpatialBlend, matchTo);
            MatchAudioSourceCurveType(AudioSourceCurveType.Spread, matchTo);
        }

        /// <summary>
        /// Introloop defaults to this setting. You can hear the song fully with respect to volume in <see cref="IntroloopAudio"> file
        /// no matter where your <see cref="AudioListener"> is.
        /// </summary>
        public void Set2DSpatialBlend()
        {
            SetSpatialBlend(0);
        }

        /// <summary>
        /// If you want Introloop to be positional (possibly on local <see cref="IntroloopPlayer"> instance, not the `static` ones)
        /// You could use this function.
        /// </summary>
        public void Set3DSpatialBlend()
        {
            SetSpatialBlend(1);
        }

        /// <summary>
        /// If you want Introloop to be positional (possibly on local <see cref="IntroloopPlayer"> instance, not the `static` ones)
        /// You could use this function.
        /// </summary>
        public void SetSpatialBlend(float spatialBlend)
        {
            foreach (AudioSource aSource in InternalAudioSources)
            {
                aSource.spatialBlend = spatialBlend;
            }
        }

        /// <summary>
        /// This is a time that could decrease when it goes over looping boundary back to intro boundary.
        /// Conceptually Introloop audio has infinite length, so this time is a bit different from normal sense.
        /// 
        /// Think as it as not "elapsed time" but rather the position of the actual playhead, expressed in time as if the pitch is 1.
        /// For example with pitch enabled, the playhead will move slowly, and so the time returned from this method respect that slower playhead.
        /// 
        /// It is usable with <see cref="Play(IntroloopAudio, float)"> as a start time to "restore" the play from remembered time.
        /// </summary>
        public float GetPlayheadTime()
        {
            return twoTracks[currentTrack].PlayheadPositionSeconds;
        }

        /// <summary>
        /// For debugging purpose.
        /// </summary>
        public string[] GetDebugStringsTrack1()
        {
            return twoTracks[0].DebugInformation;
        }

        /// <summary>
        /// For debugging purpose.
        /// </summary>
        public string[] GetDebugStringsTrack2()
        {
            return twoTracks[1].DebugInformation;
        }

#if UNITY_2019_1_OR_NEWER
        private float timeBeforePause;
        /// <summary>
        /// This is a dirty workaround for the bug in 2019.1+ where on game minimize or AudioListener pause,
        /// All ScheduledEndTime will be lost. I confirmed it is not a problem in 2018.4 LTS.
        /// 
        /// The ideal fix is to call Pause just before the game goes to minimize then Resume after we comeback to reschedule.
        /// However at this callback Pause does not work, as all audio are already on its way to pausing.
        /// 
        /// So an another approach is that we will remember the time just before the pause, and the play again
        /// after coming back using that time. The Seek method can be used instead of Play here so you don't have to specify the
        /// previous audio.
        /// 
        /// Please see : https://forum.unity.com/threads/introloop-easily-play-looping-music-with-intro-section-v4-0-0-2019.378370/#post-4793741
        /// Track the case here : https://fogbugz.unity3d.com/default.asp?1151637_4i53coq9v07qctp1
        /// </summary>
        public void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                timeBeforePause = this.GetPlayheadTime();
            }
            else
            {
                this.Seek(timeBeforePause);
            }
        }
#endif

    }
}