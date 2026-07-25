using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using TMPro;

public enum VerificationState
{
    Process,
    Success,
    Failed
}

[System.Serializable]
public class VerificationStateEntry
{
    [SerializeField] private VerificationState _state;
    [SerializeField] private string _localizationKey;
    [SerializeField] private Sprite _icon;

    public VerificationState State => _state;
    public string LocalizationKey => _localizationKey;
    public Sprite Icon => _icon;
}

public class UIVerifyingWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stateText;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Image _stateImage;
    [SerializeField] private VerificationStateEntry[] _stateEntries;

    private System.Action _onClose;
    private Tween _waitingTween;

    private void Awake()
    {
        _waitingTween = _stateImage.transform.DOLocalRotate(Vector3.forward * 360f, 2f, RotateMode.LocalAxisAdd)
           .SetEase(Ease.Linear)
           .SetLoops(-1)
           .SetUpdate(true)
           .SetAutoKill(false);

        _waitingTween.Pause();

        _closeButton.onClick.AddListener(() =>
        {
            _onClose?.Invoke();
        });
    }

    public void Show(VerificationState verificationState, System.Action onClose, string error = null)
    {
        var entry = _stateEntries.FirstOrDefault(e => e.State == verificationState);

        _stateImage.sprite = entry.Icon;
        _stateText.text = entry.LocalizationKey;

        _closeButton.interactable = verificationState != VerificationState.Success;

        if (verificationState == VerificationState.Process)
        {
            _waitingTween.Restart();
        }
        else
        {
            if (verificationState == VerificationState.Failed && error != null)
            {
                _stateText.text = $"Try Again Later.<br>{error}";
            }

            _waitingTween.Pause();
            _stateImage.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        if (verificationState == VerificationState.Success)
        {
            Invoke(nameof(Hide), 2f);
        }

        _onClose = onClose;

        gameObject.SetActive(true);
    }

    private void Hide()
    {
        _closeButton.onClick.Invoke();
    }
}
