using System.Collections.Generic;
using Toriki;
using UnityEngine;

public class Login : MonoBehaviour
{
    void Start()
    {
        TwitterAPI.InitWithLogin(
            (nickname, token, secret) =>
            {
                // set access token to another scene API use.
                APIAccess.ACCESS_TOKEN = token;
                APIAccess.ACCESS_SECRET = secret;

                // tweet API example.
                TwitterAPI.Post(
                    "https://api.twitter.com/1.1/statuses/update.json",
                    new SortedDictionary<string, string>
                    {
                        {"status", "#LoveToriki"}
                    },
                    result =>
                    {
                        Debug.Log("update result:" + result);
                        UnityEngine.SceneManagement.SceneManager.LoadScene("_Sample/APIAccess/APIAccess");
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
