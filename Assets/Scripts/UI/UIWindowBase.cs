using DG.Tweening;
using UnityEngine;

public class UIWindowBase : MonoBehaviour
{
    [SerializeField] private Transform _windowTransform;

    private CanvasGroup _canvasGroup;
    private Vector3 _windowScaleDefault;
    private Vector3 _windowScaleMin;

    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _windowScaleDefault = _windowTransform.localScale;
        _windowScaleMin = _windowScaleDefault * 0.92f;
        _canvasGroup.alpha = 0f;
        _windowTransform.localScale = _windowScaleMin;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);

        DOTween.Sequence()
            .AppendInterval(0.01f)
            .Append(_canvasGroup.DOFade(1f, 0.5f))
            .Join(_windowTransform.DOScale(_windowScaleDefault, 0.5f));
    }

    public virtual void Hide()
    {
        DOTween.Sequence()
            .Append(_canvasGroup.DOFade(0f, 0.25f))
            .Join(_windowTransform.DOScale(_windowScaleMin, 0.25f))
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });

    }
}
