#import "TKTwitter.h"
#import "Toriki.h"

static const char * TKInternalGameObject = "TorikiGameObject";
static const char * TKUnityAPIMethodLogInComplete = "LoginComplete";
static const char * TKUnityAPIMethodLogInFailed = "LoginFailed";


static char * cStringCopy(const char *string)
{
    if (string == NULL)
        return NULL;
    
    char *res = (char *)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

static NSString * const NSStringFromCString(const char *string)
{
    if (string != NULL) {
        return [NSString stringWithUTF8String:string];
    } else {
        return nil;
    }
}

static char * serializedJSONFromNSDictionary(NSDictionary *dictionary)
{
    if (!dictionary) {
        return NULL;
    }
    
    NSData *serializedData = [NSJSONSerialization dataWithJSONObject:dictionary options:0 error:nil];
    NSString *serializedJSONString = [[NSString alloc] initWithData:serializedData encoding:NSUTF8StringEncoding];
    return cStringCopy([serializedJSONString UTF8String]);
}




static NSDictionary * NSDictionaryFromError(NSError *error)
{
    return @{ @"code": @(error.code), @"message": error.localizedDescription };
}

static Toriki *_instance = [Toriki sharedInstance];

@implementation Toriki

+ (Toriki *)sharedInstance
{
    if(_instance == nil) {
        _instance = [[Toriki alloc] init];
    }

    return _instance;
}

- (id)init
{
    if(_instance != nil) {
        return _instance;
    }
    
    if ((self = [super init])) {
        _instance = self;
        UnityRegisterAppDelegateListener(self);
    }
    
    return self;
}


/**
    Twitterアプリからの起動
 */
- (void)onOpenURL:(NSNotification *)notification
{
    [[TKTwitter sharedInstance]
     onAppLaunch:notification.userInfo[@"url"]
     sourceApplication:notification.userInfo[@"sourceApplication"]];
}

@end


extern "C" {
    /**
        Unity -> Twitterアプリの起動
     */
    void TwitterInit(const char *consumerKey, const char *consumerSecret)
    {
        [[TKTwitter sharedInstance] startWithConsumerKey:NSStringFromCString(consumerKey) consumerSecret:NSStringFromCString(consumerSecret)];
    }

    /**
        Unity -> Twitterのトークン取得処理
     */
    void TwitterLogIn()
    {
        [[TKTwitter sharedInstance] attemptLogin:^(
                                   NSString *username,
                                   NSString *accessToken,
                                   NSString* accessSecret,
                                   NSError *error
                                   ) {
            if (!error) {
//                トークンを受け取ったのでUnity側の呼び出し処理を行う
                NSDictionary *sessionDictionary =
                        @{
                          @"nickname": username,
                          @"accessToken": accessToken,
                          @"accessSecret": accessSecret
                          };
                char *serializedSession = serializedJSONFromNSDictionary(sessionDictionary);

//                NSLog(@"sessionDictionary:%@", sessionDictionary);
                UnitySendMessage(TKInternalGameObject, TKUnityAPIMethodLogInComplete, serializedSession);
                free(serializedSession);
            } else {
                NSDictionary *errorDictionary = NSDictionaryFromError(error);
                char *serializedError = serializedJSONFromNSDictionary(errorDictionary);
                UnitySendMessage(TKInternalGameObject, TKUnityAPIMethodLogInFailed, serializedError);
                free(serializedError);
            }
        }];
    }
}
