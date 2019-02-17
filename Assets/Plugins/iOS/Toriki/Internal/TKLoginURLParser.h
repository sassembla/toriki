NS_ASSUME_NONNULL_BEGIN

@class TKAuthConfig;

@interface TKLoginURLParser : NSObject

- (instancetype)initWithKey:(NSString *)key secret:(NSString *)secret;

- (BOOL)isTwitterKitRedirectURL:(NSURL *)url;
- (BOOL)hasValidURLScheme;
- (NSURL *)twitterAuthorizeURL;
- (BOOL)isMobileSSOSuccessURL:(NSURL *)url;
- (BOOL)isMobileSSOCancelURL:(NSURL *)url;
- (NSDictionary *)parametersForSSOURL:(NSURL *)url;

@end

NS_ASSUME_NONNULL_END
