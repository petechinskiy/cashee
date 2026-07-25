using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EarnTab : MonoBehaviour
{
    public Animator anim;

    [SerializeField] private GameObject _notAvailableWindow;
    [SerializeField] private Button _webViewCloseButton;
    [SerializeField] private Button _bitlabsButton;
    [SerializeField] private Button _adsButton;
    [SerializeField] private Button _offertoroButton;

    private TextMeshProUGUI _adButtonText;
    private float _nextAdTime;
    private string _defaultAdButtonText;

    private const float ADS_COOLDOWN_TIME = 30f;

    public void Init(Action onOffertoroShow)
    {
        var appController = FindFirstObjectByType<ApplicationController>();

        _adButtonText = _adsButton.GetComponentInChildren<TextMeshProUGUI>();
        _defaultAdButtonText = _adButtonText.text;

        _webViewCloseButton.onClick.AddListener(() =>
        {
            _webViewCloseButton.gameObject.SetActive(false);
        });

        _bitlabsButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        _adsButton.onClick.AddListener(() =>
        {
            //_cleverAdsController.ShowAds();
        });

        _offertoroButton.onClick.AddListener(() =>
        {
            onOffertoroShow.Invoke();
            _webViewCloseButton.gameObject.SetActive(true);
        });
    }

    private void OnEnable()
    {
        //_cleverAdsController.OnAdShowed += OnAdShowed;
    }

    private void OnDisable()
    {
        anim.enabled = true;

        //_cleverAdsController.OnAdShowed -= OnAdShowed;
    }

    private void Update()
    {
        int remainingAdTime = (int)(_nextAdTime - Time.time);

        if (remainingAdTime > 0)
        {
            _adsButton.interactable = false;
            _adButtonText.text = $"{remainingAdTime} sec";
        }
        else
        {
            _adsButton.interactable = true;
            _adButtonText.text = _defaultAdButtonText;
        }
    }

    private void OnAdShowed()
    {
        _nextAdTime = Time.time + ADS_COOLDOWN_TIME;
    }

    public void DisableAnimator()
    {
        anim.enabled = false;
    }
}
