using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDontReceiveWindow : MonoBehaviour
{
    [SerializeField] private Button _paypalButton;
    [SerializeField] private Button _otherButton;
    [SerializeField] private GameObject _paypalTutorial;
    [SerializeField] private GameObject _otherTutorial;

    private void Awake()
    {
        _paypalButton.onClick.AddListener(() => _paypalTutorial.SetActive(true));
        _otherButton.onClick.AddListener(() => _otherTutorial.SetActive(true));
    }
}
