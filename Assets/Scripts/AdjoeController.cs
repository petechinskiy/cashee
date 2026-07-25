using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using io.adjoe.sdk;
using System;

public class AdjoeController : MonoBehaviour
{
    private const string SDK_HASH = "8b50f8826e52379f6d9819db7d7b2498";

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
