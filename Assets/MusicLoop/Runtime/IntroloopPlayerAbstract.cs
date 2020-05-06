/* 
/// Copyright (c) 2015 Sirawat Pitaksarit, Exceed7 Experiments LP 
/// http://www.exceed7.com/introloop
*/

namespace E7.Introloop
{
    /// <summary>
    /// Define your class like this `public class FieldBGMPlayer : IntroloopPlayer<FieldBGMPlayer>` (Put your class itself into the generic variable)
    /// </summary>
    public abstract class IntroloopPlayer<T> : IntroloopPlayer where T : IntroloopPlayer
    {
        private static T instance;
        protected override bool IsIntroloopSubclass { get { return true; } }

        /// <summary>
        /// With <see cref="IntroloopPlayer.Instance">, it refers to the same "Instance" throughout your game.
        /// Meaning that you cannot have 2 concurrent Introloop players playing+looping at the same time.
        /// 
        /// With <code>MySubClassOfIntroloopPlayer.Get</code>, it will spawns a different player.
        /// This means you can now have many Introloop playing at the same.
        /// It is useful for dividing the players into several parts. Like `BGMPlayer`, `AmbientPlayer`, etc.
        /// 
        /// Moreover, you can then define your own methods on your subclass to be more suitable for your game.
        /// Like <code>FieldBGMPlayer.Get.PlayDesertTheme()</code> instead of <code>IntroloopPlayer.Instance.Play(desertTheme);</code>.
        /// 
        /// The template's name was hardcoded as the same as your class name.
        /// If your class name is FieldBGMPlayer then you must have FieldBGMPlayer.prefab in 
        /// the same location as IntroloopPlayer.prefab in Resources folder. (Defined in <see cref="IntroloopSettings.defaultTemplatePathWithoutFileName"> as a constant `string`.)
        /// </summary>
        public static T Get
        {
            get
            {
                if (instance == null)
                {
                    instance = MakeSingletonInstance<T>();
                }
                return instance;
            }
        }

    }

}