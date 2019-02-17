#import <Foundation/Foundation.h>
#import "AppDelegateListener.h"
#include "RegisterMonoModules.h"

@interface Toriki : NSObject <AppDelegateListener>
+ (Toriki *)sharedInstance;
@end
