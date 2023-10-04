#if HAS_AAS
using System;
using UnityEngine.AddressableAssets;

namespace E7.Introloop
{
    /// <summary>
    ///     Serialize an addressable asset reference to <see cref="IntroloopAudio"/>.
    ///     You can load this asynchronously before using the result with <see cref="IntroloopPlayer"/>.
    /// </summary>
    [Serializable]
    public class AssetReferenceIntroloopAudio : AssetReferenceT<IntroloopAudio>
    {
        public AssetReferenceIntroloopAudio(string guid) : base(guid)
        {
        }
    }
}
#endif