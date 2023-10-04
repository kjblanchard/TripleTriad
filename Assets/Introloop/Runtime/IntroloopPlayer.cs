using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace E7.Introloop
{
    /// <summary>
    ///     A component that coordinates 4 <see cref="AudioSource"/> together with scheduling methods
    ///     to achieve gap-less looping music with intro section.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         2 <see cref="AudioSource"/> uses scheduling methods to stitch up audio precisely, while the other 2 sources
    ///         are there to support cross fading to a new Introloop audio.
    ///     </para>
    ///     <para>
    ///         Potentially there is a moment when all 4 sources are playing at the same time.
    ///         (e.g. One introlooping audio at the seam, while being tasked to cross fade into
    ///         an another introloop audio that starts near the seam.)
    ///     </para>
    /// </remarks>
    public class IntroloopPlayer : MonoBehaviour
    {
        /// <summary>
        ///     This fade is inaudible, it helps removing loud pop/click when you stop song suddenly.
        ///     This is used automatically when you <see cref="Stop()"/>.
        ///     If you really don't want this, use <see cref="Stop(float)"/> with 0 second fade length.
        /// </summary>
        private const float popRemovalFadeTime = 0.055f;

        private static IntroloopPlayer instance;

        private static AudioSource singletonInstanceTemplateSource;

        [Tooltip("This works the same as Clips slot on the AudioSource. It is the asset to play when you call Play() " +
                 "in the API. However, Introloop also has Play overload where you can just send an asset to play via " +
                 "the arguments regardless of asset reference connected to this field.")]
        [SerializeField] private IntroloopAudio defaultIntroloopAudio;

        [Tooltip("Introloop spawns 4 AudioSource at runtime to manage the loops. All sources inherits settings from " +
                 "this template AudioSource, including the output AudioMixerGroup. You can add the AudioSource to be " +
                 "the template next to this IntroloopPlayer component then connect to this slot.\n\n" +
                 "If it is not assigned, new AudioSource component will be added next to " +
                 "this component on the first play with settings : Priority 0, Spatial Blend 2D. (Expected settings " +
                 "for background music uses.) Then this field will reference that newly added AudioSource as a template.")]
        [SerializeField] private AudioSource templateSource;

        [Tooltip("Works like Play On Awake of AudioSource, play the connected \"Default Introloop Audio\" asset " +
                 "automatically on Awake().")]
        [SerializeField] private bool playOnAwake;

        [SerializeField] private IntroloopSettings introloopSettings;

        private readonly float[] fadeLength = new float[2];
        private readonly float[] towardsVolume = new float[2];
        private readonly IntroloopTrack[] twoTracks = new IntroloopTrack[2];
        private readonly bool[] willPause = new bool[2];
        private readonly bool[] willStop = new bool[2];

        /// <summary>
        ///     It will change to 0 on first <see cref="Play(E7.Introloop.IntroloopAudio,float,float)"/>.
        ///     0 is the first track.
        /// </summary>
        private int currentTrack = 1;

        private bool importantChildrenCreated;
        private IntroloopAudio previousPlay;

        private float timeBeforePause;


        /// <summary>
        ///     Works like <see cref="AudioSource.clip"/> property. You can set this to any <see cref="IntroloopAudio"/>
        ///     for it to be used when you call <see cref="Play()"/> or <see cref="Play(float,float)"/> next.
        /// </summary>
        public IntroloopAudio DefaultIntroloopAudio
        {
            get => defaultIntroloopAudio;
            set => defaultIntroloopAudio = value;
        }

        /// <summary>
        ///     Works like <see cref="AudioSource.playOnAwake"/>, play the connected <see cref="DefaultIntroloopAudio"/>
        ///     automatically on <c>Awake</c>.
        /// </summary>
        public bool PlayOnAwake
        {
            get => playOnAwake;
            set => playOnAwake = value;
        }

        /// <summary>
        ///     <para>
        ///         When it would spawn 4 <see cref="AudioSource"/> for the first time on <c>Start()</c>, read out the
        ///         fields from this <see cref="AudioSource"/> reference and copy them to all 4. Assigning this after
        ///         it had already spawned underlying 4 <see cref="AudioSource"/> has no effect.
        ///     </para>
        ///     <para>
        ///         To apply this template again after <c>Start()</c>, use <see cref="ApplyAudioSource"/>.
        ///         The argument could be this <see cref="TemplateSource"/> or any other <see cref="AudioSource"/>.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     The <see cref="AudioSource"/> does not need to be <see cref="MonoBehaviour.enabled"/> since it just
        ///     need to read the fields out for copy. Also it does not need to be anywhere on the scene,
        ///     it can come from a prefab with <see cref="AudioSource"/> in your project.
        /// </remarks>
        public AudioSource TemplateSource
        {
            get
            {
                if (templateSource != null)
                {
                    return templateSource;
                }

                // A fallback to pickup nearby AudioSource as a template.
                templateSource = GetComponent<AudioSource>();
                if (templateSource != null)
                {
                    return templateSource;
                }

                // If no template source, make it a high priority + 2D source by default.
                templateSource = gameObject.AddComponent<AudioSource>();
                SetupDefaultAudioSourceForIntroloop(templateSource);

                return templateSource;
            }
            set => templateSource = value;
        }

        /// <summary>
        ///     If you wish to do something that affects all 4 <see cref="AudioSource"/> that Introloop utilize at once,
        ///     do a <c>foreach</c> on this property.
        /// </summary>
        /// <remarks>
        ///     You should not use this in <c>Awake</c>, as Introloop might still
        ///     not yet spawn the <see cref="AudioSource"/>.
        /// </remarks>
        public IEnumerable<AudioSource> InternalAudioSources
        {
            get
            {
                if (twoTracks == null)
                {
                    throw new IntroloopException(
                        "Child game objects of Introloop player is not yet initialized. " +
                        "Please avoid accessing internal AudioSource on Awake.");
                }

                foreach (var aSource in twoTracks[0].AllAudioSources)
                {
                    yield return aSource;
                }

                foreach (var aSource in twoTracks[1].AllAudioSources)
                {
                    yield return aSource;
                }
            }
        }

        /// <summary>
        ///     <para>
        ///         Get a convenient singleton instance of <see cref="IntroloopPlayer"/> from anywhere in your code.
        ///         It has <c>DontDestroyOnLoad</c> applied.
        ///     </para>
        ///     <para>
        ///         Before calling this <b>for the first time</b>, call <see cref="SetSingletonInstanceTemplateSource"/>
        ///         first to setup its <see cref="TemplateSource"/> from script. (It does not exist until runtime, you
        ///         cannot setup the template ahead of time unlike non-singleton instances.)
        ///     </para>
        /// </summary>
        public static IntroloopPlayer Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = InstantiateSingletonPlayer<IntroloopPlayer>(singletonInstanceTemplateSource);
                instance.name = IntroloopSettings.singletonObjectPrefix + instance.name;

                return instance;
            }
        }

        private void Awake()
        {
            if (introloopSettings == null)
            {
                introloopSettings = new IntroloopSettings();
            }

            CreateImportantChildren();
            if (playOnAwake)
            {
                Play();
            }
        }

        private void Start()
        {
            TemplateSource.enabled = false;
            ApplyAudioSource(TemplateSource);
        }

        private void Update()
        {
            FadeUpdate();
            twoTracks[0].SchedulingUpdate();
            twoTracks[1].SchedulingUpdate();
        }

        /// <summary>
        ///     <para>
        ///         This is a dirty workaround for the bug in 2019.1+ where on game minimize or
        ///         <see cref="AudioListener"/> pause, all <see cref="AudioSource.SetScheduledEndTime(double)"/> will be lost.
        ///     </para>
        ///     <para>
        ///         I confirmed it is not a problem in 2018.4 LTS.
        ///         The ideal fix is to call Pause just before the game goes to minimize then
        ///         Resume after we comeback to reschedule.
        ///     </para>
        ///     <para>
        ///         However at this callback Pause does not work, as all audio are already on its way to pausing.
        ///         So an another approach is that we will remember the time just before the pause, and the play again
        ///         after coming back using that time. The Seek method can be used instead of Play here so you don't have to specify the
        ///         previous audio.
        ///     </para>
        ///     <para>
        ///         Please see : https://forum.unity.com/threads/introloop-easily-play-looping-music-with-intro-section-v4-0-0-2019.378370/#post-4793741
        ///         Track the case here : https://fogbugz.unity3d.com/default.asp?1151637_4i53coq9v07qctp1
        ///     </para>
        /// </summary>
        public void OnApplicationPause(bool paused)
        {
            if (paused)
            {
                timeBeforePause = GetPlayheadTime();
            }
            else
            {
                Seek(timeBeforePause);
            }
        }

        private protected static T InstantiateSingletonPlayer<T>(AudioSource templateOfTemplateSource)
            where T : IntroloopPlayer
        {
            var type = typeof(T);
            var typeString = type.Name;

            var newIntroloopPlayerObject = new GameObject(typeString);
            var playerComponent = newIntroloopPlayerObject.AddComponent<T>();
            var templateAudioSource = newIntroloopPlayerObject.AddComponent<AudioSource>();
            playerComponent.TemplateSource = templateOfTemplateSource;
            SetupDefaultAudioSourceForIntroloop(templateAudioSource);
            if (templateOfTemplateSource != null)
            {
                CopyAudioSourceFields(templateAudioSource, templateOfTemplateSource);
            }

            DontDestroyOnLoad(newIntroloopPlayerObject);

            playerComponent.CreateImportantChildren();

            return playerComponent;
        }

        private void CreateImportantChildren()
        {
            if (importantChildrenCreated)
            {
                return;
            }
            // These are all the components that make this plugin works. Basically 4 AudioSources with special control script
            // to juggle music file carefully, stop/pause/resume gracefully while Introloop-ing.

            var musicPlayerTransform = transform;
            var musicTrack1 = new GameObject();
            musicTrack1.AddComponent<IntroloopTrack>();
            musicTrack1.name = "IntroloopTrack 1";
            musicTrack1.transform.parent = musicPlayerTransform;
            musicTrack1.transform.localPosition = Vector3.zero;
            twoTracks[0] = musicTrack1.GetComponent<IntroloopTrack>();
            twoTracks[0].introloopSettings = introloopSettings;

            var musicTrack2 = new GameObject();
            musicTrack2.AddComponent<IntroloopTrack>();
            musicTrack2.name = "IntroloopTrack 2";
            musicTrack2.transform.parent = musicPlayerTransform;
            musicTrack2.transform.localPosition = Vector3.zero;
            twoTracks[1] = musicTrack2.GetComponent<IntroloopTrack>();
            twoTracks[1].introloopSettings = introloopSettings;

            importantChildrenCreated = true;
        }

        private static void SetupDefaultAudioSourceForIntroloop(AudioSource audioSource)
        {
            audioSource.spatialBlend = 0;
            audioSource.priority = 0;
        }

        private void FadeUpdate()
        {
            //For two main tracks
            for (var i = 0; i < 2; i++)
            {
                var towardsVolumeBgmVolumeApplied = towardsVolume[i];
                if (!(Math.Abs(twoTracks[i].FadeVolume - towardsVolumeBgmVolumeApplied) > 0.0001f))
                {
                    continue;
                }

                // Handles the fade in/out
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

        private void UnloadTrack(int trackNumber)
        {
            //Have to check if other track is using the music or not?

            //If playing the same song again,
            //the loading of the next song might come earlier, then got immediately unloaded by this.

            //Also check for when using different IntroloopAudio with the same source file.
            //In this case .Music will be not equal, but actually the audioClip inside is the same song.

            //Note that load/unloading has no effect on "Streaming" audio type.

            var musicEqualCurrent = twoTracks[trackNumber].Music == twoTracks[(trackNumber + 1) % 2].Music;
            var clipEqualCurrent = twoTracks[trackNumber].Music != null &&
                                   twoTracks[(trackNumber + 1) % 2].Music != null &&
                                   twoTracks[trackNumber].Music.AudioClip ==
                                   twoTracks[(trackNumber + 1) % 2].Music.AudioClip;

            //As = AudioSource
            var isSameSongAsCurrent = musicEqualCurrent || clipEqualCurrent;

            var musicEqualNext = twoTracks[trackNumber].Music == twoTracks[(trackNumber + 1) % 2].MusicAboutToPlay;
            var clipEqualNext = twoTracks[trackNumber].Music != null &&
                                twoTracks[(trackNumber + 1) % 2].MusicAboutToPlay != null &&
                                twoTracks[trackNumber].Music.AudioClip ==
                                twoTracks[(trackNumber + 1) % 2].MusicAboutToPlay.AudioClip;

            var isSameSongAsAboutToPlay = musicEqualNext || clipEqualNext;

            var usingAndPlaying = twoTracks[(trackNumber + 1) % 2].IsPlaying && isSameSongAsCurrent;

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
        ///     Play <see cref="IntroloopAudio"/> asset currently assigned to <see cref="DefaultIntroloopAudio"/>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         It applies <see cref="IntroloopAudio.Volume"/> and <see cref="IntroloopAudio.Pitch"/>
        ///         to the underlying <see cref="AudioSource"/>.
        ///     </para>
        ///     <para>
        ///         If an another <see cref="IntroloopAudio"/> is playing on this player,
        ///         it could cross-fade between the two if <paramref name="fadeLengthSeconds"/> is provided.
        ///         The faded out audio will be unloaded automatically once the fade is finished.
        ///     </para>
        /// </remarks>
        /// <param name="fadeLengthSeconds">
        ///     Fade in/out length to use in seconds.
        ///     <list type="bullet">
        ///         <item>
        ///             <description>If 0, it uses a small pop removal fade time.</description>
        ///         </item>
        ///         <item>
        ///             <description>If negative, it is immediate.</description>
        ///         </item>
        ///     </list>
        ///     The audio will be unloaded only after it had fade out completely.
        /// </param>
        /// <param name="startTime">
        ///     <para>
        ///         Specify starting point in time instead of starting from the beginning.
        ///     </para>
        ///     <para>
        ///         The time you specify here will be converted to "play head time", Introloop will make the play head
        ///         at the point in time as if you had played for this amount of time before starting.
        ///     </para>
        ///     <para>
        ///         Since <see cref="IntroloopAudio"/> conceptually has infinite length, any number that is over looping boundary
        ///         will be wrapped over to the intro boundary in the calculation. (Except that if the audio is non-looping)
        ///     </para>
        ///     <para>
        ///         The time specified here is <b>not</b> taking <see cref="IntroloopAudio.Pitch"/> into account.
        ///         It's an elapsed time as if <see cref="IntroloopAudio.Pitch"/> is 1.
        ///     </para>
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown when <see cref="DefaultIntroloopAudio"/> was not assigned.
        /// </exception>
        public void Play(float fadeLengthSeconds = 0, float startTime = 0)
        {
            if (defaultIntroloopAudio == null)
            {
                throw new ArgumentNullException(nameof(defaultIntroloopAudio),
                    "Default Introloop Audio was not assigned, but you called " +
                    "Play overload without IntroloopAudio argument.");
            }

            Play(defaultIntroloopAudio, fadeLengthSeconds, startTime);
        }

        /// <summary>
        ///     Play any <see cref="IntroloopAudio"/> asset with the argument <paramref name="introloopAudio"/>,
        ///     regardless of <see cref="IntroloopAudio"/> asset assigned in <see cref="DefaultIntroloopAudio"/>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         It applies <see cref="IntroloopAudio.Volume"/> and <see cref="IntroloopAudio.Pitch"/>
        ///         to the underlying <see cref="AudioSource"/>.
        ///     </para>
        ///     <para>
        ///         If an another <see cref="IntroloopAudio"/> is playing on this player,
        ///         it could cross-fade between the two if <paramref name="fadeLengthSeconds"/> is provided.
        ///         The faded out audio will be unloaded automatically once the fade is finished.
        ///     </para>
        /// </remarks>
        /// <param name="introloopAudio">
        ///     A reference to <see cref="IntroloopAudio"/> asset file to play.
        /// </param>
        /// <param name="fadeLengthSeconds">
        ///     Fade in/out length to use in seconds.
        ///     <list type="bullet">
        ///         <item>
        ///             <description>If 0, it uses a small pop removal fade time.</description>
        ///         </item>
        ///         <item>
        ///             <description>If negative, it is immediate.</description>
        ///         </item>
        ///     </list>
        ///     The audio will be unloaded only after it had fade out completely.
        /// </param>
        /// <param name="startTime">
        ///     <para>
        ///         Specify starting point in time instead of starting from the beginning.
        ///     </para>
        ///     <para>
        ///         The time you specify here will be converted to "play head time", Introloop will make the play head
        ///         at the point in time as if you had played for this amount of time before starting.
        ///     </para>
        ///     <para>
        ///         Since <see cref="IntroloopAudio"/> conceptually has infinite length, any number that is over looping boundary
        ///         will be wrapped over to the intro boundary in the calculation. (Except that if the audio is non-looping)
        ///     </para>
        ///     <para>
        ///         The time specified here is <b>not</b> taking <see cref="IntroloopAudio.Pitch"/> into account.
        ///         It's an elapsed time as if <see cref="IntroloopAudio.Pitch"/> is 1.
        ///     </para>
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="introloopAudio"/> is `null`.</exception>
        public void Play(IntroloopAudio introloopAudio, float fadeLengthSeconds = 0, float startTime = 0)
        {
            if (introloopAudio == null)
            {
                throw new ArgumentNullException(nameof(introloopAudio), "Played IntroloopAudio is null");
            }

            //Auto cross fade old ones. If no fade length specified, a very very small fade will be used to avoid pops/clicks.
            Stop(fadeLengthSeconds);

            var next = (currentTrack + 1) % 2;
            twoTracks[next].Play(introloopAudio, fadeLengthSeconds == 0 ? false : true, startTime);
            towardsVolume[next] = 1;
            fadeLength[next] = TranslateFadeLength(fadeLengthSeconds);

            currentTrack = next;
            previousPlay = introloopAudio;
        }

        /// <summary>
        ///     Similar to <see cref="Play(IntroloopAudio, float, float)"/> overload, but has only a single
        ///     argument so it is able to receive calls from <see cref="UnityEvent"/>.
        /// </summary>
        public void Play(IntroloopAudio introloopAudio)
        {
            Play(introloopAudio, 0);
        }

        /// <summary>
        ///     Similar to <see cref="Play(IntroloopAudio, float, float)"/> overload, but has no
        ///     optional arguments so it is able to receive calls from <see cref="UnityEvent"/>.
        /// </summary>
        public void Play()
        {
            Play(0);
        }

        /// <summary>
        ///     Move the play head of the currently playing audio to anywhere in terms of elapsed time.
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 If it is currently playing, you can instantly move the play head position to anywhere else.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 If it is not playing, no effect. (This includes while in paused state, you cannot seek in paused state.)
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         An internal implementation is not actually a seek, but a completely new
        ///         <see cref="Play(IntroloopAudio, float, float)"/> with the previous <see cref="IntroloopAudio"/>.
        ///     </para>
        ///     <para>
        ///         This is why you cannot seek while in pause, as it actually does a new play for you.
        ///         It is handy because it doesn't require you to remember and specify that audio again.
        ///     </para>
        /// </remarks>
        /// <param name="elapsedTime">
        ///     <para>
        ///         Introloop will make the play head at the point in time as if you had played for this amount
        ///         of time before starting.
        ///     </para>
        ///     <para>
        ///         The time you specify here will be converted to "play head time", Introloop will make the play head
        ///         at the point in time as if you had played for this amount of time before starting.
        ///     </para>
        ///     <para>
        ///         Since <see cref="IntroloopAudio"/> conceptually has infinite length, any number that is over looping boundary
        ///         will be wrapped over to the intro boundary in the calculation. (Except that if the audio is non-looping)
        ///         The time specified here is <b>not</b> taking <see cref="IntroloopAudio.Pitch"/> into account.
        ///         It's an elapsed time as if <see cref="IntroloopAudio.Pitch"/> is 1.
        ///     </para>
        /// </param>
        public void Seek(float elapsedTime)
        {
            if (!twoTracks[currentTrack].IsPlaying)
            {
                return;
            }

            twoTracks[currentTrack].Play(previousPlay, false, elapsedTime);
            towardsVolume[currentTrack] = 1;
            fadeLength[currentTrack] = 0;
        }

        /// <summary>
        ///     Stop the currently playing <see cref="IntroloopAudio"/> immediately and unload it from memory.
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
        ///     Fading out to stop the currently playing <see cref="IntroloopAudio"/>, and unload it from memory
        ///     once it is completely faded out.
        /// </summary>
        /// <param name="fadeLengthSeconds">
        ///     Fade out length to use in seconds.
        ///     <list type="bullet">
        ///         <item>
        ///             <description>0 is a special value that will still apply small pop removal fade time.</description>
        ///         </item>
        ///         <item>
        ///             <description>If negative, this method works like <see cref="Stop()"/> overload.</description>
        ///         </item>
        ///     </list>
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
        ///     Pause the currently playing <see cref="IntroloopAudio"/> immediately without unloading.
        ///     Call <see cref="Resume(float)"/> to continue playing.
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
        ///     Fading out to pause the currently playing <see cref="IntroloopAudio"/> without unloading.
        ///     Call <see cref="Resume(float)"/> to continue playing.
        /// </summary>
        /// <param name="fadeLengthSeconds">
        ///     Fade out length to use in seconds.
        ///     <list type="bullet">
        ///         <item>
        ///             <description>0 is a special value that will still apply small pop removal fade time.</description>
        ///         </item>
        ///         <item>
        ///             <description>If negative, this method works like <see cref="Pause()"/> overload.</description>
        ///         </item>
        ///     </list>
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
                    fadeLength[currentTrack] = TranslateFadeLength(fadeLengthSeconds);
                    ;
                    towardsVolume[currentTrack] = 0;
                }
            }
        }

        /// <summary>
        ///     Resume playing of previously paused (<see cref="Pause(float)"/>) <see cref="IntroloopAudio"/>.
        ///     If currently not pausing, it does nothing.
        /// </summary>
        /// <remarks>
        ///     Note that if it is currently "fading to pause", the state is not considered paused
        ///     yet so you can't resume in that time.
        /// </remarks>
        /// <param name="fadeLengthSeconds">
        ///     Fade out length to use in seconds.
        ///     <list type="bullet">
        ///         <item>
        ///             <description>If 0, it uses a small pop removal fade time.</description>
        ///         </item>
        ///         <item>
        ///             <description>If negative, it resumes immediately.</description>
        ///         </item>
        ///     </list>
        /// </param>
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
        ///     Zero length is a special value that equals pop removal small fade time.
        ///     Negative length is a special value that equals (real) 0.
        /// </summary>
        private static float TranslateFadeLength(float fadeLength)
        {
            return fadeLength > 0 ? fadeLength : fadeLength < 0 ? 0 : popRemovalFadeTime;
        }

        /// <summary>
        ///     An experimental feature in the case that you really want the audio to start
        ///     in an instant you call <see cref="Play(IntroloopAudio, float, float)"/>. You must use the same
        ///     <see cref="IntroloopAudio"/> that you preload in the next play.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         By normally using <see cref="Play(IntroloopAudio, float, float)"/> and <see cref="Stop(float)"/>
        ///         it loads the audio the moment you called <see cref="Play(IntroloopAudio, float, float)"/>.
        ///         Introloop waits for an audio to load before playing with a coroutine.
        ///     </para>
        ///     <para>
        ///         (Only if you have <see cref="AudioClip.loadInBackground"/> in the import settings.
        ///         Otherwise, <see cref="Play(IntroloopAudio, float, float)"/> will be a blocking call.)
        ///     </para>
        ///     <para>
        ///         Introloop can't guarantee that the playback will be instant,
        ///         but your game can continue while it is loading. By using this method before actually calling
        ///         <see cref="Play(IntroloopAudio, float, float)"/> it will instead be instant.
        ///     </para>
        ///     <para>
        ///         This function is special even songs with <see cref="AudioClip.loadInBackground"/>
        ///         can be loaded in a blocking fashion. (You can put <see cref="Play(IntroloopAudio, float, float)"/> immediately
        ///         in the next line expecting a fully loaded audio.)
        ///     </para>
        ///     <para>
        ///         However be aware that memory is managed less efficiently in the following case :
        ///         Normally Introloop immediately unloads the previous track to minimize memory.
        ///         But if you use <see cref="Preload(IntroloopAudio)"/> then did not call
        ///         <see cref="Play(IntroloopAudio, float, float)"/> with the same <see cref="IntroloopAudio"/> afterwards,
        ///         the loaded memory will be unmanaged.
        ///     </para>
        ///     <para>
        ///         (Just like if you tick <see cref="AudioClip.preloadAudioData"/> on your clip and have them
        ///         in a hierarchy somewhere, then did not use it.)
        ///     </para>
        ///     <para>
        ///         Does not work with <see cref="AudioClipLoadType.Streaming"/> audio loading type.
        ///     </para>
        /// </remarks>
        public void Preload(IntroloopAudio introloopAudio)
        {
            introloopAudio.Preload();
        }

        /// <summary>
        ///     This interpretation of a play time could decrease when it goes over
        ///     looping boundary back to intro boundary. Conceptually Introloop audio has infinite length,
        ///     so this time is a bit different from normal sense.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Think as it as not "elapsed time" but rather the position of the actual playhead,
        ///         expressed in time as if the pitch is 1.
        ///     </para>
        ///     <para>
        ///         For example with pitch enabled, the play head will move slowly,
        ///         and so the time returned from this method respect that slower play head.
        ///     </para>
        ///     <para>
        ///         It is usable with <see cref="Play(IntroloopAudio, float, float)"/> as a start time
        ///         to "restore" the play from remembered time. With only 1 <see cref="IntroloopPlayer"/> you can stop and
        ///         unload previous song then continue later after reloading it.
        ///     </para>
        ///     <para>
        ///         Common use case includes battle music which resumes the field music afterwards.
        ///         If the battle is memory consuming unloading the field music could help.
        ///     </para>
        /// </remarks>
        public float GetPlayheadTime()
        {
            return twoTracks[currentTrack].PlayheadPositionSeconds;
        }

        /// <summary>
        ///     Assign a different audio mixer group to all underlying <see cref="AudioSource"/>.
        /// </summary>
        public void SetMixerGroup(AudioMixerGroup audioMixerGroup)
        {
            foreach (var aSource in InternalAudioSources)
            {
                aSource.outputAudioMixerGroup = audioMixerGroup;
            }
        }

        /// <summary>
        ///     Call this before the first use of <see cref="Instance"/> to have the singleton instance
        ///     copy <see cref="AudioSource"/> fields from <paramref name="templateSource"/>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Singleton instance is convenient but you cannot pre-connect <see cref="TemplateSource"/> like
        ///         a regular instance because it does not exist until runtime.
        ///     </para>
        ///     <para>
        ///         If you had already used the singleton instance before calling this, you can still call
        ///         <see cref="ApplyAudioSource"/> on the singleton instance to apply different
        ///         settings of <see cref="AudioSource"/>.
        ///     </para>
        /// </remarks>
        public static void SetSingletonInstanceTemplateSource(AudioSource templateSource)
        {
            singletonInstanceTemplateSource = templateSource;
        }

        /// <summary>
        ///     <para>
        ///         Copy fields from <paramref name="applyFrom"/> to all 4 underlying <see cref="AudioSource"/>.
        ///         Make it as if they had <paramref name="applyFrom"/> as a <see cref="TemplateSource"/> from
        ///         the beginning. (Or you can think this method as a way to late-assign a <see cref="TemplateSource"/>.)
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     The <see cref="AudioSource"/> does not need to be <see cref="MonoBehaviour.enabled"/> since it just
        ///     need to read the fields out for copy. Also it does not need to be anywhere on the scene,
        ///     it can come from a prefab with <see cref="AudioSource"/> in your project.
        /// </remarks>
        public void ApplyAudioSource(AudioSource applyFrom)
        {
            foreach (var aSource in InternalAudioSources)
            {
                CopyAudioSourceFields(aSource, applyFrom);
            }
        }

        private static void CopyAudioSourceFields(AudioSource copyTo, AudioSource copyFrom)
        {
            // Pitch is NOT inherited, that could destroy scheduling!
            // Pitch can only be specified via the IntroloopAudio asset file.

            copyTo.outputAudioMixerGroup = copyFrom.outputAudioMixerGroup;

            copyTo.SetCustomCurve(AudioSourceCurveType.CustomRolloff,
                copyFrom.GetCustomCurve(AudioSourceCurveType.CustomRolloff));
            copyTo.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix,
                copyFrom.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix));
            copyTo.SetCustomCurve(AudioSourceCurveType.SpatialBlend,
                copyFrom.GetCustomCurve(AudioSourceCurveType.SpatialBlend));
            copyTo.SetCustomCurve(AudioSourceCurveType.Spread,
                copyFrom.GetCustomCurve(AudioSourceCurveType.Spread));

            // Spatial blend, reverb zone mix, and spread must NOT be copied
            // since it will overwrite the curve copy above.

            copyTo.ignoreListenerVolume = copyFrom.ignoreListenerVolume;
            copyTo.ignoreListenerPause = copyFrom.ignoreListenerPause;
            copyTo.velocityUpdateMode = copyFrom.velocityUpdateMode;
            copyTo.panStereo = copyFrom.panStereo;
            // applyTarget.spatialBlend = applyFrom.spatialBlend;
            copyTo.spatialize = copyFrom.spatialize;
            copyTo.spatializePostEffects = copyFrom.spatializePostEffects;
            // applyTarget.reverbZoneMix = applyFrom.reverbZoneMix;
            copyTo.bypassEffects = copyFrom.bypassEffects;
            copyTo.bypassListenerEffects = copyFrom.bypassListenerEffects;
            copyTo.bypassReverbZones = copyFrom.bypassReverbZones;
            copyTo.dopplerLevel = copyFrom.dopplerLevel;
            // applyTarget.spread = applyFrom.spread;
            copyTo.priority = copyFrom.priority;
            copyTo.mute = copyFrom.mute;
            copyTo.minDistance = copyFrom.minDistance;
            copyTo.maxDistance = copyFrom.maxDistance;
        }

        /// <summary>
        ///     Each player contains 4 <see cref="AudioSource"/>, this method
        ///     returns the current information of the first pair for debugging purpose.
        /// </summary>
        public string[] GetDebugStringsTrack1()
        {
            return twoTracks[0].DebugInformation;
        }

        /// <summary>
        ///     Each player contains 4 <see cref="AudioSource"/>, this method
        ///     returns the current information of the second pair for debugging purpose.
        /// </summary>
        public string[] GetDebugStringsTrack2()
        {
            return twoTracks[1].DebugInformation;
        }
    }
}