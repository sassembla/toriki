#if UNITY_IOS
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.IO;
using Toriki.Settings;
using UnityEditor.iOS.Xcode;

namespace Toriki.Internal
{
    public class TwitterPostProcessBuild
    {
        private const string URL_TYPES = "CFBundleURLTypes";
        private const string URL_SCHEMES = "CFBundleURLSchemes";
        private const string APPLICATION_QUERIES_SCHEMES = "LSApplicationQueriesSchemes";

        [PostProcessBuild]
        public static void SetupXcodeSettingsForToriki(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                // Get plist
                string plistPath = pathToBuiltProject + "/Info.plist";
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));

                // Get root
                PlistElementDict rootDict = plist.root;

                // Modify Info.Plist for Twitter Kit (https://dev.twitter.com/twitterkit/ios/installation)
                PlistElementArray bundleURLTypesArray = rootDict[URL_TYPES] as PlistElementArray;
                if (bundleURLTypesArray == null)
                {
                    bundleURLTypesArray = rootDict.CreateArray(URL_TYPES);
                }
                PlistElementDict dict = bundleURLTypesArray.AddDict();
                PlistElementArray bundleURLSchemesArray = dict.CreateArray(URL_SCHEMES);
                bundleURLSchemesArray.AddString("twitterkit-" + TwitterSettings.ConsumerKey);
                PlistElementArray queriesSchemesArray = rootDict[APPLICATION_QUERIES_SCHEMES] as PlistElementArray;
                if (queriesSchemesArray == null)
                {
                    queriesSchemesArray = rootDict.CreateArray(APPLICATION_QUERIES_SCHEMES);
                }
                queriesSchemesArray.AddString("twitter");
                queriesSchemesArray.AddString("twitterauth");

                // Write to file
                File.WriteAllText(plistPath, plist.WriteToString());
            }
        }
    }
}
#endif