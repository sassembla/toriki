#import "TKLoginURLParser.h"

@interface TKLoginURLParser ()

@property (nonatomic, copy) NSString *twitterKitURLScheme;
@property (nonatomic, copy) NSString *twitterAuthURL;

@end

@implementation TKLoginURLParser

- (instancetype)initWithKey:(NSString *)key secret:(NSString *)secret
{
    if (self = [super init]) {
        self.twitterKitURLScheme = [NSString stringWithFormat:@"twitterkit-%@", key];
        self.twitterAuthURL = [NSString stringWithFormat:@"twitterauth://authorize?consumer_key=%@&consumer_secret=%@&oauth_callback=%@", key, secret, self.twitterKitURLScheme];
    }
    return self;
}

- (NSString *)percentUnescapedQueryStringWithString:(NSString *)string encoding:(NSStringEncoding)encoding
{
    return [string stringByRemovingPercentEncoding];
}

- (NSDictionary *)parametersFromQueryString:(NSString *)queryString
{
    NSArray *pairs = [queryString componentsSeparatedByString:@"&"];
    NSMutableDictionary *parameters = [NSMutableDictionary dictionary];
    [pairs enumerateObjectsUsingBlock:^(NSString *keyValueStr, NSUInteger idx, BOOL *stop) {
        NSArray *keyValue = [keyValueStr componentsSeparatedByString:@"="];
        if ([keyValue count] >= 2) {
            NSString *key = [self percentUnescapedQueryStringWithString:keyValue[0] encoding:NSUTF8StringEncoding];
            NSString *value = [self percentUnescapedQueryStringWithString:keyValue[1] encoding:NSUTF8StringEncoding];
            parameters[key] = value;
        }
    }];
    return parameters;
}


- (BOOL)isMobileSSOSuccessURL:(NSURL *)url
{
    BOOL properScheme = [self isTwitterKitRedirectURL:url];

    NSDictionary *parameters = [self parametersFromQueryString:url.host];
    NSArray *keys = [parameters allKeys];
    BOOL successState = [keys containsObject:@"secret"] && [keys containsObject:@"token"] && [keys containsObject:@"username"] && properScheme;

    BOOL isSuccessURL = successState && properScheme;

    return isSuccessURL;
}

- (BOOL)isMobileSSOCancelURL:(NSURL *)url
{
    BOOL properScheme = [self isTwitterKitRedirectURL:url];
    BOOL cancelState = (url.host == nil) && properScheme;

    BOOL isCancelURL = properScheme && cancelState;

    return isCancelURL;
}

- (NSDictionary *)parametersForSSOURL:(NSURL *)url
{
    return [self parametersFromQueryString:url.host];
}

- (BOOL)isTwitterKitRedirectURL:(NSURL *)url
{
    return [self isTwitterKitURLScheme:url.scheme];
}

- (BOOL)hasValidURLScheme
{
    return ([self appSpecificURLScheme] != nil);
}

- (NSURL *)twitterAuthorizeURL
{
    return [NSURL URLWithString:self.twitterAuthURL];
}

- (BOOL)isTwitterKitURLScheme:(NSString *)scheme
{
    // The Twitter API will redirect to a lowercase version of the
    // URL that we pass to them
    return [scheme caseInsensitiveCompare:self.twitterKitURLScheme] == NSOrderedSame;
}

- (NSString *)appSpecificURLScheme
{
    NSString *matchingScheme;
    NSBundle* mainBundle = [NSBundle mainBundle];
    NSDictionary *infoPlist = mainBundle.infoDictionary;
    
    NSArray *urlTypes = [infoPlist objectForKey:@"CFBundleURLTypes"];

    for (NSDictionary *schemeDetails in urlTypes) {
        NSPredicate *predicate = [NSPredicate predicateWithBlock:^BOOL(id _Nullable evaluatedObject, NSDictionary<NSString *, id> *_Nullable bindings) {
            NSString *scheme = (NSString *)evaluatedObject;
            return (scheme) ? [self isTwitterKitURLScheme:scheme] : NO;
        }];

        NSArray *filteredArray = [[schemeDetails objectForKey:@"CFBundleURLSchemes"] filteredArrayUsingPredicate:predicate];
        if ([filteredArray count] > 0) {
            matchingScheme = [filteredArray firstObject];
        }
    }

    return matchingScheme;
}

@end
