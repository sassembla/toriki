#import "TKTwitter.h"
#import "TKConstants.h"
#import "TKLoginURLParser.h"

@interface TKTwitter ()
@property(nonatomic, copy, readwrite) NSString *consumerKey;
@property(nonatomic, copy, readwrite) NSString *consumerSecret;
@property(nonatomic, readonly, getter=isInitialized) BOOL initialized;
@property(nonatomic) TKLoginURLParser *loginURLParser;
@property(nonatomic, copy) TKLogInCompletion completion;
@end

@implementation TKTwitter

static TKTwitter *sharedTwitter;
+ (TKTwitter *)sharedInstance {
  if (!sharedTwitter) {
    sharedTwitter = [[super allocWithZone:nil] init];
  }

  return sharedTwitter;
}

+ (id)allocWithZone:(NSZone *)zone {
  return [self sharedInstance];
}

- (void)startWithConsumerKey:(NSString *)consumerKey
              consumerSecret:(NSString *)consumerSecret {
  if (self.isInitialized) {
    return;
  }

  self.consumerKey = consumerKey;
  self.consumerSecret = consumerSecret;
  self.loginURLParser =
      [[TKLoginURLParser alloc] initWithKey:self.consumerKey
                                     secret:self.consumerSecret];

  _initialized = YES;
}

- (void)assertTwitterKitInitialized {
  if (!self.isInitialized) {
    [NSException raise:@"TKInvalidInitializationException"
                format:@"Attempted to call TwitterKit methods before calling "
                       @"the requisite start method. You must call "
                       @"TKTwitter.sharedInstance().start(withConsumerKey:"
                       @"consumerSecret:) before calling any other methods."];
  }
}

/**
    ログイン処理 = Twitterアプリの起動と認証を行う。
    Twiiterアプリからのレスポンスパラメータの検証を行い、エラーがなければ成功とみなす。
 */
- (void)attemptLogin:(TKLogInCompletion)unityCompletion {
  [self assertTwitterKitInitialized];

  if (![self.loginURLParser hasValidURLScheme]) {
    //        アプリのURLSchemeに、TwitterAPpから起動するためのセッティングが足りない。このままだとTwitterAppから復帰できない。
    [NSException raise:@"TKInvalidInitializationException"
                format:@"Attempt made to Log in without a valid Twitter Kit "
                       @"URL Scheme set up in the app settings. Please see "
                       @"https://dev.twitter.com/twitterkit/ios/installation "
                       @"for more info."];
    return;
  }

  [self
      attemptAppLoginWithCompletion:^(NSString *username, NSString *accessToken,
                                      NSString *accessSecret, NSError *error) {
        //        エラーがなければ成功
        if (!error) {
          unityCompletion(username, accessToken, accessSecret, error);
          return;
        }

        //        失敗
        unityCompletion(nil, nil, nil, error);
      }];
}

- (void)attemptAppLoginWithCompletion:(TKLogInCompletion)completion {
  //    ここで終了時処理をキャプチャしておき、TwitterAppから本Appが呼ばれた際に実行できるようにしておく。
  self.completion = [completion copy];

  //    Twitterアプリを開く
  NSURL *twitterAuthURL = [self.loginURLParser twitterAuthorizeURL];
  [[UIApplication sharedApplication]
                openURL:twitterAuthURL
                options:@{}
      completionHandler:^(BOOL success) {
        //            失敗した場合、アプリケーションが存在していない。
        if (!success) {
          completion(nil, nil, nil,
                     [[NSError alloc] initWithDomain:TKLogInErrorDomain
                                                code:TKLoginErrorNoTwitterApp
                                            userInfo:@{
                                              NSLocalizedDescriptionKey :
                                                  @"No Twitter App installed. "
                                                  @"please install Twitter.app."
                                            }]);
        }
      }];
}

- (void)onAppLaunch:(NSURL *)url
    sourceApplication:(NSString *)sourceApplication {
  if ([self.loginURLParser isTwitterKitRedirectURL:url]) {
    // 成功URLかどうかチェック
    if ([self.loginURLParser isMobileSSOSuccessURL:url]) {
      NSDictionary *parameters = [self.loginURLParser parametersForSSOURL:url];

      // 最終的なアプリ側へのパラメータ渡しを行う
      self.completion(parameters[@"username"], parameters[@"token"],
                      parameters[@"secret"], nil);
      return;
    }

    // キャンセルURLかどうかチェック
    if ([self.loginURLParser isMobileSSOCancelURL:url]) {
      // The user cancelled the Twitter SSO flow
      dispatch_async(dispatch_get_main_queue(), ^{
        self.completion(nil, nil, nil,
                        [[NSError alloc]
                            initWithDomain:TKLogInErrorDomain
                                      code:TKLogInErrorCodeCancelled
                                  userInfo:@{
                                    NSLocalizedDescriptionKey :
                                        @"User cancelled login from Twitter App"
                                  }]);
      });
      return;
    }

    NSLog(@"unhandled twitter error. url:%@", url);
    return;
  }

  //    Twitter以外のアプリからのデータが来た。
  //    無視する。
}

@end
