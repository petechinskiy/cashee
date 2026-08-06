#import <Foundation/Foundation.h>
#import <PlaytimeMonetize/PlaytimeMonetize-Swift.h>

extern "C" {

    static PlaytimeOptions *playtimeOptionsFromJSON(
        const char* optionsJSON,
        const char* methodName,
        void (*onError)(const char*)
    ) {
        NSString* optionsJSONStr = optionsJSON ? [NSString stringWithUTF8String:optionsJSON] : nil;
        
        NSError *error = nil;
        NSData *optionsData = [optionsJSONStr dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *optionsDict = [NSJSONSerialization JSONObjectWithData:optionsData options:0 error:&error];
        
        if (error != nil || ![optionsDict isKindOfClass:[NSDictionary class]]) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for %s, error: %@", methodName, error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return nil;
        }
        
        NSMutableDictionary *mutableParams = [optionsDict mutableCopy];
        mutableParams[@"wrapper"] = @"unity";
        
        PlaytimeOptions *playtimeOptions = [[PlaytimeOptions alloc] initWithJSONObject:mutableParams error:&error];
        
        if (playtimeOptions == nil || error != nil) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for %s, error: %@", methodName, error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return nil;
        }
        
        return playtimeOptions;
    }

    void PlaytimeStudio_getCampaigns(
        const char* optionsJSON,
        void (*onSuccess)(const char*),
        void (*onError)(const char*)
    ) {
        PlaytimeOptions *playtimeOptions = playtimeOptionsFromJSON(optionsJSON, "getCampaigns", onError);
        if (playtimeOptions == nil) {
            return;
        }
        
        [PlaytimeStudio getCampaignsWithOptions:playtimeOptions
                                       completionHandler: ^(PlaytimeCampaignsResponse * _Nullable response, NSError * _Nullable error) {
            if (error != nil) {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
                return;
            }
            
            NSDictionary *responseDict = [response toJSONObject];
            if (responseDict == nil) {
                if (onError) {
                    NSString *errorMessage = [NSString stringWithFormat:@"Error parsing campaigns response, error: %@", error.localizedDescription];
                    onError([errorMessage UTF8String]);
                }
                return;
            }
            
            NSData *responseData = [NSJSONSerialization dataWithJSONObject:responseDict options:0 error:&error];
            if (error != nil) {
                if (onError) {
                    NSString *errorMessage = [NSString stringWithFormat:@"Error converting response to JSON, error: %@", error.localizedDescription];
                    onError([errorMessage UTF8String]);
                }
                return;
            }
            
            NSString *responseJSON = [[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding];
            if (onSuccess) {
                onSuccess([responseJSON UTF8String]);
            }
        }];
    }

    void PlaytimeStudio_getCampaignsWithTokens(
        const char* tokensString,
        const char* optionsJSON,
        void (*onSuccess)(const char*),
        void (*onError)(const char*)
    ) {
        NSString* tokensStringStr = tokensString ? [NSString stringWithUTF8String:tokensString] : nil;
        
        PlaytimeOptions *playtimeOptions = playtimeOptionsFromJSON(optionsJSON, "getCampaignsWithTokens", onError);
        if (playtimeOptions == nil) {
            return;
        }
        
        // Parse tokens from comma-separated string
        NSMutableArray<NSString *> *tokens = [NSMutableArray array];
        if (tokensStringStr && tokensStringStr.length > 0) {
            NSArray *tokenComponents = [tokensStringStr componentsSeparatedByString:@","];
            for (NSString *token in tokenComponents) {
                NSString *trimmedToken = [token stringByTrimmingCharactersInSet:[NSCharacterSet whitespaceAndNewlineCharacterSet]];
                if (trimmedToken.length > 0) {
                    [tokens addObject:trimmedToken];
                }
            }
        }
        
        [PlaytimeStudio getCampaignsWithTokens:tokens
                                       options:playtimeOptions
                             completionHandler: ^(PlaytimeCampaignsResponse * _Nullable response, NSError * _Nullable error) {
            if (error != nil) {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
                return;
            }
            
            NSDictionary *responseDict = [response toJSONObject];
            if (responseDict == nil) {
                if (onError) {
                    NSString *errorMessage = [NSString stringWithFormat:@"Error parsing campaigns with tokens response, error: %@", error.localizedDescription];
                    onError([errorMessage UTF8String]);
                }
                return;
            }
            
            NSData *responseData = [NSJSONSerialization dataWithJSONObject:responseDict options:0 error:&error];
            if (error != nil) {
                if (onError) {
                    NSString *errorMessage = [NSString stringWithFormat:@"Error converting response to JSON, error: %@", error.localizedDescription];
                    onError([errorMessage UTF8String]);
                }
                return;
            }
            
            NSString *responseJSON = [[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding];
            if (onSuccess) {
                onSuccess([responseJSON UTF8String]);
            }
        }];
    }

    void PlaytimeStudio_getInstalledCampaigns(
        const char* optionsJSON,
        void (*onSuccess)(const char*),
        void (*onError)(const char*)
    ) {
        PlaytimeOptions *playtimeOptions = playtimeOptionsFromJSON(optionsJSON, "getInstalledCampaigns", onError);
        if (playtimeOptions == nil) {
            return;
        }
        
        [PlaytimeStudio getInstalledCampaignsWithOptions:playtimeOptions
                                   completionHandler: ^(PlaytimeCampaignsResponse * _Nullable response, NSError * _Nullable error) {
            if (error != nil) {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
                return;
            }
            
            NSDictionary *responseDict = [response toJSONObject];
            if (responseDict == nil) {
                if (onError) {
                    NSString *errorMessage = [NSString stringWithFormat:@"Error parsing installed campaigns response, error: %@", error.localizedDescription];
                    onError([errorMessage UTF8String]);
                }
                return;
            }
            
            NSData *responseData = [NSJSONSerialization dataWithJSONObject:responseDict options:0 error:&error];
            if (error != nil) {
                if (onError) {
                    NSString *errorMessage = [NSString stringWithFormat:@"Error converting response to JSON, error: %@", error.localizedDescription];
                    onError([errorMessage UTF8String]);
                }
                return;
            }
            
            NSString *responseJSON = [[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding];
            if (onSuccess) {
                onSuccess([responseJSON UTF8String]);
            }
        }];
    }

    void PlaytimeStudio_openInStore(
        const char* campaignJSON,
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        NSString* campaignJSONStr = campaignJSON ? [NSString stringWithUTF8String:campaignJSON] : nil;
        
        NSError *error = nil;
        NSData *campaignData = [campaignJSONStr dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *campaignDict = [NSJSONSerialization JSONObjectWithData:campaignData options:0 error:&error];

        if (error != nil || ![campaignDict isKindOfClass:[NSDictionary class]]) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for openInStore, error: %@", error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return;
        }
        
        // JSON converted from C# by default have empty strings when null should be
        // Remove installedAt if it's an empty string to avoid the native SDK to assume the campaign is installed
        if (campaignDict[@"installedAt"] && [campaignDict[@"installedAt"] isKindOfClass:[NSString class]] && [campaignDict[@"installedAt"] length] == 0) {
            NSMutableDictionary *mutableCampaignDict = [campaignDict mutableCopy];
            [mutableCampaignDict removeObjectForKey:@"installedAt"];
            campaignDict = mutableCampaignDict;
        }
        
        PlaytimeCampaign *campaign = [[PlaytimeCampaign alloc] initWithJSONObject:campaignDict error:&error];
        
        if (campaign == nil || error != nil) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for openInStore, error: %@", error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return;
        }
        
        [PlaytimeStudio openInStoreWithCampaign:campaign
                  completionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

    void PlaytimeStudio_openChatbotWithCampaign(
        const char* campaignJSON,
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        NSString* campaignJSONStr = [NSString stringWithUTF8String:campaignJSON];
        
        NSError *error = nil;
        NSData *campaignData = [campaignJSONStr dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *campaignDict = [NSJSONSerialization JSONObjectWithData:campaignData options:0 error:&error];

        if ((error != nil) || (![campaignDict isKindOfClass:[NSDictionary class]] && !campaignJSON)) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for openChatbot, error: %@", error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return;
        }
        
        PlaytimeCampaign *campaign = [[PlaytimeCampaign alloc] initWithJSONObject:campaignDict error:&error];
        
        if (error != nil) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for openChatbot, error: %@", error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return;
        }
        
        [PlaytimeStudio openChatbotWithCampaign:campaign
                  completionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

    void PlaytimeStudio_openChatbot(
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        [PlaytimeStudio openChatbotWithCampaign:nil
                  completionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

    void PlaytimeStudio_openInstalledCampaign(
        const char* campaignJSON,
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        NSString* campaignJSONStr = campaignJSON ? [NSString stringWithUTF8String:campaignJSON] : nil;
        
        NSError *error = nil;
        NSData *campaignData = [campaignJSONStr dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *campaignDict = [NSJSONSerialization JSONObjectWithData:campaignData options:0 error:&error];
        
        if (error != nil || ![campaignDict isKindOfClass:[NSDictionary class]]) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for openInstalledCampaign, error: %@", error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return;
        }
        
        PlaytimeCampaign *campaign = [[PlaytimeCampaign alloc] initWithJSONObject:campaignDict error:&error];
        
        if (campaign == nil || error != nil) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for openInstalledCampaign, error: %@", error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return;
        }
        
        [PlaytimeStudio openInstalledCampaignWithCampaign:campaign
                  completionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

    void PlaytimeStudio_getPermissions(
        void (*onSuccess)(const char*),
        void (*onError)(const char*)
    ) {
        if (onError) {
            NSString *errorMessage = [NSString stringWithFormat:@"Get permissions is not supported for iOS."];
            onError([errorMessage UTF8String]);
        }
    }
    
    void PlaytimeStudio_resetRewardsConnect(
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        [PlaytimeStudio resetRewardsConnectWithCompletionHandler:^(NSError * _Nullable error) {
            if (error != nil) {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
                return;
            }
            
            if (onSuccess) {
                onSuccess();
            }
        }];
    }

     void PlaytimeStudio_registerRewardsConnect(
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        [PlaytimeStudio registerRewardsConnectWithCompletionHandler:^(NSError * _Nullable error) {
            if (error != nil) {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
                return;
            }
            
            if (onSuccess) {
                onSuccess();
            }
        }];
    }

    void PlaytimeStudio_showPermissionsPrompt(
        void (*onSuccess)(const char*),
        void (*onError)(const char*)
    ) {
        [PlaytimeStudio showPermissionsPromptWithCompletionHandler:^(PlaytimePermissionsResponse * _Nullable response, NSError * _Nullable error) {
            if (error != nil) {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
                return;
            }
            
            NSDictionary *responseDict = [response toJSONObject];
            if (responseDict == nil) {
                if (onError) {
                    NSString *errorMessage = [NSString stringWithFormat:@"Error parsing permissions prompt response, error: %@", error.localizedDescription];
                    onError([errorMessage UTF8String]);
                }
                return;
            }
            
            NSError *jsonError = nil;
            NSData *responseData = [NSJSONSerialization dataWithJSONObject:responseDict options:0 error:&jsonError];
            if (jsonError != nil) {
                if (onError) {
                    NSString *errorMessage = [NSString stringWithFormat:@"Error converting response to JSON, error: %@", jsonError.localizedDescription];
                    onError([errorMessage UTF8String]);
                }
                return;
            }
            
            NSString *responseJSON = [[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding];
            if (onSuccess) {
                onSuccess([responseJSON UTF8String]);
            }
        }];
    }

    void Playtime_showCatalogWithOptions(
        const char* optionsJSON,
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        PlaytimeOptions *playtimeOptions = playtimeOptionsFromJSON(optionsJSON, "showCatalogWithOptions", onError);
        if (playtimeOptions == nil) {
            return;
        }
        
        [Playtime showCatalogWithOptions:playtimeOptions
                   completionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

    void Playtime_setPlaytimeOptions(
        const char* optionsJSON,
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        PlaytimeOptions *playtimeOptions = playtimeOptionsFromJSON(optionsJSON, "setPlaytimeOptions", onError);
        if (playtimeOptions == nil) {
            return;
        }
        
        [Playtime setPlaytimeOptionsWithOptions:playtimeOptions
                   completionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

    void Playtime_getStatus(
        void (*onSuccess)(const char*),
        void (*onError)(const char*)
    ) {
        [Playtime getStatusWithCompletionHandler:^(PlaytimeStatus * _Nullable response, NSError * _Nullable error) {
            if (error != nil) {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
                return;
            }
            
            NSDictionary *responseDict = [response toJSONObject];
            if (responseDict == nil) {
                if (onError) {
                    NSString *errorMessage = [NSString stringWithFormat:@"Error parsing status response, error: %@", error.localizedDescription];
                    onError([errorMessage UTF8String]);
                }
                return;
            }
            
            NSError *jsonError = nil;
            NSData *responseData = [NSJSONSerialization dataWithJSONObject:responseDict options:0 error:&jsonError];
            if (jsonError != nil) {
                if (onError) {
                    NSString *errorMessage = [NSString stringWithFormat:@"Error converting response to JSON, error: %@", jsonError.localizedDescription];
                    onError([errorMessage UTF8String]);
                }
                return;
            }
            
            NSString *responseJSON = [[NSString alloc] initWithData:responseData encoding:NSUTF8StringEncoding];
            if (onSuccess) {
                onSuccess([responseJSON UTF8String]);
            }
        }];
    }

    void PlaytimeStudio_showInstalledApps(
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        [PlaytimeStudio showInstalledAppsWithCompletionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

    void PlaytimeStudio_showAppDetails(
        const char* campaignJSON,
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        NSString* campaignJSONStr = campaignJSON ? [NSString stringWithUTF8String:campaignJSON] : nil;
        
        NSError *error = nil;
        NSData *campaignData = [campaignJSONStr dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *campaignDict = [NSJSONSerialization JSONObjectWithData:campaignData options:0 error:&error];
        
        if (error != nil || ![campaignDict isKindOfClass:[NSDictionary class]]) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for showAppDetails, error: %@", error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return;
        }
        
        PlaytimeCampaign *campaign = [[PlaytimeCampaign alloc] initWithJSONObject:campaignDict error:&error];
        
        if (campaign == nil || error != nil) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for showAppDetails, error: %@", error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return;
        }

        [PlaytimeStudio showAppDetailsWithCampaign:campaign
                  completionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

    void PlaytimeStudio_showAppDetailsWithToken(
        const char* token,
        const char* appId,
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        NSString* tokenStr = token ? [NSString stringWithUTF8String:token] : nil;
        NSString* appIdStr = appId ? [NSString stringWithUTF8String:appId] : nil;
        
        if (tokenStr == nil || appIdStr == nil) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for showAppDetails"];
                onError([errorMessage UTF8String]);
            }
            return;
        }

        [PlaytimeStudio showAppDetailsWithCampaignAppId:appIdStr
                    token:tokenStr
                    completionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

    void Playtime_getUserId(
        void (*onSuccess)(const char*),
        void (*onError)(const char*)
    ) {
        [Playtime getUserIdWithCompletionHandler:^(NSString * _Nullable userId, NSError * _Nullable error) {
            if (error != nil) {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
                return;
            }
            
            if (onSuccess) {
                const char* userIdCString = userId != nil ? [userId UTF8String] : NULL;
                onSuccess(userIdCString);
            }
        }];
    }

    void PlaytimeStudio_executeEngagement(
        const char* campaignJSON,
        const char* engagementType,
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        NSString* campaignJSONStr = campaignJSON ? [NSString stringWithUTF8String:campaignJSON] : nil;
        NSString* engagementTypeStr = engagementType ? [NSString stringWithUTF8String:engagementType] : nil;
        
        NSError *error = nil;
        NSData *campaignData = [campaignJSONStr dataUsingEncoding:NSUTF8StringEncoding];
        NSDictionary *campaignDict = [NSJSONSerialization JSONObjectWithData:campaignData options:0 error:&error];
        PlaytimeEngagementType rawEngagementType = PlaytimeEngagementTypeDefault;

        if ([engagementTypeStr isEqualToString:@"engaged"]) {
            rawEngagementType = PlaytimeEngagementTypeEngaged;
        }

        if (error != nil || ![campaignDict isKindOfClass:[NSDictionary class]]) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for executeEngagement, error: %@", error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return;
        }
        
        // JSON converted from C# by default have empty strings when null should be
        // Remove installedAt if it's an empty string to avoid the native SDK to assume the campaign is installed
        if (campaignDict[@"installedAt"] && [campaignDict[@"installedAt"] isKindOfClass:[NSString class]] && [campaignDict[@"installedAt"] length] == 0) {
            NSMutableDictionary *mutableCampaignDict = [campaignDict mutableCopy];
            [mutableCampaignDict removeObjectForKey:@"installedAt"];
            campaignDict = mutableCampaignDict;
        }
        
        PlaytimeCampaign *campaign = [[PlaytimeCampaign alloc] initWithJSONObject:campaignDict error:&error];
        
        if (campaign == nil || error != nil) {
            if (onError) {
                NSString *errorMessage = [NSString stringWithFormat:@"Invalid parameters for openInStore, error: %@", error.localizedDescription];
                onError([errorMessage UTF8String]);
            }
            return;
        }
        
        [PlaytimeStudio executeEngagementFor:campaign
                  engagementType:rawEngagementType
                  completionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

    void Playtime_teardown(
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        [Playtime teardownWithCompletionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

    void PlaytimeStudio_executeEngagementWithToken(
        const char* appID,
        const char* token,
        void (*onSuccess)(void),
        void (*onError)(const char*)
    ) {
        NSString* appIDStr = appID ? [NSString stringWithUTF8String:appID] : nil;
        NSString* tokenStr = token ? [NSString stringWithUTF8String:token] : nil;

        [PlaytimeStudio executeEngagementWithAppID:appIDStr
                  token:tokenStr
                  completionHandler:^(NSError * _Nullable error) {
            if (error == nil) {
                if (onSuccess) {
                    onSuccess();
                }
            } else {
                if (onError) {
                    onError([error.localizedDescription UTF8String]);
                }
            }
        }];
    }

}
