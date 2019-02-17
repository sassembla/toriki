using System.Collections.Generic;
using Toriki;
using UnityEngine;

public class Login : MonoBehaviour
{
    void Start()
    {
        TwitterAPI.Init();
        TwitterAPI.LogIn(
            (nickname, token, secret) =>
            {
                // show user info API example.
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

                // tweet API example.
                TwitterAPI.Post(
                    "https://api.twitter.com/1.1/statuses/update.json",
                    new SortedDictionary<string, string>
                    {
                        {"status", "Love Toriki. "}
                    },
                    result =>
                    {
                        Debug.Log("update result:" + result);
                    },
                    (code, errors) =>
                    {
                        Debug.Log("update code:" + code);
                    }
                );
            },
            (errorCode, message) =>
            {
                Debug.Log("login error:" + errorCode + " message:" + message);
            }
        );
    }
}
