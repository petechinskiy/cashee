using System;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class CoinsScale : MonoBehaviour
    {
        [SerializeField] private Transform coinTransform;
        [SerializeField] private float scaleFactor = 1.2f;
        [SerializeField] private float pulseDuration = 0.15f;

        private Tween _scaleTween;
        private Vector3 _originalScale = Vector3.one;

        public static event Action OnCoinPulseRequested;

        private void Awake()
        {
            if (coinTransform == null)
                coinTransform = transform;

            _originalScale = coinTransform.localScale;
        }

        private void OnEnable()
        {
            OnCoinPulseRequested += HandlePulseRequested;
        }

        private void OnDisable()
        {
            OnCoinPulseRequested -= HandlePulseRequested;
            _scaleTween?.Kill();
        }

        public static void RequestPulse()
        {
            OnCoinPulseRequested?.Invoke();
        }

        private void HandlePulseRequested()
        {
            if (coinTransform == null)
                return;

            _scaleTween?.Kill();
            coinTransform.localScale = _originalScale;

            _scaleTween = coinTransform.DOScale(_originalScale * scaleFactor, pulseDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _scaleTween = coinTransform.DOScale(_originalScale, pulseDuration).SetEase(Ease.InQuad);
                });
        }
    }
}