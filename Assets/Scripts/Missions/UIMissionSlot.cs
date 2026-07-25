using DG.Tweening;
using I2.Loc;
using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionSlot : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private Image _completedCheckboxImage;
    [SerializeField] private Sprite _completedCheckboxSprite;
    [SerializeField] private Sprite _defaultCheckboxSprite;

    protected Sequence _descriptionSeq;
    private const string DESCRIPTION_HIGHLIGHT_COLOR = "#F4C135";

    protected virtual void OnEnable()
    {
        _descriptionSeq?.Restart();
    }

    public virtual void Show(MissionState state)
    {
        var rewardText = "+ $ " + Math.Round(state.Reward, 2).ToString("0.00", CultureInfo.InvariantCulture);

        var translation = LocalizationManager.GetTranslation($"Mission{state.Id}");

        if (state.Id != 4)
        {
            var desc = state.Id == 3 ? translation : string.Format(translation, ApplicationController.Instance.SettingsData.AdjoeForEarnButton ? "adjoe" : "prime");

            _descriptionText.text = desc;
        }
        
        _rewardText.text = rewardText;
        _completedCheckboxImage.sprite = state.Completed ? _completedCheckboxSprite : _defaultCheckboxSprite;

        if (state.Completed)
        {
            ColorUtility.TryParseHtmlString(DESCRIPTION_HIGHLIGHT_COLOR, out Color descColor);

            _descriptionSeq = DOTween.Sequence().Pause();
            _descriptionSeq
                .AppendInterval(1f)
                .Append(_descriptionText.DOColor(descColor, 1f).SetLoops(2, LoopType.Yoyo))
                .Join(_completedCheckboxImage.transform.DOPunchScale(Vector3.one * 1.25f, 0.5f, 0));
        }
    }
}
