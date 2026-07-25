using DG.Tweening;
using I2.Loc;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIDoubleMissionSlot : UIMissionSlot
{
    [SerializeField] private TextMeshProUGUI _secondDescriptionText;
    [SerializeField] private TextMeshProUGUI[] _progressTexts;

    private Sequence _progressSeq;

    protected override void OnEnable()
    {
        base.OnEnable();

        _progressSeq?.Restart();
    }

    public override void Show(MissionState state)
    {
        base.Show(state);

        _progressSeq = DOTween.Sequence().Pause();

        for (int i = 0; i < state.Progresses.Count; i++)
        {
            var progress = state.Progresses[i];
            var progressText = _progressTexts[i];
            var localization = LocalizationManager.GetTranslation($"MissionsWindow/ProgressType/{progress.Type}");

            if (progress.Current > 0)
            {
                _progressSeq
                    .Join(progressText.transform.parent.DOPunchScale(Vector3.one * 1.025f, 0.5f, 0));
            }

            _progressSeq
                .Join(DOTween.To((x) =>
                {
                    progressText.text = string.Format(localization, (int)x, progress.Total);
                }, 0, progress.Current, 3f));
        }

        var translation = LocalizationManager.GetTranslation($"Mission{state.Id}-1");
        var desc = string.Format(translation, ApplicationController.Instance.SettingsData.AdjoeForEarnButton ? "adjoe" : "prime");

        _descriptionText.text = desc;
        _secondDescriptionText.text = LocalizationManager.GetTranslation($"Mission{state.Id}-2");
    }
}
