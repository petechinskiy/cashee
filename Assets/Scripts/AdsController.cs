using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class AdsController : MonoBehaviour
{
    public AdData[] allAdData;
    [SerializeField] private GameObject fadeLoad;
    [SerializeField] private GameObject allPanels;
    [SerializeField] private Image logo;
    [SerializeField] private Image progressLine;
    [SerializeField] private Text applicationName;
    [SerializeField] private GameObject closeBtn;
    [SerializeField] private GameObject appBanner;
    [SerializeField] private VideoPlayer videoPlayer;

    private bool isPlaying;
    private string openUrl;

    private void OnEnable()
    {
        StartCoroutine(LoadAd());
    }

    private void Update()
    {
        progressLine.fillAmount = Mathf.Clamp01(1f - ((float)videoPlayer.frame / (float)videoPlayer.frameCount));
    }

    private void FixedUpdate()
    {
        if (!isPlaying)
            return;

        if (videoPlayer.frame >= videoPlayer.frameCount - 5f)
        {
            closeBtn.SetActive(true);
            appBanner.SetActive(true);
        }
    }

    public void Install()
    {
        Application.OpenURL(openUrl);
    }

    private IEnumerator LoadAd()
    {
        allPanels.SetActive(false);
        fadeLoad.SetActive(true);

        isPlaying = false;
        videoPlayer.Stop();
        int randData = Random.Range(0, allAdData.Length);
        videoPlayer.clip = allAdData[randData].allClips[Random.Range(0, allAdData[randData].allClips.Length)];
        videoPlayer.Prepare();

        yield return new WaitForSeconds(1f);
        allPanels.SetActive(true);
        videoPlayer.Play();
        openUrl = allAdData[randData].applicationUrl;

        logo.sprite = allAdData[randData].logo;
        applicationName.text = allAdData[randData].applicationName;

        closeBtn.SetActive(false);
        appBanner.SetActive(false);
        isPlaying = true;
    }
}

[System.Serializable]
public struct AdData
{
    public Sprite logo;
    [TextArea] public string applicationName;
    public string applicationUrl;
    public VideoClip[] allClips;
}
