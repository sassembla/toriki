#if UNITY_EDITOR
#elif UNITY_IOS
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Toriki.Settings;

namespace Toriki.Internal
{
    internal class IOSTwitterImpl : ITwitterImpl
    {
        [DllImport("__Internal")]
        private static extern void TwitterInit(string consumerKey, string consumerSecret);

        [DllImport("__Internal")]
        private static extern void TwitterLogIn();

        public void Init(string consumerKey, string consumerSecret)
        {
            IOSTwitterImpl.TwitterInit(consumerKey, consumerSecret);
        }

        public void LogIn()
        {
            IOSTwitterImpl.TwitterLogIn();
        }
    }
}
#endif
