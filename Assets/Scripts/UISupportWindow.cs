using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISupportWindow : MonoBehaviour
{
    [SerializeField] private Button _emailButton;
    [SerializeField] private Button _telegramButton;

    private void Awake()
    {
        _emailButton.onClick.AddListener(() => Application.OpenURL("mailto:reward@plus-games.com"));
        _telegramButton.onClick.AddListener(() => Application.OpenURL("https://t.me/casheesupport"));
    }
}
