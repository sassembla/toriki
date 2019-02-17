NS_ASSUME_NONNULL_BEGIN

//ログイン処理完了時に発行するイベント型
typedef void (^TKLogInCompletion)(
                                  NSString *_Nullable username,
                                  NSString *_Nullable accessToken,
                                  NSString *_Nullable accessSecret,
                                  NSError *_Nullable error);

@interface TKTwitter : NSObject
@property (nonatomic, copy, readonly) NSString *consumerKey;
@property (nonatomic, copy, readonly) NSString *consumerSecret;

+ (TKTwitter *)sharedInstance;

- (void)startWithConsumerKey:(NSString *)consumerKey consumerSecret:(NSString *)consumerSecret;
- (void)attemptLogin:(TKLogInCompletion)completion;

- (void)onAppLaunch:(NSURL *)url sourceApplication:(NSString *)sourceApplication;

@end

NS_ASSUME_NONNULL_END
