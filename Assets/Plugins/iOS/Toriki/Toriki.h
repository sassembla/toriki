#import <Foundation/Foundation.h>
#import "AppDelegateListener.h"

@interface Toriki : NSObject <AppDelegateListener>
+ (Toriki *)sharedInstance;
@end
