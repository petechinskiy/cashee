using UnityEngine;
using UnityEngine.UI;

public class UIDailyStreakSlot : MonoBehaviour
{
    [SerializeField] private Image _slotImage;
    
    public void UpdateView(Sprite slotSprite)
    {
        _slotImage.sprite = slotSprite;
    }
}
