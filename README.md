# Toriki

Tori(the Twitter Bird in Japanese) kit -> Toriki.  

Ultra lightweight Twitter tool for Unity. supports the SingleSignOn with Twitter app and Twitter API invocation.  

Currently, Toriki requires Twitter application in supported platflorms.


## supported platforms

* iOS
* Android
* WebGL(under development.)

with Unity 2018.x or later.


## installation
Use UnityPackage in [releases](https://github.com/sassembla/toriki/releases).


## usage

### 1. setup Twitter ConsumerKey and ConsumerSecret 

```csharp
static TwitterSettings()
{
    ConsumerKey = string.Empty;// change here.
    ConsumerSecret = string.Empty;// change here.

    if (string.IsNullOrEmpty(ConsumerKey))
    {
        Debug.Log("ConsumerKey is null. please write some code for setting ConsumerKey.");
    }

    if (string.IsNullOrEmpty(ConsumerSecret))
    {
        Debug.Log("ConsumerSecret is null. please write some code for setting ConsumerSecret.");
    }
}
```

### 2. Log in to Twitter

get AccessToken & AccessTokenSecret from Twitter.

```csharp
TwitterAPI.InitWithLogin(
    (nickname, token, secret) =>
    {
    	// nickname, accesstoken, accesstokensecret is available. 
    	// also TwitterAPI is ready now.
    },
    (errorCode, message) =>
    {
    	// failed to log in to Twitter.
        Debug.Log("login error:" + errorCode + " message:" + message);
    }
```

or

log in with stored AccessToken & AccessTokenSecret.

```csharp
TwitterAPI.InitWithToken("ACCESS_TOKEN", "ACCESS_SECRET");
```


### 3. Use Twitter API

after log in to Twitter, you can use Twitter APIs.

```csharp
// tweet API example.
TwitterAPI.Post(
    "https://api.twitter.com/1.1/statuses/update.json",
    new SortedDictionary<string, string>
    {
        {"status", "#LoveToriki. "}
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
```

## license
[license](https://github.com/sassembla/toriki/blob/master/LICENSE)

