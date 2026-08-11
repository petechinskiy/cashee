using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.adjoe.sdk;
using System;

public class AdjoeController : MonoBehaviour
{
    private const string SDK_HASH = "d0013f0948e21cef3f0be6de36f7d90c";

    public Action _onOfferwallShow;
    private PlaytimeOptions _playtimeOptions;

    public void Init(string userId, string network, string channel, string subpublisher, Action onOfferwallShow)
    {
        var options = new PlaytimeOptions();

        PlaytimeParams adjoeParams = new PlaytimeParams();
        adjoeParams.SetUaNetwork(network)
                .SetUaChannel(channel)
                .SetUaSubPublisherCleartext(subpublisher);

        options.SetPlaytimeParams(adjoeParams);

        var userProfile = new PlaytimeUserProfile();
        //userProfile.SetGender(PlaytimeGender.FEMALE)
        //        .SetBirthday(DateTime.Parse("1990-05-15T00:00:00.000Z"));

        //options.SetPlaytimeUserProfile(userProfile);

        options.SetUserId(userId);
        options.SetSdkHash(SDK_HASH);

        _playtimeOptions = options;

        Playtime.SetPlaytimeOptions(
            options,
            () =>
            {
                Playtime.GetStatus((status) =>
                {
                    Debug.Log($"ADJOE - Current SDK Status: {status}");

                    if (status != null && status.IsInitialized)
                    {
                        // Step 2: Show the Playtime catalog
                    }
                    else
                    {
                        Debug.LogWarning("ADJOE - SDK is not initialized yet. Please try again in a moment.");
                    }
                }, (error) =>
                {
                    Debug.LogWarning($"Adjoe status error: {error.Message}");
                });
            },
            error =>
            {
                Debug.LogError($"ADJOE - SetPlaytimeOptions Error: {error.Message}");
            }
        );
    }

    public void ShowOfferwall()
    {
        Playtime.ShowCatalogWithOptions(
            _playtimeOptions,
            () =>
            {
                Debug.Log("ShowCatalogWithOptions Success!");
            },
            error =>
            {
                Debug.LogError($"ShowCatalogWithOptions Error: {error.Message}");
            }
        );

        _onOfferwallShow?.Invoke();
    }
}
