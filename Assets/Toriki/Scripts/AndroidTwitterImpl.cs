#if UNITY_EDITOR
#elif UNITY_ANDROID
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Toriki.Settings;

namespace Toriki.Internal
{
    internal class AndroidTwitterImpl : ITwitterImpl
    {
        private AndroidJavaClass twitter = new AndroidJavaClass("com.twitter.sdk.android.unity.TwitterKit");

        public void Init(string consumerKey, string consumerSecret)
        {
            twitter.CallStatic("init", consumerKey, consumerSecret);
        }

        public void LogIn()
        {
            twitter.CallStatic("login");
        }
    }
}
#endif