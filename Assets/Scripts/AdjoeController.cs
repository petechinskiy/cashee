using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.adjoe.sdk;
using System;

public class AdjoeController : MonoBehaviour
{
    private const string SDK_HASH = "d0013f0948e21cef3f0be6de36f7d90c";

    public Action _onOfferwallShow;

    public void Init(string userId, string network, string channel, string subpublisher, Action onOfferwallShow)
    {
        var playtimeParams = new PlaytimeParams
        {
            uaNetwork = network,
            uaChannel = channel,
            uaSubPublisherEncrypted = subpublisher,
        };

        var options = new PlaytimeOptions()
        {
            userId = userId,
            playtimeParams = playtimeParams
        };

        _onOfferwallShow = onOfferwallShow;

        Playtime.Init(SDK_HASH, options, AdjoeInitialisationSuccess, AdjoeInitialisationError);
    }

    public void AdjoeInitialisationSuccess()
    {
        Debug.Log("Adjoe initialization successful.");
    }

    public void AdjoeInitialisationError(Exception exception)
    {
        Debug.LogError($"Adjoe initializing is fault: {exception.Message}");
    }

    public void ShowOfferwall()
    {
        Playtime.ShowCatalog();

        _onOfferwallShow?.Invoke();
    }
}
