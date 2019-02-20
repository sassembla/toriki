using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Toriki.Internal
{
    public class TwitterConnector
    {

        private readonly string consumerKey;
        private readonly string consumerKeySecret;
        private readonly string accessToken;
        private readonly string accessTokenSecret;

        private const string OauthVersion = "1.0";
        private const string OauthSignatureMethod = "HMAC-SHA1";

        public TwitterConnector(string consumerKey, string consumerKeySecret, string accessToken, string accessTokenSecret)
        {
            this.consumerKey = consumerKey;
            this.consumerKeySecret = consumerKeySecret;
            this.accessToken = accessToken;
            this.accessTokenSecret = accessTokenSecret;
        }


        /*
            与えられたTwitter API urlに対し、指定したHTTPメソッドでアクセスを行う。
         */
        public IEnumerator GenerateAccessCoroutine(
            string resourceUrl,
            TKMethod method,
            SortedDictionary<string, string> urlQueryParameters,
            SortedDictionary<string, string> bodyParameters,
            Action<string, int, Dictionary<string, string>, string> succeeded,
            Action<string, int, TwitterAPI.APIError, Dictionary<string, string>> failed
        )
        {
            if (urlQueryParameters == null) urlQueryParameters = new SortedDictionary<string, string>();
            if (bodyParameters == null) bodyParameters = new SortedDictionary<string, string>();

            /*
                collect all parameters.
             */
            var allParameters = new SortedDictionary<string, string>();
            foreach (var item in urlQueryParameters)
            {
                allParameters[item.Key] = item.Value;
            }

            foreach (var item in bodyParameters)
            {
                allParameters[item.Key] = item.Value;
            }


            var oauthNonce = GenerateNonce();
            var oauthTimestamp = GenerateTimestamp();
            var oauthSignature = GenerateSignature(resourceUrl, method, oauthNonce, oauthTimestamp, allParameters);

            /*
                generate authorization header.
             */
            const string headerFormat = "OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", " + "oauth_signature=\"{2}\", oauth_signature_method=\"{3}\", " + "oauth_timestamp=\"{4}\", oauth_token=\"{5}\", " + "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                Uri.EscapeDataString(consumerKey),
                Uri.EscapeDataString(oauthNonce),
                Uri.EscapeDataString(oauthSignature),
                Uri.EscapeDataString(OauthSignatureMethod),
                Uri.EscapeDataString(oauthTimestamp),
                Uri.EscapeDataString(accessToken),
                Uri.EscapeDataString(OauthVersion)
            );

            // Debug.Log("authHeader:" + authHeader);

            var reqCon = new TKHTTPConnection();
            IEnumerator requestCor = null;

            switch (method)
            {
                case TKMethod.GET:
                    {
                        requestCor = reqCon.Get(
                            "twitterCon_Get_" + Guid.NewGuid().ToString(),
                            new Dictionary<string, string>
                            {
                                    {"Authorization", authHeader}
                            },
                            resourceUrl + GenerateQueryParameters(urlQueryParameters),
                            succeeded,
                            (conId, code, reason, response) =>
                            {
                                TwitterAPI.APIError error = null;
                                try
                                {
                                    error = JsonUtility.FromJson<TwitterAPI.APIError>(reason);
                                }
                                catch
                                {
                                    error = new TwitterAPI.APIError()
                                    {
                                        errors = new TwitterAPI.APIError.Error[1]
                                        {
                                                new TwitterAPI.APIError.Error()
                                                {
                                                    code = code,
                                                    message = reason
                                                }
                                        }
                                    };
                                }

                                failed(conId, code, error, response);
                            }
                        );
                        break;
                    }
                case TKMethod.POST:
                    {
                        var pairArray = new List<string>();
                        foreach (var pair in bodyParameters)
                        {
                            var kv = "\"" + pair.Key + "\":\"" + pair.Value + "\"";
                            pairArray.Add(kv);
                        }

                        var data = "{" + string.Join(",", pairArray.ToArray()) + "}";

                        requestCor = reqCon.Post(
                            "twitterCon_Post_" + Guid.NewGuid().ToString(),
                            new Dictionary<string, string>
                            {
                                   {"Authorization", authHeader},
                            },
                            resourceUrl + GenerateQueryParameters(bodyParameters),
                            data,
                            succeeded,
                            (conId, code, reason, response) =>
                            {
                                TwitterAPI.APIError error = null;
                                try
                                {
                                    error = JsonUtility.FromJson<TwitterAPI.APIError>(reason);
                                }
                                catch
                                {
                                    error = new TwitterAPI.APIError()
                                    {
                                        errors = new TwitterAPI.APIError.Error[1]
                                        {
                                                new TwitterAPI.APIError.Error()
                                                {
                                                    code = code,
                                                    message = reason
                                                }
                                        }
                                    };
                                }

                                failed(conId, code, error, response);
                            }
                        );
                        break;
                    }
            }

            while (requestCor.MoveNext())
            {
                yield return null;
            }
        }

        private class TKHTTPConnection
        {
            public IEnumerator Get(string connectionId, Dictionary<string, string> requestHeader, string url, Action<string, int, Dictionary<string, string>, string> succeeded, Action<string, int, string, Dictionary<string, string>> failed)
            {
                var currentDate = DateTime.UtcNow;

                using (var request = UnityWebRequest.Get(url))
                {
                    if (requestHeader != null)
                    {
                        foreach (var kv in requestHeader)
                        {
                            request.SetRequestHeader(kv.Key, kv.Value);
                        }
                    }
                    request.chunkedTransfer = false;

                    var p = request.SendWebRequest();

                    while (!p.isDone)
                    {
                        yield return null;
                    }

                    var responseCode = (int)request.responseCode;
                    var responseHeaders = request.GetResponseHeaders();

                    if (request.isNetworkError)
                    {
                        failed(connectionId, responseCode, request.error, responseHeaders);
                        yield break;
                    }

                    var result = Encoding.UTF8.GetString(request.downloadHandler.data);
                    if (200 <= responseCode && responseCode <= 299)
                    {
                        succeeded(connectionId, responseCode, responseHeaders, result);
                    }
                    else
                    {
                        failed(connectionId, responseCode, result, responseHeaders);
                    }
                }
            }

            public IEnumerator Post(string connectionId, Dictionary<string, string> requestHeader, string url, string data, Action<string, int, Dictionary<string, string>, string> succeeded, Action<string, int, string, Dictionary<string, string>> failed)
            {
                var currentDate = DateTime.UtcNow;

                using (var request = UnityWebRequest.Post(url, data))
                {
                    request.uploadHandler = (UploadHandler)new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
                    if (requestHeader != null)
                    {
                        foreach (var kv in requestHeader)
                        {
                            request.SetRequestHeader(kv.Key, kv.Value);
                        }
                    }
                    request.chunkedTransfer = false;

                    var p = request.SendWebRequest();

                    while (!p.isDone)
                    {
                        yield return null;
                    }

                    var responseCode = (int)request.responseCode;
                    var responseHeaders = request.GetResponseHeaders();

                    if (request.isNetworkError)
                    {
                        failed(connectionId, responseCode, request.error, responseHeaders);
                        yield break;
                    }

                    var result = Encoding.UTF8.GetString(request.downloadHandler.data);
                    if (200 <= responseCode && responseCode <= 299)
                    {
                        succeeded(connectionId, responseCode, responseHeaders, result);
                    }
                    else
                    {
                        failed(connectionId, responseCode, result, responseHeaders);
                    }
                }
            }
        }

        private string GenerateNonce()
        {
            return Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
        }

        private string GenerateQueryParameters(SortedDictionary<string, string> urlQueryParameters)
        {
            if (urlQueryParameters.Count == 0)
            {
                return string.Empty;
            }

            var builder = new List<string>();

            foreach (var item in urlQueryParameters)
            {
                builder.Add(item.Key + "=" + Uri.EscapeDataString(item.Value));
            }

            return "?" + string.Join("&", builder.ToArray());
        }

        private string ToWebString(SortedDictionary<string, string> source)
        {
            var body = new StringBuilder();

            foreach (var requestParameter in source)
            {
                body.Append(requestParameter.Key);
                body.Append("=");
                body.Append(Uri.EscapeDataString(requestParameter.Value));
                body.Append("&");
            }

            //不要な末尾の&を消す。
            body.Remove(body.Length - 1, 1);

            return body.ToString();
        }

        private string GenerateSignature(string resourceUrl, TKMethod method, string oauthNonce, string oauthTimestamp, SortedDictionary<string, string> requestParameters)
        {
            // パラメータ集合にoauth用の値を加える。
            requestParameters.Add("oauth_consumer_key", consumerKey);
            requestParameters.Add("oauth_nonce", oauthNonce);
            requestParameters.Add("oauth_signature_method", OauthSignatureMethod);
            requestParameters.Add("oauth_timestamp", oauthTimestamp);
            requestParameters.Add("oauth_token", accessToken);
            requestParameters.Add("oauth_version", OauthVersion);

            var sigBaseString = ToWebString(requestParameters);

            /*
                method と url と パラメータ列を&で結合
             */
            var signatureBaseString = string.Concat(
                method.ToString(),
                "&",
                Uri.EscapeDataString(resourceUrl),
                "&",
                Uri.EscapeDataString(sigBaseString)
            );

            // consumer secret と access secret を結合、
            var compositeKey = string.Concat(Uri.EscapeDataString(consumerKeySecret), "&", Uri.EscapeDataString(accessTokenSecret));

            var oauthSignature = string.Empty;
            using (var hasher = new HMACSHA1(Encoding.ASCII.GetBytes(compositeKey)))
            {
                oauthSignature = Convert.ToBase64String(
                    hasher.ComputeHash(Encoding.ASCII.GetBytes(signatureBaseString)));
            }

            return oauthSignature;
        }

        private static string GenerateTimestamp()
        {
            var nowUtc = DateTime.UtcNow;
            var timeSpan = nowUtc - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            return timestamp;
        }
    }

    public enum TKMethod
    {
        POST,
        GET
    }
}