using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINavigationButton : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Color _textActiveColor;
    [SerializeField] private Color _textInactiveColor;
    [Header("Image")]
    [SerializeField] private Image _icon;
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Sprite _iconActiveSprite;
    [SerializeField] private Sprite _iconInactiveSprite;
    [SerializeField] private Color _buttonActiveColor;
    [SerializeField] private Color _buttonInactiveColor;

    public void SetSelect(bool selected)
    {
        if (_icon)
        {
            _icon.sprite = selected ? _iconActiveSprite : _iconInactiveSprite;
        }

        if (_buttonImage)
        {
            _buttonImage.color = selected ? _buttonActiveColor : _buttonInactiveColor;
        }

        _text.color = selected ? _textActiveColor : _textInactiveColor;
    }
}