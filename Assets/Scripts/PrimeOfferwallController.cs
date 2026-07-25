using UnityEngine;
using UnityEngine.UI;

public class PrimeOfferwallController : MonoBehaviour
{
    [SerializeField] private Button _closeButton;

    private UniWebView _webView;

    private readonly string _uri = "https://monetize.primeearn.com/offers?app=FLFpmq6QgY";

    private void Awake()
    {
        _webView = new GameObject("UniWebView").AddComponent<UniWebView>();
        _webView.Frame = new Rect(0, 150, Screen.width, Screen.height - 150);
        _webView.SetBackButtonEnabled(false);

        _closeButton.onClick.AddListener(() => Hide());
    }

    public void ShowOfferwall()
    {
        _webView.Load($"{_uri}&uuid={ApplicationController.Instance.DeviceId}&maid={ApplicationController.Instance.GpsAdid}");
        _webView.Show();
        _closeButton.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _webView.Hide();
        _closeButton.gameObject.SetActive(false);
    }
}
