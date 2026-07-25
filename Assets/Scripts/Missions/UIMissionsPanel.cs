using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIMissionsPanel : MonoBehaviour
{
    [SerializeField] private Button _offersButton;
    [SerializeField] private List<UIMissionSlot> _slots;

    private void Awake()
    {
        _offersButton.onClick.AddListener(() =>
        {
            ApplicationController.Instance.OnClickEarnButton();
        });
    }

    public void UpdateView(MissionsData data)
    {
        data.Missions = data.Missions.OrderBy(e => e.Id).ToList();

        for (int i = 0; i < data.Missions.Count; i++)
        {
            var mission = data.Missions[i];
            var slot = _slots[mission.Id - 1];

            slot.Show(mission);
            slot.transform.SetAsLastSibling();
        }
    }
}
