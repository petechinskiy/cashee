using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISendRequestWindow : MonoBehaviour
{
    [SerializeField] private Button _sendButton;

    private void Awake()
    {
        _sendButton.onClick.AddListener(() =>
        {
            var text = _sendButton.GetComponentInChildren<TextMeshProUGUI>().text;
            ApplicationController.CopyToClickboard(text);
        });
    }
}
