using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryInfoTab : MonoBehaviour
{
    public InfoTime infoTime;

    [SerializeField] private Text moneyText;

    private void OnEnable()
    {
        switch (infoTime)
        {
            case InfoTime.day:
                moneyText.text = $"+{PlayerPrefs.GetInt($"earn_money_{DateTime.Now.DayOfWeek}_{DateTime.Now.Month}_{DateTime.Now.Year}", 0)}";
                break;
            case InfoTime.week:
                moneyText.text = $"+{PlayerPrefs.GetInt($"earn_money_{Mathf.CeilToInt(DateTime.Now.DayOfYear / 7)}_{DateTime.Now.Month}_{DateTime.Now.Year}", 0)}";
                break;
            case InfoTime.month:
                moneyText.text = $"+{PlayerPrefs.GetInt($"earn_money_{DateTime.Now.Month}_{DateTime.Now.Year}", 0)}";
                break;
        }
    }
}

public enum InfoTime
{
    day,
    week,
    month,
}
