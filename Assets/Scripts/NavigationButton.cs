using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavigationButton : MonoBehaviour
{
    public GameObject panel;

    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _inactiveColor;

    public void OnClick()
    {
        if (ApplicationController.Instance.canChangePanel)
            ApplicationController.Instance.NavigationButtonSelect(this);
    }

    public void ActiveButton()
    {
        ApplicationController.Instance.ChangePanel(panel);
        _iconImage.color = _text.color = _activeColor;
    }

    public void UnactiveButton()
    {
        _iconImage.color = _text.color = _inactiveColor;
    }
}
