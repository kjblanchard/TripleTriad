using UnityEngine;

namespace E7.Introloop
{
    /// <summary>
    ///     <para>
    ///         An <c>abstract</c> class that grants you a new set of convenient singleton instance just like the
    ///         <see cref="IntroloopPlayer.Instance"/>, but now under your own self-defined class name.
    ///         (e.g. You can now call <c>MyIntroloopPlayer.Instance</c> and so on.)
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class utilize C# trick where each unique combination of generic class gives you a
    ///         unique instance of <c>static</c> variables. Putting your self-defined class itself inside <c>T</c>
    ///         basically give you a personal set of <c>static</c>.
    ///     </para>
    ///     <para>
    ///         Define your class like this :
    ///         <code>
    ///     public class FieldBGMPlayer : IntroloopPlayer&lt;FieldBGMPlayer&gt;
    ///     </code>
    ///         (Put your class <b>itself</b> into the generic variable.)
    ///     </para>
    /// </remarks>
    public abstract class IntroloopPlayer<T> : IntroloopPlayer where T : IntroloopPlayer
    {
        private static T instance;

        // ReSharper disable once StaticMemberInGenericType
        private static AudioSource singletonInstanceTemplateSource;

        /// <summary>
        ///     <para>
        ///         Get a convenient singleton instance of <see cref="T"/> from anywhere in your code.
        ///         This singleton instant is different for each subclass of this <c>abstract</c> class.
        ///         It has <c>DontDestroyOnLoad</c> applied.
        ///     </para>
        ///     <para>
        ///         Before calling this <b>for the first time</b>, call <see cref="SetSingletonInstanceTemplateSource"/>
        ///         first to setup its <see cref="IntroloopPlayer.TemplateSource"/> from script.
        ///         (It does not exist until runtime, you cannot setup the template ahead of time
        ///         unlike non-singleton instances.)
        ///     </para>
        /// </summary>
        public new static T Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                instance = InstantiateSingletonPlayer<T>(singletonInstanceTemplateSource);
                instance.name = IntroloopSettings.singletonObjectPrefix + instance.name;

                return instance;
            }
        }

        /// <summary>
        ///     Call this before the first use of <see cref="Instance"/> to have the singleton instance
        ///     copy <see cref="AudioSource"/> fields from <paramref name="templateSource"/>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Singleton instance is convenient but you cannot pre-connect
        ///         <see cref="IntroloopPlayer.TemplateSource"/> like a regular instance because
        ///         it does not exist until runtime.
        ///     </para>
        ///     <para>
        ///         If you had already used the singleton instance before calling this, you can still call
        ///         <see cref="IntroloopPlayer.ApplyAudioSource"/> on the singleton instance to apply different
        ///         settings of <see cref="AudioSource"/>.
        ///     </para>
        /// </remarks>
        public new static void SetSingletonInstanceTemplateSource(AudioSource templateSource)
        {
            singletonInstanceTemplateSource = templateSource;
        }
    }
}