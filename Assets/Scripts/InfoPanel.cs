using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Text _descText;
    [SerializeField] private TextMeshProUGUI _descTMP;

    private GameObject openedPanel;
    private bool isHiding;

    private void OnEnable()
    {
        isHiding = false;
    }

    public void DisableAnimator()
    {
        anim.enabled = false;
    }

    public void OpenPanel(GameObject panel)
    {
        if (panel != gameObject)
            openedPanel = panel;

        anim.enabled = true;
        anim.SetTrigger("Hide");

        isHiding = true;
    }

    public void DisableObject()
    {
        if (openedPanel != null)
            openedPanel.SetActive(true);

        gameObject.SetActive(false);
    }

    public void Show(string desc)
    {
        if (_descText)
        {
            _descText.text = desc;
        }
        if (_descTMP)
        {
            _descTMP.text = desc;
        }
        gameObject.SetActive(true);
    }
}
