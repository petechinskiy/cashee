using Firebase;
using Firebase.Analytics;
using UnityEngine;

public class FirebaseManager : Singleton<FirebaseManager>
{
    protected override void Awake()
    {
        base.Awake();

        return;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Debug.Log("Firebase Analytics ready");
            }
            else
            {
                Debug.LogError("Firebase dependencies not resolved: " + status);
            }
        });
    }

    public void SendRevenueEvent(float value)
    {
        var parameters = new[]
        {
            new Parameter("ad_platform", "adjoe"),
            new Parameter("ad_source", "adjoe"),
            new Parameter("ad_unit_name", "bucksup"),
            new Parameter("ad_format", "offerwall"),
            new Parameter("value", value),
            new Parameter("currency", "USD"),
        };

        FirebaseAnalytics.LogEvent("ad_impression", parameters);
    }
}
