using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInviteFriendScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _codeText;
    [SerializeField] private Button _shareButton;

    private string _inviteCode;
    private const string GET_REFERRER_CODE_URI = "https://casheetrack.com/get_referrer_code.php";
    private const string INVITE_CODE_URI = "https://casheetrack.com/ref.php";

    private void Awake()
    {
        var appController = FindFirstObjectByType<ApplicationController>();
        var url = $"{GET_REFERRER_CODE_URI}?device_id={appController.DeviceId}";

        _codeText.text = string.Empty;
        _shareButton.interactable = false;

        appController.SendEvent(url, (code) =>
        {
            if (code.Length == 6)
            {
                _inviteCode = code;
                _codeText.text = code;
                _shareButton.interactable = true;
            }
        });

        _shareButton.onClick.AddListener(() =>
        {
            var link = $"{INVITE_CODE_URI}?ref_code={_inviteCode}";
            ApplicationController.CopyToClickboard(link);
        });
    }
}
