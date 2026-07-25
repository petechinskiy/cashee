using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TabView
{
    public TabType Type;
    public Toggle Toggle;
    public Transform Content;
}

public enum TabType
{
    Missions, DailyStreak
}

public class TabsController : MonoBehaviour
{
    [SerializeField] private List<TabView> _tabs;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private bool _tmpText;

    public TabView GetTab(TabType type)
    {
        return _tabs.FirstOrDefault(e => e.Type == type);
    }

    public void Init(Action<TabView> onClick = null)
    {

        foreach (var view in _tabs)
        {
            view.Toggle.onValueChanged.AddListener((isOn) =>
            {
                var color = isOn ? _selectedColor : _defaultColor;

                if (_tmpText)
                {
                    var text = view.Toggle.GetComponentInChildren<TextMeshProUGUI>();
                    text.color = color;
                }
                else
                {
                    var text = view.Toggle.GetComponentInChildren<Text>();
                    text.color = color;
                }

                if (isOn)
                {
                    onClick?.Invoke(view);
                }
            });
        }
    }
}
