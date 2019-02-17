namespace Toriki.Internal
{
    internal interface ITwitterImpl
    {
        void Init(string consumerKey, string consumerSecret);

        void LogIn();
    }
}
