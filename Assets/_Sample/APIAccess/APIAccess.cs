using System.Collections.Generic;
using Toriki;
using UnityEngine;

public class APIAccess : MonoBehaviour
{
    public static string ACCESS_TOKEN;
    public static string ACCESS_SECRET;

    void Start()
    {
        TwitterAPI.InitWithToken(ACCESS_TOKEN, ACCESS_SECRET);
        TwitterAPI.Get(
            "https://api.twitter.com/1.1/users/show.json",
            new SortedDictionary<string, string>
            {
                {"screen_name", "toru_inoue"}
            },
            result =>
            {
                Debug.Log("show result:" + result);
            },
            (code, errors) =>
            {
                Debug.Log("show code:" + code);
            }
        );
    }
}
