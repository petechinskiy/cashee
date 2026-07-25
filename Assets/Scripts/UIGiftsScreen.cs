using DG.Tweening;
using I2.Loc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GiftsData
{
    public bool WasPaid;
    public int BestGiftIndex;
    public int BestGiftPercentIndex;
    public int LeftSecondsToUpdate; // секунд до обновления подарков
    public List<GiftState> States;
}

[Serializable]
public class GiftState
{
    public int State;
    public int Value;
}

public class UIGiftsScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nextPayoutStateText;
    [SerializeField] private GameObject _balance;
    [SerializeField] private RectTransform _slotsParent;
    [SerializeField] private InfoPanel _successWindow;
    [SerializeField] private ParticleSystem _openingFX;
    [SerializeField] private GameObject[] _prefabs;

    private float _nextSlotAnimTime;
    private GiftsData _giftsData;
    private Tween _currentTween;
    private ApplicationController _appController;
    private readonly List<GameObject> _slots = new List<GameObject>();

    private void Awake()
    {
        _appController = FindFirstObjectByType<ApplicationController>();
    }

    private void Update()
    {
        if (_giftsData != null && !_giftsData.WasPaid && _giftsData.States.Any(e => e.State == 0) && Time.time > _nextSlotAnimTime)
        {
            var randomSlot = GetRandomSlot();

            if (randomSlot != null)
            {
                randomSlot.GetComponentsInChildren<Image>()[1].transform.DOShakePosition(1f, Vector2.right * 10f);
            }

            var cooldownTime = UnityEngine.Random.Range(1, 4);
            _nextSlotAnimTime = Time.time + cooldownTime;
        }
    }

    public void UpdateSlots(GiftsData giftsData)
    {
        /*
        var adsController = FindFirstObjectByType<CleverAdsController>();
        var allGiftsOpened = !giftsData.States.Any(e => e.State == 0);

        _giftsData = giftsData;

        foreach (var slot in _slots)
        {
            Destroy(slot);
        }

        _slots.Clear();

        for (int i = 0; i < giftsData.States.Count; i++)
        {
            int slotIndex = i;
            var state = giftsData.States[i];
            int prefabIndex = Mathf.Abs(state.State);
            var prefab = _prefabs[prefabIndex];
            var newSlot = Instantiate(prefab, _slotsParent);

            switch (state.State)
            {
                case 0:
                    if (giftsData.WasPaid)
                    {
                        newSlot.GetComponent<CanvasGroup>().alpha = 0.3f;
                        newSlot.GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        newSlot.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            var target = newSlot.GetComponentsInChildren<Image>()[1].transform;

                            _currentTween?.Kill();

                            target.rotation = Quaternion.identity;
                            target.localScale = Vector3.one;

                            _currentTween = DOTween.Sequence()
                            .Append(target.DOShakeRotation(1f, Vector3.forward * 10f))
                            .Join(target.DOPunchScale(Vector3.one * 0.2f, 1f))
                            .OnComplete(() =>
                            {
                                target.rotation = Quaternion.identity;
                                target.localScale = Vector3.one;

                                adsController.ShowGiftAds(slotIndex, (type, val) =>
                                {
                                    _appController.UpdatePayoutSlots();

                                    DOTween.Sequence()
                                    .AppendCallback(() =>
                                    {
                                        _openingFX.transform.position = newSlot.transform.position;
                                        _openingFX.Play();
                                    })
                                    .AppendInterval(1f)
                                    .AppendCallback(() =>
                                    {
                                        string text = null;

                                        if (type == 1)
                                        {
                                            text = string.Format(LocalizationManager.GetTranslation("Gifts/Popup/Coins"), val);
                                        }
                                        else
                                        {
                                            text = string.Format(LocalizationManager.GetTranslation("Gifts/Popup/Percent"), val);
                                        }

                                        _successWindow.Show(text);
                                    });
                                });
                            });
                        });
                    }
                    break;
                case 1:
                case -1:
                    newSlot.GetComponentInChildren<TextMeshProUGUI>().text = $"+{state.Value}";
                    break;
                case 2:
                case -2:
                    newSlot.GetComponentInChildren<TextMeshProUGUI>().text = $"<size=72>-{state.Value}%</size><br>ON NEXT PAYOUT";
                    break;
            }
            
            _slots.Add(newSlot);

            if (state.State > 0)
            {
                if (slotIndex != giftsData.BestGiftIndex && slotIndex != giftsData.BestGiftPercentIndex)
                {
                    newSlot.GetComponent<CanvasGroup>().alpha = 0.3f;
                }
            }
            else if (state.State < 0)
            {
                newSlot.GetComponent<CanvasGroup>().alpha = 0.3f;
            }
        }

        bool showStateText = allGiftsOpened || giftsData.WasPaid;
        _balance.SetActive(!showStateText);
        _nextPayoutStateText.gameObject.SetActive(showStateText);

        if (showStateText)
        {
            _nextPayoutStateText.text = giftsData.WasPaid ? "New gifts in<br>" + SecondsToString(giftsData.LeftSecondsToUpdate) : LocalizationManager.GetTranslation("Gifts/NextPayout");
        }
        */
    }

    public void UpdateTimer(int seconds)
    {
        _nextPayoutStateText.text = "New gifts in<br>" + SecondsToString(seconds);
    }

    public string SecondsToString(int seconds)
    {
        var t = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
    }

    private GameObject GetRandomSlot()
    {
        int slotsCount = _slots.Count;
        int startIndex = _giftsData.States.Count(e => e.State == 0) > 1 ? UnityEngine.Random.Range(0, slotsCount) : 0;
        GameObject targetSlot = null;

        for (int i = startIndex; i < slotsCount; i++)
        {
            var state = _giftsData.States[i];

            if (state.State == 0)
            {
                targetSlot = _slots[i];
                break;
            }

            if (i + 1 >= _slots.Count)
            {
                i = 0;
            }
        }

        return targetSlot;
    }
}
