#if UNITY_EDITOR
using Toriki.Settings;
using UnityEngine;

namespace Toriki.Internal
{
    internal class EditorTwitterImpl : ITwitterImpl
    {
        public EditorTwitterImpl()
        {
        }

        public void Init(string consumerKey, string consumerSecret)
        {
            Debug.Log("Would call Twitter init on a physical device with key=" + consumerKey + " and secret=" + consumerSecret);
            // このへんどうするのがいいのかな〜〜、、key, secretがあれば、tokenが得られるか、、というと得られないし。 accessTokenとかがあれば。
        }

        public void LogIn()
        {
            Debug.Log("Would call Twitter login on a physical device.");
        }
    }
}
#endif
