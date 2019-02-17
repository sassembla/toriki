#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

FOUNDATION_EXTERN NSString *const TKErrorDomain;

typedef NS_ENUM(NSInteger, TKErrorCode) {

    /**
     *  Unknown error.
     */
    TKErrorCodeUnknown = -1,

    /**
     *  Authentication has not been set up yet. You must call -[TKTwitter logInWithCompletion:] or -[TKTwitter logInGuestWithCompletion:]
     */
    TKErrorCodeNoAuthentication = 0,

    /**
     *  Twitter has not been initialized yet. Call +[Fabric with:@[TwitterKit]] or -[TKTwitter startWithConsumerKey:consumerSecret:].
     */
    TKErrorCodeNotInitialized = 1,

    /**
     *  User has declined to grant permission to information such as their email address.
     */
    TKErrorCodeUserDeclinedPermission = 2,

    /**
     *  User has granted permission to their email address but no address is associated with their account.
     */
    TKErrorCodeUserHasNoEmailAddress = 3,

    /**
     *  A resource has been requested by ID, but that ID was not found.
     */
    TKErrorCodeInvalidResourceID = 4,

    /**
     *  A request has been issued for an invalid URL.
     */
    TKErrorCodeInvalidURL = 5,

    /**
     *  Type mismatch in parsing JSON from the Twitter API.
     */
    TKErrorCodeMismatchedJSONType = 6,

    /**
     *  Fail to save to keychain.
     */
    TKErrorCodeKeychainSerializationFailure = 7,

    /**
     *  Fail to save to disk.
     */
    TKErrorCodeDiskSerializationError = 8,

    /**
     *  Error authenticating with the webview.
     */
    TKErrorCodeWebViewError = 9,

    /**
     *  A required parameter is missing.
     */
    TKErrorCodeMissingParameter = 10
};

FOUNDATION_EXTERN NSString *const TKLogInErrorDomain;

typedef NS_ENUM(NSInteger, TKLogInErrorCode) {

    /**
     * Unknown error.
     */
    TKLogInErrorCodeUnknown = -1,

    /**
     * User denied login.
     */
    TKLogInErrorCodeDenied = 0,

    /**
     * User canceled login.
     */
    TKLogInErrorCodeCancelled = 1,

    /**
     * No Twitter account found.
     */
    TKLogInErrorCodeNoAccounts = 2,

    /**
     * Reverse auth with linked account failed.
     */
    TKLogInErrorCodeReverseAuthFailed = 3,

    /**
     *  Refreshing session tokens failed.
     */
    TKLogInErrorCodeCannotRefreshSession = 4,

    /**
     *  No such session or session is not tracked
     *  in the associated session store.
     */
    TKLogInErrorCodeSessionNotFound = 5,

    /**
     * The login request failed.
     */
    TKLogInErrorCodeFailed = 6,

    /**
     * The system account credentials are no longer valid and the
     * user will need to update their credentials in the Settings app.
     */
    TKLogInErrorCodeSystemAccountCredentialsInvalid = 7,

    /**
     *  There was no Twitter iOS app installed to attemp
     *  the Mobile SSO flow.
     */
    TKLoginErrorNoTwitterApp = 8,
};

NS_ASSUME_NONNULL_END
